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


	}
}
