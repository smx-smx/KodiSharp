using Smx.KodiInterop.Modules.XbmcAddon;
using Smx.KodiInterop.Python;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Smx.KodiInterop
{
	public class KodiAddonSettings : IDictionary<string, string>
	{
		//public KodiAddon Addon { get; private set; }
		private readonly Addon runningAddon;

		public KodiAddonSettings(int? id = null) {
			this.runningAddon = new Addon(id);
		}

		public string this[string setting] {
			/*
			get {
				return PythonInterop.CallFunction(
					new PythonFunction(PyModule.XbmcPlugin, "getSetting"),
					new List<object> { Addon.Handle, setting }
				);
			}

			set {
				PythonInterop.CallFunction(
					new PythonFunction(PyModule.XbmcPlugin, "setSetting"),
					new List<object> { Addon.Handle, setting, value }
				);
			}
			*/
			get {
				return runningAddon.GetSetting(setting);
			}
			set {
				runningAddon.SetSetting(setting, value);
			}
		}

		public int Count {
			get {
				throw new NotImplementedException();
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public ICollection<string> Keys {
			get {
				throw new NotImplementedException();
			}
		}

		public ICollection<string> Values {
			get {
				throw new NotImplementedException();
			}
		}

		public void Add(KeyValuePair<string, string> item) {
			throw new NotImplementedException();
		}

		public void Add(string key, string value) {
			throw new NotImplementedException();
		}

		public void Clear() {
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<string, string> item) {
			throw new NotImplementedException();
		}

		public bool ContainsKey(string key) {
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<string, string> item) {
			throw new NotImplementedException();
		}

		public bool Remove(string key) {
			throw new NotImplementedException();
		}

		public bool TryGetValue(string key, out string value) {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
	}
}
