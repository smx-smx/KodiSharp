#define _GNU_SOURCE
#include <stdio.h>
#include <stdbool.h>
#include <string.h>
#include <stdint.h>
#include <assert.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/threads.h>
#include <mono/utils/mono-error.h>

#include "host.h"
#include "utils.h"

static MonoDomain *domain = NULL;
static MonoAssembly *assembly = NULL;
static MonoImage* image = NULL;

static MonoClass *pluginClass = NULL;
static MonoObject *pluginInstance = NULL;

static bool initialized = false;
static char *MainMethodName = NULL;

static message_callback_t py_onMessage = NULL;
static exit_callback_t py_exit = NULL;

#if USE_INTERNAL_METHODS
static MonoString *Python_OnMessage(MonoString *messageData){
	dprintf("\n");
	assert(py_onMessage != NULL);

	char *jsonData = mono_string_to_utf8(messageData);
	char *reply = py_onMessage(jsonData);
	free(jsonData);

	return mono_string_new(domain, reply);
}

static void Python_Exit(){
	dprintf("\n");
	assert(py_exit != NULL);
	py_exit();
}
#endif

extern void SetMainMethodName(char *methodName){
	dprintf("\n");

	if(MainMethodName != NULL)
		free(MainMethodName);

	size_t sz = strlen(methodName) + 1;
	MainMethodName = calloc(1, sz);
	strncpy(MainMethodName, methodName, sz);
}


extern int CreateInstance(char *namespace, char *className){
	dprintf("\n");

	pluginClass = mono_class_from_name(image, namespace, className);
	if(!pluginClass){
		fprintf(stderr, "Failed to find class '%s'\n", className);
		return -1;
	}

	pluginInstance = mono_object_new(domain, pluginClass);
	if(!pluginInstance){
		fprintf(stderr, "Failed to allocate object for '%s'\n", className);
		return -1;
	}

	//TODO: mono_runtime_object_init_checked could check for error, but it's not exported
	mono_runtime_object_init(pluginInstance);
	return 0;
}

/*
 * This is an alternative way of passing C callbacks to a Mono program
 * Instead of passing an IntPtr / delegate we can bind a C# method so that it will call a C function
 */ 
#if USE_INTERNAL_METHODS
static void SetPythonCallbacks(message_callback_t cb1, exit_callback_t cb2){
	dprintf("Binding Python Callbacks to C# (%p <= %p, %p <= %p)\n", py_onMessage, cb1, py_exit, cb2);
	py_onMessage = cb1;
	py_exit = cb2;

	mono_add_internal_call("Smx.KodiInterop.MonoNativeBridge::PySendString", Python_OnMessage);
	mono_add_internal_call("Smx.KodiInterop.MonoNativeBridge::PyExit", Python_Exit);
}
#endif

/*
 * Initializes the Mono runtime
 */
extern int clrInit(char *assemblyPath, message_callback_t cb1, exit_callback_t cb2){
	dprintf("\n");

	/*
	 * If we are already initialized, don't do anything
	 * The Mono runtime is kept persistent to allow static classes to survive
	 */
	if(initialized){
		return 1;
	}

	// Load the default mono configuration file
	mono_config_parse(NULL);

	domain = mono_jit_init(assemblyPath);
	if(!domain){
		fprintf(stderr, "Failed to initialize mono\n");
		return -1;
	}

	// This is an alternative to passing function pointers
#if USE_INTERNAL_METHODS
	SetPythonCallbacks(cb1, cb2);
#endif

	/*
	 * Set the domain basedir and the
	 * (required) optional configuration file path
	 *
	 * base_dir gets	: the directory where the Assembly is located
	 * config_file_name : the name of the Assembly without extension
	 */
	char *assemblyName = my_basename(assemblyPath);
	char *assemblyDir = my_dirname(assemblyPath);
	char *assemblyFileName = remove_ext(assemblyName);
	char *assemblyConfigFile;
	asprintf(&assemblyConfigFile, "%s.config", assemblyFileName);
	
	mono_domain_set_config(domain, assemblyDir, assemblyConfigFile);
	
	free(assemblyConfigFile);
	free(assemblyDir);
	free(assemblyName);

	assembly = mono_domain_assembly_open(domain, assemblyPath);
	if(!assembly){
		fprintf(stderr, "Failed to open assembly '%s'\n", assemblyPath);
		return -2;
	}

	image = mono_assembly_get_image(assembly);
	if(!image){
		fprintf(stderr, "Failed to get image\n");
		return -3;
	}

	initialized = true;
	return 0;
}

/*
 * Unloads the app domain with all the assemblies it contains
 * As per mono docs, once cleanup is called, mono cannot be re-loaded
 * until the process is restarted
 */
extern void clrDeInit(){
	dprintf("\n");

	if(domain)
		mono_jit_cleanup(domain);
}

///////////// Plugin Interface
/*
 * These are C bindings to C# methods.
 * Calling any of the methods below will call the respective C# method in the loaded assembly
 */

extern bool Initialize(message_callback_t cb1, exit_callback_t cb2, bool debug){
	dprintf("\n");
	assert(image != NULL);

	MonoMethodDesc *desc = mono_method_desc_new("KodiBridge::Initialize", false);
	MonoMethod *method = mono_method_desc_search_in_image(desc, image);
	mono_method_desc_free(desc);
	assert(method != NULL);

	void *args[] = {&cb1, &cb2, &debug, NULL};

	mono_thread_attach (domain);
	MonoObject *ret = mono_runtime_invoke(method, NULL, args, NULL);

	return *(bool*)mono_object_unbox(ret);
}

extern int PluginMain(){
	dprintf("Calling %s\n", MainMethodName);
	assert(image != NULL);

	MonoMethodDesc *desc = mono_method_desc_new(MainMethodName, true);
	MonoMethod *method = mono_method_desc_search_in_image(desc, image);
	mono_method_desc_free(desc);
	assert(method != NULL);

	mono_thread_attach (domain);
	MonoObject *ret = mono_runtime_invoke(method, NULL, NULL, NULL);

	return *(int *)mono_object_unbox(ret);
}

extern bool PostEvent(char *eventMessage){
	dprintf("\n");
	assert(image != NULL);

	MonoMethodDesc *desc = mono_method_desc_new("KodiBridge::PostEvent(string)", false);
	MonoMethod *method = mono_method_desc_search_in_image(desc, image);
	mono_method_desc_free(desc);
	assert(method != NULL);

	void *args[] = {&eventMessage, NULL};

	mono_thread_attach (domain);
	MonoObject *ret = mono_runtime_invoke(method, NULL, args, NULL);
	return *(bool *)mono_object_unbox(ret);
}

extern bool StopRPC(){
	dprintf("\n");
	assert(image != NULL);

	MonoMethodDesc *desc = mono_method_desc_new("KodiBridge::StopRPC()", false);
	MonoMethod *method = mono_method_desc_search_in_image(desc, image);
	mono_method_desc_free(desc);
	assert(method != NULL);

	mono_thread_attach (domain);
	MonoObject *ret = mono_runtime_invoke(method, NULL, NULL, NULL);
	return *(bool *)mono_object_unbox(ret);
}
