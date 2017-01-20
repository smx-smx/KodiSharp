using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
	public class AddonBuiltins
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
	}
}
