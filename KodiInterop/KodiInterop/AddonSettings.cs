using Smx.KodiInterop.Modules.XbmcAddon;
using Smx.KodiInterop.Python;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Smx.KodiInterop
{
	public class KodiAddonSettings : IDictionary<string, string> {
		private Addon addon;

		public KodiAddonSettings(Addon addon) {
			this.addon = addon;
		}

		public string this[string key] {
			// FIXME: settings via XbmcPlugin cause "Invalid Handle" being printed in kodi.log
			// even if the handle seems valid
#if false
			get {
				return PythonInterop.CallFunction(
					new PythonFunction(PyModule.XbmcPlugin, "getSetting"),
					new List<object> { KodiBridge.RunningAddon.Handle, key }
				);
			}

			set {
				PythonInterop.CallFunction(
					new PythonFunction(PyModule.XbmcPlugin, "setSetting"),
					new List<object> { KodiBridge.RunningAddon.Handle, key, value }
				);
			}
#endif

			get {
				return addon.GetSetting(key);
			}
			set {
				addon.SetSetting(key, value);
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
