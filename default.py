##
## Copyright (C) 2025 Smx <smxdev4@gmail.com>
##

import os
import sys
from csharp.bridge import Bridge
import platform

me = os.path.abspath(os.path.dirname(__file__))
build_dir = os.path.join(me, 'build', 'out')

def getKodiInterop():
	return {
		'dir' : os.path.join(build_dir, 'bin', 'KodiInterop'),
		'file': 'KodiInterop.dll',
	}

def getTestPlugin():
	return {
		'dir' : os.path.join(build_dir, 'bin', 'TestPlugin'),
		'file': 'TestPlugin.dll'
	}

def getSpeechRecognizerPlugin():
	return {
		'dir': os.path.join(build_dir, 'bin', 'SpeechRecognizerPlugin'),
		'file': 'SpeechRecognizerPlugin.dll'
	}

def getClrHost():
	return {
		'dir': build_dir,
		'file': 'CLRHost.{}'.format(getNativeLibSuffix())
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
	publish = 'publish' if len(obj.get('rid', '')) > 0 else ''
	return os.path.join(me,
		obj.get('dir', ''),
		obj.get('arch', ''),
		# Debug
		obj.get('config', ''),
		# netstandard2.0
		obj.get('fwk', ''),
		obj.get('rid', ''),
		publish,
		obj.get('file', '')
	)


#### Config
use_mono = False
enable_debug = False

assembly_path = getPath(me, getTestPlugin())
#assembly_path = getPath(me, getSpeechRecognizerPlugin())

if use_mono:
	lib_path = os.path.join(build_dir, 'lib', 'libMonoHost.{}'.format(getNativeLibSuffix()))
else:
	lib_path = os.path.join(build_dir, 'lib', 'libcoreclrhost.{}'.format(getNativeLibSuffix()))

interop_path = getPath(me, getKodiInterop())

bridge = Bridge(lib_path, interop_path, assembly_path, enable_debug)
bridge.run()
