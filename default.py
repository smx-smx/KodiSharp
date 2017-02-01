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
PutMessage = lib.PutMessage
PutMessage.argtypes = [c_char_p]
PutMessage.restype = c_bool

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

sys.excepthook = exception_hook

# Thread to handle incoming C# requests
def message_receiver():
	t = threading.currentThread()
	while getattr(t, "do_run", True):
		exitcode = 0
		# Will be blocking in C# context
		try:
			message = GetMessage()
			message = json.loads(message)
			print message
		except Exception as exc:
			on_exception(exc)
			exitcode = 1
			t.do_run = False

		if message.get('type'):
			if message['type'] == 'exit':
				print "We got exit, bye"
				# Stop thread at next loop
				t.do_run = False

		exec_code = message.get('exec_code')
		if exec_code:
			try:
				eval(compile(exec_code, '<string>', 'exec'))
			except Exception as exc:
				on_exception(exc)
				exitcode = 1
				t.do_run = False


		sendRet = PutMessage(json.dumps({
			"result" : Variables['LastResult'],
			"exit_code": exitcode
		}))

thread = threading.Thread(target = message_receiver)
thread.start()

# Invoke PluginMain from C#
ret = Main()
print "PluginMain returned %d" % ret

# Ask C# to stop RPC. This will cause the message_receiver thread to stop
StopRPC()
thread.join()

# Close handle to DLL
del lib
