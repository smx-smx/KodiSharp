##
## Copyright (C) 2017 Smx <smxdev4@gmail.com>
##

import os
import sys
from csharp.bridge import Bridge

def getTestPlugin():
	return {
		'config': 'Debug',
		'dir' : os.path.join('KodiInterop', 'TestPlugin', 'bin'),
		'file': 'TestPlugin.dll'
	}

def getSpeechRecognizerPlugin():
	return {
		'config': 'Debug',
		'dir': os.path.join('KodiInterop', 'SpeechRecognizerPlugin', 'bin'),
		'file': 'SpeechRecognizerPlugin.dll'
	}

def getClrHost():
	return {
		'arch': 'x64',
		'config': 'Debug',
		'dir': os.path.join('KodiInterop', 'CLRHost', 'bin'),
		'file': 'CLRHost.dll'
	}

def getNativeLibSuffix():
	if sys.platform.startswith('win'):
		return 'dll'
	
	if 'darwin' in sys.platform:
		return 'dylib'

	return 'so'

def getMonoHost():
	return {
		'dir': os.path.join('KodiInterop', 'Mono', 'build'),
		'file': 'libMonoHost.{}'.format(getNativeLibSuffix())
	}

def getPath(me, obj):
	return os.path.join(me,
		obj.get('dir', ''),
		obj.get('arch', ''),
		obj.get('config', ''),
		obj.get('file', '')
	)


#### Config
mono_on_windows = False
enable_debug = False

me = os.path.abspath(os.path.dirname(__file__))

assembly_path = getPath(me, getTestPlugin())
#assembly_path = getPath(me, getSpeechRecognizerPlugin())

if sys.platform.startswith('win') and not mono_on_windows:
	lib_path = getPath(me, getClrHost())
else:
	lib_path = getPath(me, getMonoHost())
	
bridge = Bridge(lib_path, assembly_path, enable_debug)
bridge.run()