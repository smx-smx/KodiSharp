# KodiSharp
Use Kodi python APIs in C#, and write rich addons using the .NET framework

## How to use
- Download this repo as zip or clone the repo and zip it.

  Make sure the zip contains a top level folder in the format plugin.[type].[name], example plugin.video.test
- Install this ZIP with kodi
- Go to the kodi addons folder and open the solution (TestPlugin.sln).

  To make new plugins you can copy the solution and rename the TestPlugin project to fit your needs.
  
  Make sure you edit addon.xml aswell

## NOTES
- The project must target either x86 or x64 for UnmanagedExports to generate the proper code (**NOT AnyCpu**). If you use the wrong architecture type you may have issues like "[your plugin] is not a valid win 32 application". On windows, kodi builds are generally x86.
- If you want to make a new project from scratch, make sure to:
  - Clone this repo to your new addon solution
  - Add the KodiInterop shared project to the solution
  - Add a reference to the KodiInterop project to your Addon project
  - Install the nuget packages "Newtonsoft.Json" and "UnmanagedExports"
  - Copy default.py and addon.xml to your new addon folder, then add them to your solution as link. Edit them accordingly to change the DLL path and the addon name/author

## TODO
- Mono support (involves getting rid of UnmanagedExports)
- Implement remaining builtins
- Implement remaining modules functionality (xbmc, xbmcgui, ...)
- Implement a JSON interface (via executeJSONRPC)
