using Smx.KodiInterop.Modules.Xbmc;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;

namespace Smx.KodiInterop.Modules.XbmcGui
{
	public class ListItem : IDisposable
    {
		public readonly PyVariable Instance = PyVariableManager.Get.NewVariable();
		public string Url { get; private set; }
		public bool IsFolder { get; private set; }

		private PyVariable MusicInfoInstance;
		private PyVariable VideoInfoInstance;

		[Obsolete]
		public TimeSpan Duration {
			get {
				double duration = Convert.ToDouble(Instance.CallFunction(
					PythonFunction.ClassFunction("getduration")
				));
				return TimeSpan.FromSeconds(duration);
			}
		}

		[Obsolete]
		public string Filename {
			get {
				return Instance.CallFunction(
					PythonFunction.ClassFunction("getfilename")
				);
			}
		}

		public InfoTagMusic MusicInfoTag {
			get {
				throw new NotImplementedException();
				/*
				this.MusicInfoInstance = PyVariableManager.NewVariable(isObject: true);
				this.MusicInfoInstance.CallAssign(
					new PythonFunction("getMusicInfoTag")
				);
				*/

			}
		}

		public InfoTagVideo VideoInfoTag {
			get {
				throw new NotImplementedException();
				/*
				this.VideoInfoInstance = PyVariableManager.NewVariable(isObject: true);
				this.VideoInfoInstance = Instance.CallAssign(
					new PythonFunction("getVideoInfoTag")
				);
				*/
			}
		}

		public bool Selected {
			get {
				return Convert.ToBoolean(Instance.CallFunction(
					PythonFunction.ClassFunction("isSelected")
				));
			}
			set {
				Instance.CallFunction(
					PythonFunction.ClassFunction("select"), new List<object> { value }
				);
			}
		}

		public string Label {
			get {
				return Instance.CallFunction(
					PythonFunction.ClassFunction("getLabel")
				);
			}
			set {
				Instance.CallFunction(
					PythonFunction.ClassFunction("setLabel"), new List<object> { value }
				);
			}
		}

		public string Label2 {
			get {
				return Instance.CallFunction(
					PythonFunction.ClassFunction("getLabel2")
				);
			}
			set {
				Instance.CallFunction(
					PythonFunction.ClassFunction("setLabel2"), new List<object> { value }
				);
			}
		}

		public string Path {
			get {
				return this.GetProperty("path");
			}
			set {
				Instance.CallFunction(
					PythonFunction.ClassFunction("setPath"), new List<object> { value }
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
			bool isFolder = false,
			Dictionary<Art, string> art = null
		) {
			this.Url = url;
			this.IsFolder = isFolder;
			Instance.CallAssign(
				new PythonFunction(PyModule.XbmcGui, "ListItem"),
				new List<object> { label, label2, iconImage, thumbnailImage, path }
			);

			if(art != null)
			{
				SetArt(art);
			}
		}

		public string GetProperty(string key) {
			return Instance.CallFunction(
				PythonFunction.ClassFunction("getProperty"), new List<object> { key }
			);
		}

		public void SetProperty(string key, object value) {
			Instance.CallFunction(
				PythonFunction.ClassFunction("setProperty"), new List<object> { key, value }
			);
		}

		public string GetArt(string key) {
			return Instance.CallFunction(
				PythonFunction.ClassFunction("getArt"), new List<object> { key }
			);
		}

		public void SetArt(Dictionary<Art, string> art) {
			Instance.CallFunction(
				PythonFunction.ClassFunction("setArt"), art.ToPythonCode()
			);
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
