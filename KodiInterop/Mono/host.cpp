#include <stdio.h>
#include <stdbool.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/threads.h>
#include <mono/metadata/mono-gc.h>
#include <mono/utils/mono-error.h>

#include <iostream>
#include <map>
#include <filesystem>

#define DEBUG

#ifdef _WIN32
#include <sstream>
#include <Windows.h>
#endif

#ifdef _WIN32
#define DLLEXPORT __declspec(dllexport) extern
#else
#define DLLEXPORT extern
#endif

#ifdef DEBUG
#define DPRINTF(fmt, ...) \
	fprintf(stderr, "[%s]: " fmt, __func__, ##__VA_ARGS__)
#else
#define DPRINTF(fmt, ...)
#endif


#define FN_GETPLUGINMAIN "KodiBridgeABI:GetPluginMainFunc"
#define FN_ININTIALIZE "KodiBridgeABI:GetInitializeFunc"
#define FN_GETPOSTEVENT "KodiBridgeABI:GetPostEventFunc"


#if !defined(_WIN32) && !defined(__cdecl)
#define __cdecl __attribute__((__cdecl__))
#endif

extern "C" {
	typedef size_t PLGHANDLE;

	// Python functions
	typedef char *(*message_callback_t)(char *message);
	typedef void(*exit_callback_t)();

	// C# functions
	typedef int(__cdecl *PluginMainProc)();
	typedef bool(__cdecl *InitializeProc)(message_callback_t cb1, exit_callback_t cb2, bool enableDebug);
	typedef bool(__cdecl *PostEventProc)(const char *eventMessage);
}

struct PluginInstanceData {
	PluginInstanceData(){}
public:
	MonoDomain *appDomain;
	InitializeProc initialize;
	PluginMainProc pluginMain;
	PostEventProc postEvent;

	PluginInstanceData(MonoDomain *pAppDomain, PluginMainProc pfnPluginMain, InitializeProc pfnInitialize, PostEventProc pfnPostEvent)
		:appDomain(pAppDomain), pluginMain(pfnPluginMain), initialize(pfnInitialize), postEvent(pfnPostEvent)
	{
	}
};

static std::map<PLGHANDLE, PluginInstanceData> gPlugins;
#define NULL_PLGHANDLE 0

static MonoDomain *rootDomain = nullptr;
static bool gMonoInitialized = false;

size_t str_hash(const char *str)
{
	unsigned long hash = 5381;
	int c;

	while (c = *str++)
		hash = ((hash << 5) + hash) + c; /* hash * 33 + c */

	return hash;
}

static MonoMethod *imageFindMethod(MonoImage *image, const char *methodName) {
	MonoMethodDesc *desc = mono_method_desc_new(methodName, false);
	if (desc == nullptr) {
		fprintf(stderr, "Invalid method name '%s'\n", methodName);
		return nullptr;
	}
	MonoMethod *method = mono_method_desc_search_in_image(desc, image);
	mono_method_desc_free(desc);
	if (method == nullptr) {
		return nullptr;
	}
	return method;
}

static void *callDelegateGetter(MonoImage *image, const char *methodName, void **args) {
	MonoMethod *method = imageFindMethod(image, methodName);
	if (method == nullptr) {
		fprintf(stderr, "Method '%s' not found in image\n", methodName);
		return nullptr;
	}
	DPRINTF("Resolved method '%s'\n", methodName);

	MonoObject *exception = nullptr;
	MonoObject *ret = mono_runtime_invoke(method, nullptr, args, &exception);

	if (exception) {
		mono_print_unhandled_exception(exception);
		fprintf(stderr, "Press any key to continue\n");
		getchar();
	}

	void *data;
	if (ret == nullptr || ((data = mono_object_unbox(ret)) == nullptr)) {
		fprintf(stderr, "Method invocation failed for method '%s'\n", methodName);
		return nullptr;
	}
	return *(void **)data;
}

#ifdef _WIN32
//https://stackoverflow.com/a/20387632
bool launchDebugger() {
	std::wstring systemDir(MAX_PATH + 1, '\0');
	UINT nChars = GetSystemDirectoryW(&systemDir[0], systemDir.length());
	if (nChars == 0)
		return false;
	systemDir.resize(nChars);

	DWORD pid = GetCurrentProcessId();
	std::wostringstream s;
	s << systemDir << L"\\vsjitdebugger.exe -p " << pid;
	std::wstring cmdLine = s.str();

	STARTUPINFOW si;
	memset(&si, 0x00, sizeof(si));
	si.cb = sizeof(si);

	PROCESS_INFORMATION pi;
	memset(&pi, 0x00, sizeof(pi));

	if (!CreateProcessW(
		nullptr, &cmdLine[0],
		nullptr, nullptr,
		false, 0, nullptr, nullptr,
		&si, &pi
	)) {
		return false;
	}

	CloseHandle(pi.hThread);
	CloseHandle(pi.hProcess);

	while (!IsDebuggerPresent())
		Sleep(100);

	DebugBreak();
	return true;
}

#endif

//#define LAUNCH_DEBUGGER

extern "C" {
	/*
	 * Initializes the Mono runtime
	 */
	DLLEXPORT const PLGHANDLE __cdecl clrInit(
		const char *assemblyPath, const char *pluginFolder,
		message_callback_t cb1, exit_callback_t cb2, bool enableDebug
	) {
#if defined(_WIN32) && defined(DEBUG)

#ifdef LAUNCH_DEBUGGER
		launchDebugger();
#endif

		AllocConsole();
		freopen("CONIN$", "r", stdin);
		freopen("CONOUT$", "w", stdout);
		freopen("CONOUT$", "w", stderr);

#if 0
		setvbuf(stdout, nullptr, 0, _IONBF);
		setvbuf(stderr, nullptr, 0, _IONBF);
#endif
#endif

		DPRINTF("\n");

		/**
		 * Get Paths
		 */
		std::filesystem::path asmPath(assemblyPath);
		std::string pluginName = asmPath.filename().replace_extension().string();

		PLGHANDLE handle = str_hash(pluginName.c_str());
		if (gPlugins.find(handle) != gPlugins.end()) {
			gPlugins.at(handle).initialize(cb1, cb2, enableDebug);
			return handle;
		}

		DPRINTF("loading %s\n", assemblyPath);

		if (!gMonoInitialized) {
			DPRINTF("initializing mono\n");

			// Load the default mono configuration file
			rootDomain = mono_jit_init_version("KodiSharp", "v4.0");
			if (!rootDomain) {
				fprintf(stderr, "Failed to initialize mono\n");
				return NULL_PLGHANDLE;
			}
			mono_config_parse(nullptr);

			gMonoInitialized = true;
			DPRINTF("mono initialization completed\n");
		}

		/**
		 * Create AppDomain
		 */
		DPRINTF("creating appdomain...\n");
		MonoDomain *newDomain = mono_domain_create_appdomain(const_cast<char *>(pluginName.c_str()), nullptr);
		std::string configPath = asmPath.replace_extension().string() + ".config";
		mono_domain_set_config(newDomain, pluginFolder, configPath.c_str());


		DPRINTF("loading assembly...\n");
		MonoAssembly *pluginAsm = mono_domain_assembly_open(newDomain, assemblyPath);
		if (!pluginAsm) {
			fprintf(stderr, "Failed to open assembly '%s'\n", assemblyPath);
			return NULL_PLGHANDLE;
		}

		MonoImage *image = mono_assembly_get_image(pluginAsm);
		if (!image) {
			fprintf(stderr, "Failed to get image\n");
			return NULL_PLGHANDLE;
		}

		// NOTE: can't use mono_assembly_load_references (it's deprecated and does nothing)

		int numAssemblies = mono_image_get_table_rows(image, MONO_TABLE_ASSEMBLYREF);
		for (int i = 0; i < numAssemblies; i++) {
			mono_assembly_load_reference(image, i);
		}
		
		PluginMainProc pPluginMain = nullptr;
		InitializeProc pInitialize = nullptr;
		PostEventProc pPostEvent = nullptr;

		void *pUserData[] = {
			(void *)&pPluginMain,
			(void *)&pInitialize,
			(void *)&pPostEvent,
			(void *)&cb1,
			(void *)&cb2,
			(void *)&enableDebug
		};

		MonoThread *thread = mono_thread_attach(newDomain);

		DPRINTF("%d\n", numAssemblies);
		mono_assembly_foreach([](void *refAsmPtr, void *userdata) {
			void **pPointers = (void **)userdata;
			PluginMainProc* ppPluginMain = (PluginMainProc *)pPointers[0];
			InitializeProc* ppInitialize = (InitializeProc *)pPointers[1];
			PostEventProc *ppPostEvent = (PostEventProc *)pPointers[2];
			
			if (*ppPluginMain != nullptr && *ppInitialize != nullptr && *ppPostEvent != nullptr)
				return;

			message_callback_t *pcb1 = (message_callback_t *)pPointers[3];
			exit_callback_t *pcb2 = (exit_callback_t *)pPointers[4];
			bool *pDebugEnable = (bool *)pPointers[5];


			MonoAssembly *refAsm = (MonoAssembly *)refAsmPtr;
			const char *asmName = mono_assembly_name_get_name(mono_assembly_get_name(refAsm));

			MonoImage *refAsmImage = mono_assembly_get_image(refAsm);
			if (refAsmImage == nullptr) {
				fprintf(stderr, "Cannot get image for assembly '%s'\n", asmName);
				return;
			}

			DPRINTF("Loaded '%s'\n", asmName);

			if (*ppPluginMain == nullptr) {
				void *args[] = { pcb1, pcb2, pDebugEnable };
				*ppPluginMain = (PluginMainProc)callDelegateGetter(refAsmImage, FN_GETPLUGINMAIN, args);
			}

			if (*ppInitialize == nullptr) {
				*ppInitialize = (InitializeProc)callDelegateGetter(refAsmImage, FN_ININTIALIZE, nullptr);
			}

			if(*ppPostEvent == nullptr)
				*ppPostEvent = (PostEventProc)callDelegateGetter(refAsmImage, FN_GETPOSTEVENT, nullptr);
		}, pUserData);
		mono_thread_detach(thread);

	
		DPRINTF("PluginMain: %p\n", pPluginMain);
		DPRINTF("Initialize: %p\n", pPluginMain);
		DPRINTF("PostEvent : %p\n", pPostEvent);
		if (pPluginMain == nullptr || pInitialize == nullptr || pPostEvent == nullptr) {
			fprintf(stderr, "Cannot resolve methods\n");
			return NULL_PLGHANDLE;
		}

		gPlugins.emplace(handle, PluginInstanceData(newDomain, pPluginMain, pInitialize, pPostEvent));
		return handle;
	}

	/*
	 * Unloads the app domain with all the assemblies it contains
	 */
	DLLEXPORT bool __cdecl clrDeInit(PLGHANDLE handle) {
		DPRINTF("\n");

		PluginInstanceData data = gPlugins[handle];
		mono_domain_unload(data.appDomain);

		gPlugins.erase(handle);
		return true;
	}

	///////////// Plugin Interface
	/*
	 * These are C bindings to C# methods.
	 * Calling any of the methods below will call the respective C# method in the loaded assembly
	 */
	DLLEXPORT int __cdecl PluginMain(PLGHANDLE handle) {
		DPRINTF("\n");
		return gPlugins.at(handle).pluginMain();
	}

	DLLEXPORT bool __cdecl PostEvent(PLGHANDLE handle, const char *eventMessage) {
		DPRINTF("\n");
		return gPlugins.at(handle).postEvent(eventMessage);
	}
}
