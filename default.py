##
## Copyright (C) 2017 Smx <smxdev4@gmail.com>
##

import os
import sys
from csharp.bridge import Bridge

#### Config
me = os.path.abspath(os.path.dirname(__file__))

build_arch = "x64"
build_type = "Debug"

assembly_path = os.path.join(me, "KodiInterop", "TestPlugin", "bin", build_arch, build_type, "TestPlugin.dll")

if "win" in sys.platform:
	clrhost_path = os.path.join(me, "KodiInterop", "bin", build_arch, build_type, "CLRHost.dll")
	lib_path = clrhost_path
else:
	monohost_path = os.path.join(me, "KodiInterop", "Mono", "build", "libmonoHost.so")
	lib_path = monohost_path


bridge = Bridge(lib_path, assembly_path, enable_debug=False)
bridge.run()