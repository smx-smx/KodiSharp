using Newtonsoft.Json.Linq;
using Smx.KodiInterop.Python.ValueConverters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Python
{
    public class PyDict : IDictionary<string, string>
    {
        private readonly PyVariable Instance;

		public const string EmptyDict = "{}";

		public PyVariable GetInstance()
        {
            return Instance;
        }

        public PyDict(PyVariable var)
        {
            Instance = var;
            Instance.EvalAssign(EmptyDict);
        }

		
		public string this[string key]
        {
            get
            {
                return Instance.CallFunction("get", new List<object>(){
                    key
                });
            }

            set
            {
                string pyVal = PythonInterop.EscapeArgument(value);
                GetVariable(key).EvalAssign(pyVal);
            }
        }

        public PyVariable GetVariable(string name)
        {
            string pyName = PythonInterop.EscapeArgument(name);
            return new PyVariable(
				evalCode: $"{Instance.PyName}[{pyName}]",
				basename: name
			);
        }

        public ICollection<string> Keys { get; private set; } = new List<string>();
        public void Refresh()
        {
            JArray result = Instance.CallFunction("keys");
            Keys = result.ToObject<List<string>>();
        }

        public ICollection<string> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                return Keys.Count;
            }
        }

		public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(string key, string evalCode)
        {
            Keys.Add(key);
            if(evalCode != null && evalCode.Length > 0)
                GetVariable(key).EvalAssign(evalCode);
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Instance.CallFunction("clear");
            Refresh();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return Keys.Contains(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

		public bool Remove(string key)
        {
			key = PythonInterop.EscapeArgument(key, EscapeFlags.Quotes);
            PythonInterop.Eval($"del {Instance.PyName}[{key}]");
            Keys.Remove(key);
            return !ContainsKey(key);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, out string value)
        {
            if (!ContainsKey(key)) {
                value = null;
                return false;
            }

            value = this[key];
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

		bool IDictionary<string, string>.Remove(string key) {
			return Remove(key);
		}

		bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) {
			return Remove(item);
		}
	}
}
