using Smx.KodiInterop.Python;
using Smx.KodiInterop.XbmcGui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.XbmcPlugin
{
    public class Plugin
    {
		private string module = KodiModules.XbmcPlugin;

		/// <summary>
		/// Callback function to pass directory contents back to Kodi.
		/// </summary>
		/// <param name="handle">handle the plugin was started with</param>
		/// <param name="url">url of the entry. would be plugin:// for another virtual directory</param>
		/// <param name="listitem">item to add</param>
		/// <param name="isFolder">True=folder / False=not a folder(default)</param>
		/// <param name="totalItems">total number of items that will be passed.(used for progressbar)</param>
		/// <returns></returns>
		public bool addDirectoryItem(
			int handle,
			string url,
			ListItem listitem,
			bool isFolder = false,
			int? totalItems = null
		) {
			List<object> arguments = new List<object>() {
				handle, url, listitem, isFolder
			};
			if (totalItems != null)
				arguments.Add(totalItems);

			PythonInterop.CallFunction(
				new PythonFunction(module, "addDirectoryItem"),
				arguments
			);

			return bool.Parse(VariableManager.LastResult.Value);
		}
	}
}
