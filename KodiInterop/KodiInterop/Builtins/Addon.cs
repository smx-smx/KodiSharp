using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
	public static class AddonBuiltins
    {
		/// <summary>
		/// Will install the addon with the given id.
		/// </summary>
		/// <param name="id"></param>
		public static void InstallAddon(int id) {
			PythonInterop.CallBuiltin("InstallAddon", new List<object> { id });
		}

		/// <summary>
		/// Runs the specified plugin/script
		/// </summary>
		/// <param name="id"></param>
		public static void RunAddon(int id) {
			PythonInterop.CallBuiltin("RunAddon", new List<object> { id });
		}

		/// <summary>
		/// Open a settings dialog for the addon of the given id
		/// </summary>
		/// <param name="id"></param>
		public static void OpenSettings(int id) {
			PythonInterop.CallBuiltin("Addon.OpenSettings", new List<object> { id });
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
	}
}
