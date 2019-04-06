#include <msclr/marshal_cppstd.h>
#include <filesystem>
#include <string>
#include <map>
#include <unordered_map>
#include <vcclr.h>

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Diagnostics;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Windows::Forms;
using namespace Smx::KodiInterop;

typedef size_t PLGHANDLE;

/**
 * Loads an assembly by looking up the same folder as the executing assembly
 */
ref struct SameFolderAssemblyLoader {
public:
	static Assembly^ LoadFromSameFolder(Object^ sender, ResolveEventArgs^ args) {
		String^ folderPath = Path::GetDirectoryName(Assembly::GetExecutingAssembly()->Location);
		String^ assemblyName = (gcnew AssemblyName(args->Name))->Name + ".dll";
		String^ assemblyPath = Path::Combine(folderPath, assemblyName);
		if (!File::Exists(assemblyPath))
			return nullptr;
		Assembly^ assembly = Assembly::LoadFrom(assemblyPath);
		return assembly;
	}
};

/**
 * Loads an assembly by looking up a list of folders
 */
ref class PathListAssemblyLoader {
	List<String^>^ asmPathList;
	Assembly^ LoadFromPathList(Object^ sender, ResolveEventArgs^ args) {
		for each(String^ dirPath in asmPathList) {
			String^ assemblyName = (gcnew AssemblyName(args->Name))->Name + ".dll";
			String^ candidatePath = Path::Combine(dirPath, assemblyName);
			if (File::Exists(candidatePath)) {
				Assembly^ assembly = Assembly::LoadFrom(candidatePath);
				return assembly;
			}
		}
		return nullptr;
	}

public:
	PathListAssemblyLoader(List<String^>^ asmPathList) {
		AppDomain ^appDom = AppDomain::CurrentDomain;
		this->asmPathList = asmPathList;
		appDom->AssemblyResolve += gcnew ResolveEventHandler(this, &PathListAssemblyLoader::LoadFromPathList);
	}

	Assembly^ LoadFrom(String^ assemblyPath) {
		AppDomain ^appDom = AppDomain::CurrentDomain;

		AssemblyName^ asmName = gcnew AssemblyName();
		asmName->CodeBase = assemblyPath;
		return appDom->Load(asmName);
	}

};

/**
 * Wraps the plugin instance
 */
[Serializable]
ref class PluginInstance : MarshalByRefObject {
private:
	Assembly^ pluginAssembly;
	MethodInfo^ PluginMainMethod;

public:
	PluginInstance(String^ assemblyPath, List<String^>^ asmSearchPaths) {
		pluginAssembly = (gcnew PathListAssemblyLoader(asmSearchPaths))->LoadFrom(assemblyPath);
		PluginMainMethod = KodiAbstractBridge::FindPluginMain();
	}

	bool Initialize(IntPtr cb1, IntPtr cb2, bool enableDebug) {
		return KodiBridge::Initialize(cb1, cb2, enableDebug);
	}
	
	bool PostEvent(String^ eventMessage) {
		return KodiBridge::PostEvent(eventMessage);
	}

	int PluginMain() {
		return safe_cast<int>(PluginMainMethod->Invoke(nullptr, nullptr));
	}
};

struct PluginInstanceData {
	PluginInstanceData(){}
public:
	gcroot<AppDomain^> appDomain;
	gcroot<PluginInstance^> instance;

	PluginInstanceData(AppDomain^ appDomain, PluginInstance^ instance) : appDomain(appDomain), instance(instance){}
};

static std::map<PLGHANDLE, PluginInstanceData> gPlugins;


size_t str_hash(const char *str)
{
	unsigned long hash = 5381;
	int c;

	while (c = *str++)
		hash = ((hash << 5) + hash) + c; /* hash * 33 + c */

	return hash;
}

//#define LAUNCH_DEBUGGER

extern "C" {
	typedef char *(*message_callback_t)(const char *);
	typedef void(*exit_callback_t)();

	__declspec(dllexport)
	extern bool __cdecl PostEvent(PLGHANDLE handle, const char *eventMessage) {
		return gPlugins.at(handle).instance->PostEvent(gcnew String(eventMessage));
	}

	__declspec(dllexport)
	extern int __cdecl PluginMain(PLGHANDLE handle) {
		return gPlugins.at(handle).instance->PluginMain();
	}

	__declspec(dllexport)
	extern const PLGHANDLE __cdecl clrInit(
		const char *assemblyPath, const char *pluginFolder,
		message_callback_t cb1, exit_callback_t cb2, bool enableDebug
	) {
		if (enableDebug) {
			if (!Debugger::IsAttached) {
				Debugger::Launch();
			}
			while (!Debugger::IsAttached) {
				Thread::Sleep(TimeSpan::FromSeconds(1));
			}
			//Debugger::Break();
		}

		/**
		 * Get Paths
		 */
		std::filesystem::path asmPath(assemblyPath);
		std::string pluginName = asmPath.filename().replace_extension().string();

		PLGHANDLE handle = str_hash(pluginName.c_str());

		if(gPlugins.find(handle) != gPlugins.end()){
			gPlugins.at(handle).instance->Initialize(IntPtr(cb1), IntPtr(cb2), enableDebug);
			return handle;
		}

		/**
		 * Create AppDomain
		 */
		String^ applicationName = gcnew String(pluginName.c_str());
		AppDomainSetup^ domainSetup = gcnew AppDomainSetup();
		domainSetup->ApplicationName = applicationName;
		domainSetup->ApplicationBase = gcnew String(pluginFolder);
		AppDomain^ newDomain = AppDomain::CreateDomain(applicationName, nullptr, domainSetup);

		/**
		 * Use the Directory we're running from to resolve assemblies (e.g this assembly)
		 * We have to use this because ApplicationBase and PrivateBinPath aren't sane (ApplicationBase will point to the Kodi's exe dir)
		 * Since PrivateBinPath entries are resolved relative to the ApplicationBase, it's a no-go.
		 * We would need to control the AppDomain creation in pure C++ (e.g before this code is ran)
		 * 
		 * CreateInstanceFromAndUnwrap will try to resolve where the Type/Class is (it's in this assembly)
		 * and it will try to load that assembly in the new AppDomain.
		 * 
		 * The current AppDomain is used for the resolution, so we need to install a LoadFromSameFolder resolver handler
		 * on the current AppDomain so that the CLRHost assembly can be found
		 */
		AppDomain::CurrentDomain->AssemblyResolve += gcnew ResolveEventHandler(&SameFolderAssemblyLoader::LoadFromSameFolder);

		/**
		 * Setup Assembly search paths
		 */

		String^ thisAsmPath = Assembly::GetExecutingAssembly()->Location;
		List<String^>^ asmPaths = gcnew List<String^> ();
		asmPaths->Add(Path::GetDirectoryName(thisAsmPath));

		asmPaths->Add(gcnew String(asmPath.parent_path().c_str()));
		
		/**
		 * Run the PluginLoader on the new appdomain
		 * We do this by using a Serializable,MarshalByRefObject class, which gets serialized to the new AppDomain and ran there
		 */
		array<Object^>^ args = gcnew array<Object^>(2);
		args[0] = gcnew String(asmPath.c_str());
		args[1] = asmPaths;
		PluginInstance^ pl = (PluginInstance^)newDomain->CreateInstanceFromAndUnwrap(
			thisAsmPath,
			(PluginInstance::typeid)->FullName, false,
			BindingFlags::Default,
			nullptr, args,
			nullptr, nullptr
		);
		pl->Initialize(IntPtr(cb1), IntPtr(cb2),enableDebug);

		gPlugins.emplace(handle, PluginInstanceData(newDomain, pl));
		return handle;
	}

	__declspec(dllexport)
	extern bool __cdecl clrDeInit(PLGHANDLE handle) {
		PluginInstanceData data = gPlugins[handle];
		AppDomain::Unload(data.appDomain);

		gPlugins.erase(handle);
		return true;
	}
}