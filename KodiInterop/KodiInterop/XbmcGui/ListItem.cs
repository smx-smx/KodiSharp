using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;

namespace XbmcGui
{
	public class ListItem : IDisposable
    {
		public readonly PyVariable Instance = PyVariableManager.NewVariable(isObject: true);
		public string Url { get; private set; }
		public bool IsFolder { get; private set; }

		public bool Selected {
			get {
				return bool.Parse(Instance.CallFunction(
					new PythonFunction("isSelected")
				));
			}
			set {
				Instance.CallFunction(
					new PythonFunction("select"), new List<object> { value }
				);
			}
		}

		public string Label {
			get {
				return Instance.CallFunction(
					new PythonFunction("getLabel")
				);
			}
			set {
				Instance.CallFunction(
					new PythonFunction("setLabel"), new List<object> { value }
				);
			}
		}

		public string Label2 {
			get {
				return Instance.CallFunction(
					new PythonFunction("getLabel2")
				);
			}
			set {
				Instance.CallFunction(
					new PythonFunction("setLabel2"), new List<object> { value }
				);
			}
		}

		public string Path {
			get {
				return this.GetProperty("path");
			}
			set {
				Instance.CallFunction(
					new PythonFunction("setPath"), new List<object> { value }
				);
			}
		}

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
			this.IsFolder = isFolder;
			Instance.CallAssign(
				new PythonFunction(PyModule.XbmcGui, "ListItem"),
				new List<object> { label, label2, iconImage, thumbnailImage, path }
			);
		}

		public string GetProperty(string key) {
			return Instance.CallFunction(
				new PythonFunction("getProperty"), new List<object> { key }
			);
		}

		public void SetProperty(string key, object value) {
			Instance.CallFunction(
				new PythonFunction("setProperty"), new List<object> { key, value }
			);
		}

		public string GetArt(string key) {
			return Instance.CallFunction(
				new PythonFunction("getArt"), new List<object> { key }
			);
		}

		public void SetArt(string key, object value) {
			Instance.CallFunction(
				new PythonFunction("setArt"), new List<object> { key, value }
			);
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
