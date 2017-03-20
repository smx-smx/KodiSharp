using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smx.KodiInterop
{
	public class KodiEventArgs : EventArgs, IDictionary<string, string>
	{
		private Dictionary<string, string> _innerDict = new Dictionary<string, string>();

		public string this[string key] {
			get {
				return _innerDict[key];
			}

			set {
				_innerDict[key] = value;
			}
		}

		public int Count {
			get {
				return _innerDict.Count;
			}
		}

		public bool IsReadOnly => false;

		public ICollection<string> Keys {
			get {
				return _innerDict.Keys;
			}
		}

		public ICollection<string> Values {
			get {
				return _innerDict.Values;
			}
		}

		public void Add(KeyValuePair<string, string> item) {
			_innerDict.Add(item.Key, item.Value);
		}

		public void Add(string key, string value) {
			_innerDict.Add(key, value);
		}

		public void Clear() {
			_innerDict.Clear();
		}

		public bool Contains(KeyValuePair<string, string> item) {
			return _innerDict.Contains(item);
		}

		public bool ContainsKey(string key) {
			return _innerDict.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
			var enumerator = _innerDict.GetEnumerator();
			for (int i = arrayIndex; i < array.Length; i++) {
				if (!enumerator.MoveNext())
					throw new InvalidOperationException();
				array[i] = enumerator.Current;
			}
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
			return _innerDict.GetEnumerator();
		}

		public bool Remove(KeyValuePair<string, string> item) {
			return Remove(item.Key);
		}

		public bool Remove(string key) {
			return _innerDict.Remove(key);
		}

		public bool TryGetValue(string key, out string value) {
			return _innerDict.TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
