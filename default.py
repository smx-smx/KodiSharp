##
## Copyright (C) 2025 Smx <smxdev4@gmail.com>
##

import os
import sys
from csharp.bridge import Bridge
import platform

def getKodiInterop():
	return {
		'config': 'Debug',
		'fwk': 'net8.0',
		'dir' : os.path.join('KodiInterop', 'KodiInterop', 'bin'),
		'file': 'KodiInterop.dll',
		'rid': getRid(),
	}

def getTestPlugin():
	return {
		'config': 'Debug',
		'fwk': 'netstandard2.0',
		'dir' : os.path.join('KodiInterop', 'TestPlugin', 'bin'),
		'file': 'TestPlugin.dll',
		'rid': getRid()
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

def getRid():
	# FIXME: validate on windows/macOS/etc.
	arch = platform.machine()
	if arch == 'x86_64':
		arch = 'x64'
	else:
		arch = 'x86'

	system = platform.system().lower()
	if system.startswith('win'):
		system = 'win'
	elif system == 'darwin':
		system = 'osx'

	return f'{system}-{arch}'

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

me = os.path.abspath(os.path.dirname(__file__))

assembly_path = getPath(me, getTestPlugin())
#assembly_path = getPath(me, getSpeechRecognizerPlugin())

ezdotnet_bdir = os.path.join(me, 'EzDotnet', 'build')
if use_mono:
	lib_path = os.path.join(ezdotnet_bdir, 'Mono', 'libMonoHost.{}'.format(getNativeLibSuffix()))
else:
	lib_path = os.path.join(ezdotnet_bdir, 'CoreCLR', 'libcoreclrhost.{}'.format(getNativeLibSuffix()))	

interop_path = getPath(me, getKodiInterop())

bridge = Bridge(lib_path, interop_path, assembly_path, enable_debug)
bridge.run()
