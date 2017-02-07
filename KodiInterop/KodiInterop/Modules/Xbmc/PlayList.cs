using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Modules.XbmcGui;

namespace Modules.Xbmc
{
    public class PlayList : IList<string>
    {
		public readonly PyVariable Instance = PyVariableManager.NewVariable(isObject: true);

		public PlayList(PlayListType type) {
			Instance.CallAssign(
				new PythonFunction(PyModule.Xbmc, "PlayList"),
				new List<object> { type.GetString() }
			);
		}

		public int Id {
			get {
				return int.Parse(Instance.CallFunction(
					new PythonFunction("getPlayListId")
				));
			}
		}

		#region IList
		public string this[int index] {
			get {
				throw new NotImplementedException();
			}

			set {
				throw new NotImplementedException();
			}
		}


		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public void Clear() {
			Instance.CallFunction(
				new PythonFunction("clear")
			);
		}

		public void Remove(string item) {
			Instance.CallFunction(
				new PythonFunction("remove"), new List<object> { item }
			);
		}

		public int IndexOf(string item) {
			throw new NotImplementedException();
		}

		public void Insert(int index, string item) {
			Instance.CallFunction(
				new PythonFunction("add"), new List<object> {
					item, null, index
				}
			);
		}

		public void RemoveAt(int index) {
			throw new NotImplementedException();
		}

		public void Add(string url) {
			Add(url, null);
		}

		public void Add(string url, ListItem listItem = null) {
			Instance.CallFunction(
				new PythonFunction("add"), new List<object> {
					url, listItem
				}
			);
		}

		public bool Contains(string item) {
			throw new NotImplementedException();
		}

		public void CopyTo(string[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		bool ICollection<string>.Remove(string item) {
			throw new NotImplementedException();
		}

		public IEnumerator<string> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		public int Count {
			get {
				return int.Parse(Instance.CallFunction(
					new PythonFunction("size")
				));
			}
		}
		#endregion
	}
}
