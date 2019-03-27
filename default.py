##
## Copyright (C) 2017 Smx <smxdev4@gmail.com>
##

import os
import sys
from csharp.bridge import Bridge
from csharp.assembly import Assembly, AssemblyFunc

#### Config
me = os.path.abspath(os.path.dirname(__file__))
assembly_path = os.path.join(me, "KodiInterop/TestPlugin/bin/x64/Debug", "TestPlugin.dll")
monohost_path = os.path.join(me, "KodiInterop/Mono/build", "libmonoHost.so")
plugin_namespace = "TestPlugin"
plugin_main = "Main"

if "win" in sys.platform:
	lib_path = assembly_path
else:
	lib_path = monohost_path


entry = AssemblyFunc(plugin_namespace, plugin_main)
plugin_dll = Assembly(assembly_path, entry)

bridge = Bridge(lib_path, plugin_dll, enable_debug=True)
bridge.run()