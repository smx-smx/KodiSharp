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

		public PlayList(PlayListType type) {
			Instance.CallAssign(
				new PythonFunction(PyModule.Xbmc, "PlayList"),
				new List<object> { type.GetString() },
				EscapeFlags.None
			);
		}

		public int Id {
			get {
				return Convert.ToInt32(Instance.CallFunction(
					PythonFunction.ClassFunction("getPlayListId")
				));
			}
		}

		#region IList
		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public void Clear() {
			Instance.CallFunction(
				PythonFunction.ClassFunction("clear")
			);
		}

		public void Remove(string item) {
			Instance.CallFunction(
				PythonFunction.ClassFunction("remove"), item
			);
		}

		public void Insert(int index, string item) {
			Instance.CallFunction(
				PythonFunction.ClassFunction("add"), item, null, index
			);
		}

		public void Add(string url) {
			Add(url, null);
		}

		public void Add(string url, ListItem listItem = null) {
			Instance.CallFunction(
				PythonFunction.ClassFunction("add"), url, listItem
			);
		}

		public int Count {
			get {
				return Convert.ToInt32(Instance.CallFunction(
					PythonFunction.ClassFunction("size")
				));
			}
		}
		#endregion
	}
}
