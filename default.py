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

class Errors:
	SUCCESS = 0
	RESULT_NOT_SERIALIZABLE = 1

csMonitorVar = "__csharpMonitor"
csPlayerVar = "__csharpPlayer"

Variables = {
	"LastResult": ""
}

Monitors = {}
Players = {}

me = os.path.abspath(os.path.dirname(__file__))
lib = cdll.LoadLibrary(os.path.join(me, "KodiInterop/TestPlugin/bin/Debug", "TestPlugin.dll")) # Hardcoding this for now
print lib

## C# Functions
# Get the next command in JSON format
GetMessage = lib.GetMessage
GetMessage.argtypes = []
GetMessage.restype = c_char_p

# Send the command result in JSON format
PostMessage = lib.PostMessage
PostMessage.argtypes = [c_char_p]
PostMessage.restype = c_bool

# Send the event message in JSON format
PostEvent = lib.PostEvent
PostEvent.argtypes = [c_char_p]
PostEvent.restype = c_bool

# Callback Type
message_callback_type = CFUNCTYPE(c_char_p, c_char_p)

# Plugin init
Initialize = lib.Initialize
Initialize.argtypes = [message_callback_type, c_bool]
Initialize.restype = c_bool

# Plugin entrypoint
Main = lib.PluginMain
Main.argtypes = []
Main.restype = c_int

# Causes C# to send the quit signal to this script
StopRPC = lib.StopRPC
StopRPC.argtypes = []
StopRPC.restype = c_bool

def exception_hook(exc_type, exc_value, exc_traceback):
	print "--- Caught Exception ---"
	lines = traceback.format_exception(exc_type, exc_value, exc_traceback)
	print ''.join('!! ' + line for line in lines)
	print "------------------------"

class CSharpPlayer(xbmc.Player):
	def __init__(self, *args, **kwargs):
	    super(xbmc.Player, self).__init__()
	def onPlayBackEnded(self, *args, **kwargs):
	    return PostEvent(json.dumps({
			"source": "Player",
			"sender": "onPlayBackEnded",
	        "args" : args,
		    "kwargs" : kwargs
	   }))
	def onPlayBackPaused(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
            "sender": "onPlayBackPaused",
            "args" : args,
            "kwargs" : kwargs
        }))
	def onPlayBackResumed(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
			"sender": "onPlayBackResumed",
			"args" : args,
			"kwargs" : kwargs
		}))
	def onPlayBackSeek(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
            "sender": "onPlayBackSeek",
            "args" : args,
            "kwargs" : kwargs
        }))
	def onPlayBackSeekChapter(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
            "sender": "onPlayBackSeekChapter",
            "args" : args,
            "kwargs" : kwargs
        }))
	def onPlayBackSpeedChanged(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
            "sender": "onPlayBackSpeedChanged",
            "args" : args,
            "kwargs" : kwargs
		}))
	def onPlayBackStarted(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
			"sender": "onPlayBackStarted",
			"args" : args,
			"kwargs" : kwargs
		}))
	def	onPlayBackStopped(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
		    "sender": "onPlayBackStopped",
	        "args" : args,
			"kwargs" : kwargs
		}))
	def onQueueNextItem(self, *args, **kwargs):
		return PostEvent(json.dumps({
			"source": "Player",
            "sender": "onQueueNextItem",
            "args" : args,
            "kwargs" : kwargs
		}))


class CSharpMonitor(xbmc.Monitor):
    def __init__(self, *args, **kwargs):
		super(xbmc.Monitor, self).__init__()
    def onAbortRequested(self, *args, **kwargs):
		print "=> event onAbortRequested"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onAbortRequested",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onCleanStarted(self, *args, **kwargs):
		print "=> event onCleanStarted"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onCleanStarted",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onCleanFinished(self, *args, **kwargs):
		print "=> event onCleanFinished"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onCleanFinished",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onDPMSActivated(self, *args, **kwargs):
		print "=> event onDPMSActivated"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onDPMSActivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onDPMSDeactivated(self, *args, **kwargs):
		print "=> event onDPMSDeactivated"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onDPMSDeactivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScreensaverActivated(self, *args, **kwargs):
		print "=> event onScreensaverActivated"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onScreensaverActivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScreensaverDeactivated(self, *args, **kwargs):
		print "=> event onScreensaverDeactivated"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onScreensaverDeactivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScanStarted(self, *args, **kwargs):
		print "=> event onScanStarted"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onScanStarted",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScanFinished(self, *args, **kwargs):
		print "=> event onScanFinished"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onScanFinished",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onSettingsChanged(self, *args, **kwargs):
		print "=> event onSettingsChanged"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onSettingsChanged",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onNotification(self, *args, **kwargs):
		print "=> event onNotification"
		return PostEvent(json.dumps({
			"source": "Monitor",
			"sender": "onNotification",
			"args" : args,
			"kwargs" : kwargs
		}))


def json_error(exitcode):
	return json.dumps({
		"exit_code": exitcode
	})


def on_message(data):
	try:
		message = json.loads(data)
		print message
	except Exception as exc:
		on_exception(exc)
		return json_error(1)

	type = message.get('type')
	if not type:
		print "Invalid Message Type"
		return json_error(1)

	if type == 'exit':
		print "We got exit, bye"
		return json_error(0)
	elif type == 'eval':
		exec_code = message.get('exec_code')
		if exec_code:
			try:
				eval(compile(exec_code, '<string>', 'exec'), globals())
			except Exception as exc:
				on_exception(exc)
				return json_error(1)
	elif type == 'del_var':
		var_name = message.get('var_name')
		if var_name and var_name in Variables:
			del Variables[var_name]

	result = Variables['LastResult']
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
# Start the global event monitors
Variables[csMonitorVar] = CSharpMonitor()
Variables[csPlayerVar] = CSharpPlayer()

# Invoke PluginMain from C#
MessageCallbackFunc = message_callback_type(on_message)

# Second parameter is enableDebug(bool)
Initialize(MessageCallbackFunc, True)

ret = Main()
print "PluginMain returned %d" % ret


# Close handle to DLL
del lib
print "Python RPC Exiting..."
