using Smx.KodiInterop.Modules.Xbmc;
using Smx.KodiInterop.Python;
using Smx.KodiInterop.Python.XbmcGui;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Modules.XbmcGui
{
	public class ListItem : IDisposable
    {
		public readonly PyVariable Instance = PyVariableManager.Get.NewVariable();
		public string Url { get; private set; }
		public bool IsFolder { get; private set; }

		private static PyFunction _ctor = new PyFunction(PyModule.XbmcGui, "ListItem");
		private static PyFunction _getDuration = PyFunction.ClassFunction("getduration");
		private static PyFunction _getFilename = PyFunction.ClassFunction("getfilename");
		private static PyFunction _isSelected = PyFunction.ClassFunction("isSelected");
		private static PyFunction _select = PyFunction.ClassFunction("select");
		private static PyFunction _getLabel = PyFunction.ClassFunction("getLabel");
		private static PyFunction _setLabel = PyFunction.ClassFunction("setLabel");
		private static PyFunction _getLabel2 = PyFunction.ClassFunction("getLabel2");
		private static PyFunction _setLabel2 = PyFunction.ClassFunction("setLabel2");
		private static PyFunction _setPath = PyFunction.ClassFunction("setPath");
		private static PyFunction _getProperty = PyFunction.ClassFunction("getProperty");
		private static PyFunction _setProperty = PyFunction.ClassFunction("setProperty");
		private static PyFunction _getArt = PyFunction.ClassFunction("getArt");
		private static PyFunction _setArt = PyFunction.ClassFunction("setArt");
		private static PyFunction _getVotes = PyFunction.ClassFunction("getVotes");
		private static PyFunction _getMusicInfoTag = PyFunction.ClassFunction("getMusicInfoTag");
		private static PyFunction _getVideoInfoTag = PyFunction.ClassFunction("getVideoInfoTag");

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
				return new InfoTagMusic(
					PyVariableManager.Get
						.NewVariable()
						.CallAssign(_getMusicInfoTag)
				);
			}
		}

		public InfoTagVideo VideoInfoTag {
			get {
				return new InfoTagVideo(
					PyVariableManager.Get
						.NewVariable()
						.CallAssign(_getVideoInfoTag)
				);
			}
		}

		public bool Selected {
			get {
				return Convert.ToBoolean(Instance.CallFunction(_isSelected));
			}
			set {
				Instance.CallFunction(_select, new object[] { value });
			}
		}

		public string Label {
			get {
				return Instance.CallFunction(_getLabel);
			}
			set {
				Instance.CallFunction(_setLabel, new object[] { value });
			}
		}

		public string Label2 {
			get {
				return Instance.CallFunction(_getLabel2);
			}
			set {
				Instance.CallFunction(_setLabel2, new object[] { value });
			}
		}

		public string Path {
			get {
				return GetProperty("path");
			}
			set {
				Instance.CallFunction(_setPath, new object[] { value });
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

		public string GetVotes(Votes voteKey) {
			return Instance.CallFunction(_getVotes, voteKey.GetString());
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
