using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using System.Collections.Generic;

namespace Smx.KodiInterop.Modules.XbmcGui
{
	public static class List
    {
		public static void Add(ListItem item) {
			PythonInterop.CallFunction(
				new PyFunction(PyModule.XbmcPlugin, "addDirectoryItem"),
				new List<object> {
					KodiBridge.RunningAddon.Handle,
					item.Url,
					item.Instance,
					item.IsFolder
				}
			);
		}

		public static void Add(IList<ListItem> items) {
			var list = PyVariableManager.Get.NewVariable();
			string listCode = "[";
			for (int i = 0; i < items.Count; i++) {
				listCode += string.Format("({0},{1},{2})",
					PythonInterop.EscapeArgument(items[i].Url),
					items[i].Instance.PyName,
					items[i].IsFolder
				);
				if (i + 1 < items.Count)
					listCode += ",";
			}
			listCode += "]";

			PyVariable listVar = PyVariableManager.Get.NewVariable(evalCode: listCode);

			PythonInterop.CallFunction(
				new PyFunction(PyModule.XbmcPlugin, "addDirectoryItems"),
				new List<object> {
					KodiBridge.RunningAddon.Handle,
					listVar,
					items.Count
				}
			);
		}

		public static void Show(bool succeded = true, bool updateListing = false, bool cacheToDisc = true) {
			PythonInterop.CallFunction(
				new PyFunction(PyModule.XbmcPlugin, "endOfDirectory"),
				new List<object> {
					KodiBridge.RunningAddon.Handle,
					succeded, updateListing, cacheToDisc
				}
			);
		}
	}
}
