using Smx.KodiInterop.Python;
using System;
using System.Collections;
using System.Collections.Generic;
using Smx.KodiInterop.Modules.XbmcGui;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public class PlayList //: IList<string>
    {
		public readonly PyVariable Instance = PyVariableManager.Get.NewVariable();

		private static PythonFunction _ctor = new PythonFunction(PyModule.Xbmc, "PlayList");
		private static PythonFunction _getPlayListId = PythonFunction.ClassFunction("getPlayListId");
		private static PythonFunction _clear = PythonFunction.ClassFunction("clear");
		private static PythonFunction _remove = PythonFunction.ClassFunction("remove");
		private static PythonFunction _add = PythonFunction.ClassFunction("add");
		private static PythonFunction _size = PythonFunction.ClassFunction("size");

		public PlayList(PlayListType type) {
			Instance.CallAssign(
				_ctor,
				new List<object> { type.GetString() },
				EscapeFlags.None
			);
		}

		public int Id {
			get {
				return Convert.ToInt32(Instance.CallFunction(_getPlayListId));
			}
		}

		#region IList
		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public void Clear() {
			Instance.CallFunction(_clear);
		}

		public void Remove(string item) {
			Instance.CallFunction(_remove, item);
		}

		public void Insert(int index, string item) {
			Instance.CallFunction(_add, item, null, index);
		}

		public void Add(string url) {
			Add(url, null);
		}

		public void Add(string url, ListItem listItem = null) {
			Instance.CallFunction(_add, url, listItem);
		}

		public int Count {
			get {
				return Convert.ToInt32(Instance.CallFunction(_size));
			}
		}
		#endregion
	}
}
