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

		private static PythonFunction _ctor = new PythonFunction(PyModule.XbmcGui, "ListItem");
		private static PythonFunction _getDuration = PythonFunction.ClassFunction("getduration");
		private static PythonFunction _getFilename = PythonFunction.ClassFunction("getfilename");
		private static PythonFunction _isSelected = PythonFunction.ClassFunction("isSelected");
		private static PythonFunction _select = PythonFunction.ClassFunction("select");
		private static PythonFunction _getLabel = PythonFunction.ClassFunction("getLabel");
		private static PythonFunction _setLabel = PythonFunction.ClassFunction("setLabel");
		private static PythonFunction _getLabel2 = PythonFunction.ClassFunction("getLabel2");
		private static PythonFunction _setLabel2 = PythonFunction.ClassFunction("setLabel2");
		private static PythonFunction _setPath = PythonFunction.ClassFunction("setPath");
		private static PythonFunction _getProperty = PythonFunction.ClassFunction("getProperty");
		private static PythonFunction _setProperty = PythonFunction.ClassFunction("setProperty");
		private static PythonFunction _getArt = PythonFunction.ClassFunction("getArt");
		private static PythonFunction _setArt = PythonFunction.ClassFunction("setArt");

		[Obsolete]
		public TimeSpan Duration {
			get {
				double duration = Convert.ToDouble(Instance.CallFunction(_getDuration));
				return TimeSpan.FromSeconds(duration);
			}
		}

		[Obsolete]
		public string Filename {
			get {
				return Instance.CallFunction(_getFilename);
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
				return Convert.ToBoolean(Instance.CallFunction(_isSelected));
			}
			set {
				Instance.CallFunction(_select, value);
			}
		}

		public string Label {
			get {
				return Instance.CallFunction(_getLabel);
			}
			set {
				Instance.CallFunction(_setLabel, value);
			}
		}

		public string Label2 {
			get {
				return Instance.CallFunction(_getLabel2);
			}
			set {
				Instance.CallFunction(_setLabel2, value);
			}
		}

		public string Path {
			get {
				return GetProperty("path");
			}
			set {
				Instance.CallFunction(_setPath, value);
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
				_ctor,
				label, label2, iconImage, thumbnailImage, path
			);

			if(art != null)
			{
				SetArt(art);
			}
		}

		public string GetProperty(string key) {
			return Instance.CallFunction(_getProperty, key);
		}

		public void SetProperty(string key, object value) {
			Instance.CallFunction(_setProperty, key, value);
		}

		public string GetArt(string key) {
			return Instance.CallFunction(_getArt, key);
		}

		public void SetArt(Dictionary<Art, string> art) {
			Instance.CallFunction(_setArt, art.ToPythonCode());
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
