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

mono_on_windows = False
if not "win" in sys.platform or mono_on_windows:
	if sys.platform == "darwin":
		nativeLibSuffix = "dylib"
	elif "win" in sys.platform:
		nativeLibSuffix = "dll"
	else:
		nativeLibSuffix = "so"

	monoLibName = "libMonoHost.{}".format(nativeLibSuffix)
	monohost_path = os.path.join(me, "KodiInterop", "Mono", "build", monoLibName)
	lib_path = monohost_path
else:
	clrhost_path = os.path.join(me, "KodiInterop", "bin", build_arch, build_type, "CLRHost.dll")
	lib_path = clrhost_path



bridge = Bridge(lib_path, assembly_path, enable_debug=False)
bridge.run()