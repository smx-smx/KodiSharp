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

Variables = {
	"LastResult": ""
}

me = os.path.abspath(os.path.dirname(__file__))
lib = cdll.LoadLibrary(os.path.join(me, "KodiInterop/TestPlugin/bin/x86/Debug", "TestPlugin.dll")) # Hardcoding this for now

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

# Plugin init
Initialize = lib.Initialize
Initialize.argtypes = []
Initialize.restype = c_bool

# Plugin entrypoint
Main = lib.PluginMain
Main.argtypes = []
Main.restype = c_int

# Causes C# to send the quit signal to this script
StopRPC = lib.StopRPC
StopRPC.argtypes = []
StopRPC.restype = c_bool

def on_exception(exc):
	print "--- Caught Exception ---"
	exc_type, exc_value, exc_traceback = sys.exc_info()
	lines = traceback.format_exception(exc_type, exc_value, exc_traceback)
	print ''.join('!! ' + line for line in lines)
	print "------------------------"
	xbmc.executebuiltin('Notification(KodiSharp, An error occured. Please check the log for more information')

def exception_hook(exctype, value, traceback):
	on_exception(value)

class CSharpMonitor(xbmc.Monitor):
    def __init__(self, *args, **kwargs):
		super(xbmc.Monitor, self).__init__()
    def onAbortRequested(self, *args, **kwargs):
		print "=> event onAbortRequested"
		return PostEvent(json.dumps({
			"sender": "onAbortRequested",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onCleanStarted(self, *args, **kwargs):
		print "=> event onCleanStarted"
		return PostEvent(json.dumps({
			"sender": "onCleanStarted",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onCleanFinished(self, *args, **kwargs):
		print "=> event onCleanFinished"
		return PostEvent(json.dumps({
			"sender": "onCleanFinished",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onDPMSActivated(self, *args, **kwargs):
		print "=> event onDPMSActivated"
		return PostEvent(json.dumps({
			"sender": "onDPMSActivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onDPMSDeactivated(self, *args, **kwargs):
		print "=> event onDPMSDeactivated"
		return PostEvent(json.dumps({
			"sender": "onDPMSDeactivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScreensaverActivated(self, *args, **kwargs):
		print "=> event onScreensaverActivated"
		return PostEvent(json.dumps({
			"sender": "onScreensaverActivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScreensaverDeactivated(self, *args, **kwargs):
		print "=> event onScreensaverDeactivated"
		return PostEvent(json.dumps({
			"sender": "onScreensaverDeactivated",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScanStarted(self, *args, **kwargs):
		print "=> event onScanStarted"
		return PostEvent(json.dumps({
			"sender": "onScanStarted",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onScanFinished(self, *args, **kwargs):
		print "=> event onScanFinished"
		return PostEvent(json.dumps({
			"sender": "onScanFinished",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onSettingsChanged(self, *args, **kwargs):
		print "=> event onSettingsChanged"
		return PostEvent(json.dumps({
			"sender": "onSettingsChanged",
			"args" : args,
			"kwargs" : kwargs
		}))
    def onNotification(self, *args, **kwargs):
		print "=> event onNotification"
		return PostEvent(json.dumps({
			"sender": "onNotification",
			"args" : args,
			"kwargs" : kwargs
		}))

# Thread to handle incoming C# requests
def message_receiver():
	t = threading.currentThread()
	while getattr(t, "do_run", True):
		exitcode = 0
		# Will be blocking in C# context
		try:
			message = json.loads( unicode(GetMessage(), 'latin-1') )
			print message
		except Exception as exc:
			on_exception(exc)
			exitcode = 1
			t.do_run = False

		type = message.get('type')
		if type:
			if type == 'exit':
				print "We got exit, bye"
				# Stop thread at next loop
				t.do_run = False
			elif type == 'eval':
				exec_code = message.get('exec_code')
				if exec_code:
					try:
						eval(compile(exec_code, '<string>', 'exec'))
					except Exception as exc:
						on_exception(exc)
						exitcode = 1
						t.do_run = False
			elif type == 'del_var':
				var_name = message.get('var_name')
				if var_name and var_name in Variables:
					del Variables[var_name]



		sendRet = PostMessage(json.dumps({
			"result" : Variables['LastResult'],
			"exit_code": exitcode
		}))


################################################

sys.excepthook = exception_hook
# Start the event monitor
print "Starting Event Receiver..."
monitor = CSharpMonitor()

messages_thread = threading.Thread(target = message_receiver)
messages_thread.start()

# Invoke PluginMain from C#
Initialize()
ret = Main()
print "PluginMain returned %d" % ret

monitor.waitForAbort()
# Ask C# to stop RPC. This will cause the message_receiver thread to stop
StopRPC()
messages_thread.join()

# Close handle to DLL
del lib
print "Python RPC Exiting..."
