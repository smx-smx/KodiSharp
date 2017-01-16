##
## Copyright (C) 2017 Smx <smxdev4@gmail.com>
##
import os
import json
import threading
from ctypes import *

# Load all XBMC Apis so C# can use them
import xbmc
import xbmcgui
import xbmcplugin
import xbmcaddon
import xbmcvfs


me = os.path.abspath(os.path.dirname(__file__))
lib = cdll.LoadLibrary(os.path.join(me, "KodiInterop/TestPlugin/bin/x86/Debug", "TestPlugin.dll")) # Hardcoding this for now

## C# Functions
# Get the next command in JSON format
GetMessage = lib.GetMessage
GetMessage.argtypes = []
GetMessage.restype = c_char_p

# Send the command result in JSON format
PutMessage = lib.PythonMessage
PutMessage.argtypes = [c_char_p]
PutMessage.restype = c_bool

# Plugin entrypoint
Main = lib.PluginMain
Main.argtypes = []
Main.restype = c_char_p

# Causes C# to send the quit signal to this script
StopRPC = lib.StopRPC
StopRPC.argtypes = []
StopRPC.restype = c_bool

# Thread to handle incoming C# requests
def message_receiver():
	t = threading.currentThread()
	while getattr(t, "do_run", True):
		# Will be blocking in C# context
		try:
			message = GetMessage()
			print "I got the message of type: %s" % type(message)
			print message
			message = json.loads(message)
		except Exception as e:
			print "Something went wrong, err: %s" % e
			break

		if message.get('type'):
			if message['type'] == 'exit':
				print "We got exit, bye"
				# Stop thread at next loop
				t.do_run = False

		code = message.get('code')
		if code:
			eval(code)
		
		sendRet = PutMessage(json.dumps({
			"exitcode": 0
		}))

thread = threading.Thread(target = message_receiver)
thread.start()

# Invoke PluginMain from C#
ret = Main()
print ret

# Ask C# to stop RPC. This will cause the message_receiver thread to stop
StopRPC()
thread.join()

# Close handle to DLL
del lib
