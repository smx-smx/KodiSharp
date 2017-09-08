##
## Copyright (C) 2017 Smx <smxdev4@gmail.com>
##
import os
import sys
import json
import threading
import traceback
from ctypes import *


# Load all XBMC Apis so C# can use them
import xbmc
import xbmcgui
import xbmcplugin
import xbmcaddon
import xbmcvfs

#### Config
me = os.path.abspath(os.path.dirname(__file__))
assembly_path = os.path.join(me, "KodiInterop/TestPlugin/bin/x64/Debug", "TestPlugin.dll")
monohost_path = os.path.join(me, "KodiInterop/Mono/build", "libmonoHost.so")
plugin_namespace = "TestPlugin"
plugin_main = "Main"


class Errors:
	SUCCESS = 0
	RESULT_NOT_SERIALIZABLE = 1

Variables = {
	"LastResult": ""
}

Monitors = {}
Players = {}

not_windows = not "win" in sys.platform

#mutex = threading.Lock()
close_event = threading.Event()

if not_windows:
	lib_path = monohost_path
else:
	lib_path = assembly_path

lib = cdll.LoadLibrary(lib_path)
print lib

## C# Functions
# Callback Type
message_callback_type = CFUNCTYPE(c_char_p, c_char_p)
main_callback_type = CFUNCTYPE(None)

# Plugin init
Initialize = lib.Initialize
Initialize.argtypes = [message_callback_type, main_callback_type, c_bool]
Initialize.restype = c_bool

# Plugin entrypoint
Main = lib.PluginMain
Main.argtypes = []
Main.restype = c_int

# Send the event message in JSON format
PostEvent = lib.PostEvent
PostEvent.argtypes = [c_char_p]
PostEvent.restype = c_bool

# Causes C# to send the quit signal to this script
StopRPC = lib.StopRPC
StopRPC.argtypes = []
StopRPC.restype = c_bool

if not_windows:
	CreateInstance = lib.CreateInstance
	CreateInstance.argtypes = [c_char_p, c_char_p]
	CreateInstance.restype = c_int

	ClrInit = lib.clrInit
	ClrInit.argtypes = [c_char_p, message_callback_type, main_callback_type]
	ClrInit.restype = c_int

	SetMainMethodName = lib.SetMainMethodName
	SetMainMethodName.argtypes = [c_char_p]
	SetMainMethodName.restype = None

	ClrDeInit = lib.clrDeInit
	ClrDeInit.argtypes = []
	ClrDeInit.restype = None

def exception_hook(exc_type, exc_value, exc_traceback):
	print "--- Caught Exception ---"
	lines = traceback.format_exception(exc_type, exc_value, exc_traceback)
	print ''.join('!! ' + line for line in lines)
	print "------------------------"

def json_error(exitcode):
	return json.dumps({
		"exit_code": exitcode
	})

def reaper():
	close_event.set()

def on_exit():
	# Start a thread so C# gets a return meanwhile
	rt = threading.Thread(target=reaper)
	rt.start()

def on_message(data):
	try:
		message = json.loads(data)
		print message
	except Exception as exc:
		type, value, traceback = sys.exc_info()
		exception_hook(type, value, traceback)
		return json_error(1)

	type = message.get('type')
	if not type:
		print "Invalid Message Type"
		return json_error(1)

	#mutex.acquire()
	if type == 'exit':
		print "We got exit, bye"
		close_event.set()
		#mutex.release()
		return json_error(0)
	elif type == 'eval':
		exec_code = message.get('exec_code')
		if exec_code:
			try:
				eval(compile(exec_code, '<string>', 'exec'), globals())
			except Exception as exc:
				type, value, traceback = sys.exc_info()
				exception_hook(type, value, traceback)
				#mutex.release()
				return json_error(1)
	elif type == 'del_var':
		var_name = message.get('var_name')
		if var_name and var_name in Variables:
			del Variables[var_name]

	result = Variables['LastResult']
	#mutex.release()
	typeName = result.__class__.__name__
	print "=> TypeName is " + typeName
	
	jDict = {
		"type": typeName,
		"value": result,
		"exit_code": Errors.SUCCESS
	}

	try:
		jsonData = json.dumps(jDict)
	except TypeError:
		jDict['value'] = None
		jDict['exit_code'] = Errors.RESULT_NOT_SERIALIZABLE
		jsonData = json.dumps(jDict)
	
	return jsonData


################################################

sys.excepthook = exception_hook

MessageCallbackFunc = message_callback_type(on_message)
MainCallbackFunc = main_callback_type(on_exit)

if not_windows:
	SetMainMethodName(plugin_namespace + "::" + plugin_main)
	ClrInit(assembly_path, MessageCallbackFunc, MainCallbackFunc)
#	CreateInstance(plugin_namespace, plugin_class)

# Last parameter is enableDebug(bool)
print "Calling initialize"
ret = Initialize(MessageCallbackFunc, MainCallbackFunc, True)
print "Initialize returned %d" % ret

close_event.clear()

# Invoke PluginMain from C#
ret = Main()
print "PluginMain returned %d" % ret

print "Waiting for RPC exit..."
close_event.wait()
print "Got RPC exit, exiting"

# Close handle to DLL
del lib
print "Python RPC Exiting..."
