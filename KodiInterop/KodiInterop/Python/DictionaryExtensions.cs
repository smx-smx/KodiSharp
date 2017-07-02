using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python
{
    public static class DictionaryExtensions
    {
		public static string ToPythonDict<TKey, TValue>(this IDictionary<TKey, TValue> dict) {
			StringBuilder sb = new StringBuilder("{");
			foreach(TKey key in dict.Keys) {
				sb.Append(string.Format("{0}:{1}",
					PythonInterop.EscapeArgument(key),
					PythonInterop.EscapeArgument(dict[key])
				));
				sb.Append(",");
			}
			sb.Length--; //remove last ','
			sb.Append("}");
			return sb.ToString();
		}
    }
}
