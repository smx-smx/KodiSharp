# KodiSharp
Use Kodi python APIs in C#, and write rich addons using the .NET framework
![TestPlugin](https://raw.githubusercontent.com/smx-smx/KodiSharp/master/img/KodiSharp.png)

## Features
- Events support (xbmc.Monitor)
- Python interfaces
    - Code eval
    - Variable management
    - Value escaping
    - Function calls
    - Python console logging
- C# Bindings of Kodi modules (xbmc, xbmcgui, ...)
- URL Routing (handlers for different sections of the plugin)
- Support for Service addons (executed in background, as defined in addon.xml)
- Addons can run in the background, and persist across script invocation.

### Samples
You can load the TestPlugin project for a working sample.

On Windows you can also try the SpeechRecognizerPlugin project, which is an example of speech recognition inside a Kodi Addon. Note that the speech recognition API uses the UWP APIs, and requires that you have enabled speech services. On Windows 10, you can do that from the privacy menu in the modern control panel.

## Debugging
First of all, target kodi.exe as the process we want to debug for your Class Library project
 - Right click on the plugin project
 - Properties
 - Debug
 - Start external program -> Browse for Kodi.exe

Next, to debug the C# code under visual studio, set `enable_debug` to true in `default.py`, at the following line
```python
bridge = Bridge(lib_path, plugin_dll, enable_debug=False)
```

When you run your plugin, the JIT Debugger Window will pop-up.

### Optional: Automatically start and debug the plugin from Visual Studio
Navigate to "%appdata%\Kodi\userdata"
Create a file called "autoexec.py" and insert the following code to start your plugin when kodi starts
```python
import xbmc
xbmc.executebuiltin("RunAddon(plugin.video.test)")
```
Replace "plugin.video.test" with the plugin name you used in addon.xml

This method avoids having to select the debugger from the JIT window every time

### How to use
Create a new C# Class Library, add the `KodiInterop` assembly/project as reference.
Model the plugin class like this

```csharp
using Smx.KodiInterop;
using Smx.KodiInterop.Builtins;

namespace TestPlugin
{
    public class TestPlugin : KodiAddon
    
    [Route("/")]
	public int MainHandler(NameValueCollection parameters){
        UiBuiltins.Notification(
            header: "My Notification",
            message: "Hello World from C#",
            duration: TimeSpan.FromSeconds(1)
        );
    }

    [PluginEntry]
    public static int PluginMain() {
        return GetInstance<TestPlugin>(typeof(TestPlugin), enableDebug: true, persist: true).Run();
    }
}
```

Edit `default.py` to specify the location of the plugin assembly
```python
import sys
from csharp.bridge import Bridge

if "win" in sys.platform:
    lib_path = "path_to_CLRHost.dll"    # for Windows
else:
    lib_path = "path_to_MonoHost.dll"   # for Unix
    
assembly_path = "path_to_TestPlugin.dll"

bridge = Bridge(lib_path, assembly_path, enable_debug=False)
bridge.run()
```

**NOTE**: it's important to use `os.path.join` to construct paths



## TODO
- Implement remaining builtins
- Implement remaining modules functionality (xbmc, xbmcgui, ...)
- Implement a JSON interface (via executeJSONRPC)
