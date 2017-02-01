using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.XbmcGui
{
    public class ListItem : IDisposable
    {
		public readonly PyVariable Instance = PyVariableManager.NewVariable(isObject: true);
		public string Url { get; private set; }
		public bool IsFolder { get; private set; }

		public ListItem(
			string label = null,
			string label2 = null,
			string iconImage = null,
			string thumbnailImage = null,
			string path = null,
			string url = "",
			bool isFolder = false
		) {
			this.Url = url;
			this.IsFolder = IsFolder;
			Instance.CallFunction(
				new PythonFunction(PyModules.XbmcGui, "ListItem"),
				new List<object> { label, label2, iconImage, thumbnailImage, path }
			);
		}

		public string getLabel() {
			return Instance.CallFunction(
				new PythonFunction("getLabel")
			);
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
