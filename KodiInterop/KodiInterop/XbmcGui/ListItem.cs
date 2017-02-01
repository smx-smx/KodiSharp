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

		/// <summary>
		///
		/// </summary>
		/// <param name="label">Item's left label</param>
		/// <param name="label2">Item's right label</param>
		/// <param name="iconImage">Deprecated. Use setArt</param>
		/// <param name="thumbnailImage">Deprecated. Use setArt</param>
		/// <param name="path">path, activated when item is clicked.</param>
		/// <param name="url">The destination url</param>
		/// <param name="isFolder"></param>
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
