using System;
using System.Collections.Generic;

namespace Smx.KodiInterop.Builtins
{
	public static class AddonDefaultBuiltins
	{
		/// <summary>
		/// Open a settings dialog for the default addon of the given type (extensionpoint) 
		/// </summary>
		/// <param name="extensionPoint">The add-on type</param>
		public static void OpenSettings(string extensionPoint) {
			PythonInterop.CallBuiltin("Addon.Default.OpenSettings", extensionPoint);
		}

		/// <summary>
		/// Open a select dialog to allow choosing the default addon of the given type (extensionpoint) 
		/// </summary>
		/// <param name="extensionPoint">The add-on type</param>
		public static void Set(string extensionPoint) {
			PythonInterop.CallBuiltin("Addon.Default.Set", extensionPoint);
		}
	}

	public static class AddonBuiltins
    {
		/// <summary>
		/// Will install the addon with the given id.
		/// </summary>
		/// <param name="id"></param>
		public static void InstallAddon(int id) {
			PythonInterop.CallBuiltin("InstallAddon", id);
		}

		/// <summary>
		/// Runs the specified plugin/script
		/// </summary>
		/// <param name="id"></param>
		public static void RunAddon(int id) {
			PythonInterop.CallBuiltin("RunAddon", id);
		}

		/// <summary>
		/// Open a settings dialog for the addon of the given id
		/// </summary>
		/// <param name="id"></param>
		public static void OpenSettings(int id) {
			PythonInterop.CallBuiltin("Addon.OpenSettings", id);
		}

		/// <summary>
		/// Triggers a scan of local add-on directories.
		/// </summary>
		public static void UpdateLocalAddons() {
			PythonInterop.CallBuiltin("UpdateLocalAddons");
		}

		/// <summary>
		/// Triggers a forced update of enabled add-on repositories.
		/// </summary>
		public static void UpdateAddonRepos() {
			PythonInterop.CallBuiltin("UpdateAddonRepos");
		}

		/// <summary>
		/// Run the specified AppleScript command 
		/// </summary>
		/// <param name="scriptUri">the URL to the apple script</param>
		public static void RunAppleScript(string scriptUri) {
			PythonInterop.CallBuiltin("RunAppleScript", scriptUri);
		}

		/// <summary>
		/// Runs the python script.
		/// You must specify the full path to the script.
		/// As of 2007/02/24, all extra parameters are passed to the script as arguments and can be accessed by python using sys.argv 
		/// </summary>
		/// <param name="script"> the URL to the python script. </param>
		/// <param name="args"></param>
		public static void RunScript(string script, params object[] args) {
			object[] builtinArgs = new object[args.Length + 1];
			builtinArgs[0] = script;
			Array.Copy(args, 0, builtinArgs, 1, args.Length);

			PythonInterop.CallBuiltin("RunScript", script, args);
		}

		/// <summary>
		/// Executes the specified script given its addon-id
		/// </summary>
		/// <param name="addonId">the addon-ID to the script add-on</param>
		/// <param name="args"></param>
		public static void RunScript(int addonId, params object[] args) {
			object[] builtinArgs = new object[args.Length + 1];
			builtinArgs[0] = addonId;
			Array.Copy(args, 0, builtinArgs, 1, args.Length);

			PythonInterop.CallBuiltin("RunScript", addonId, args);
		}

		/// <summary>
		/// Stop the script by ID, if running 
		/// </summary>
		/// <param name="addonId">The add-on ID of the script to stop</param>
		public static void StopScript(int addonId) {
			PythonInterop.CallBuiltin("StopScript", addonId);
		}

		/// <summary>
		/// Stop the script by path, if running 
		/// </summary>
		/// <param name="scriptUrl">The URL of the script to stop. </param>
		public static void StopScript(string scriptUrl) {
			PythonInterop.CallBuiltin("StopScript", scriptUrl);
		}

		/// <summary>
		/// Runs the plugin. Full path must be specified. Does not work for folder plugins 
		/// </summary>
		/// <param name="pluginUrl">plugin:// URL to script. </param>
		public static void RunPlugin(string pluginUrl) {
			PythonInterop.CallBuiltin("RunPlugin", pluginUrl);
		}
	}
}
