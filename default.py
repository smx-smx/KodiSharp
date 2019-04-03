##
## Copyright (C) 2017 Smx <smxdev4@gmail.com>
##

import os
import sys
from csharp.bridge import Bridge
from csharp.assembly import Assembly, AssemblyFunc

#### Config
me = os.path.abspath(os.path.dirname(__file__))

plugin_namespace = "TestPlugin"
plugin_main = "Main"
plugin_assembly = "TestPlugin.dll"

build_arch = "x64"
build_type = "Debug"

assembly_path = os.path.join(me, "KodiInterop", "TestPlugin", "bin", build_arch, build_type, plugin_assembly)

if "win" in sys.platform:
	clrhost_path = os.path.join(me, "KodiInterop", "bin", build_arch, build_type, "CLRHost.dll")
	lib_path = clrhost_path
else:
	monohost_path = os.path.join(me, "KodiInterop", "Mono", "build", "libmonoHost.so")
	lib_path = monohost_path


entry = AssemblyFunc(plugin_namespace, plugin_main)
plugin_dll = Assembly(assembly_path, entry)

bridge = Bridge(lib_path, plugin_dll, enable_debug=False)
bridge.run()