using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using System.Collections.Generic;

namespace XbmcGui
{
	public static class List
    {
		public static void Add(ListItem item) {
			PythonInterop.CallFunction(
				new PythonFunction(PyModule.XbmcPlugin, "addDirectoryItem"),
				new List<object> {
					KodiBridge.RunningAddon.Handle,
					item.Url,
					item.Instance,
					item.IsFolder
				}
			);
		}

		public static void Add(IList<ListItem> items) {
			var list = PyVariableManager.NewVariable();
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

			PyVariable listVar = PyVariableManager.NewVariable(isObject: true);
			listVar.Value = listCode;

			PythonInterop.CallFunction(
				new PythonFunction(PyModule.XbmcPlugin, "addDirectoryItems"),
				new List<object> {
					KodiBridge.RunningAddon.Handle,
					listVar,
					items.Count
				}
			);
		}

		public static void Show(bool succeded = true, bool updateListing = false, bool cacheToDisc = true) {
			PythonInterop.CallFunction(
				new PythonFunction(PyModule.XbmcPlugin, "endOfDirectory"),
				new List<object> {
					KodiBridge.RunningAddon.Handle,
					succeded, updateListing, cacheToDisc
				}
			);
		}
	}
}
