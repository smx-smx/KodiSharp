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

		private static PyFunction _ctor = new PyFunction(PyModule.Xbmc, "PlayList");
		private static PyFunction _getPlayListId = PyFunction.ClassFunction("getPlayListId");
		private static PyFunction _clear = PyFunction.ClassFunction("clear");
		private static PyFunction _remove = PyFunction.ClassFunction("remove");
		private static PyFunction _add = PyFunction.ClassFunction("add");
		private static PyFunction _size = PyFunction.ClassFunction("size");

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
