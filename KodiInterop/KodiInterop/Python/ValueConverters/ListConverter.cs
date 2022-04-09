using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Python.ValueConverters
{
	public static class ListConverter
	{
		public static string ToPythonCode<T>(this IList<T> list) where T : PythonConvertible {
			StringBuilder sb = new StringBuilder("[");

			int count = 0;

			var iter = list.GetEnumerator();
			while (iter.MoveNext()) {
				++count;
				string value = iter.Current.ToPythonCode();
				sb.Append(value + ",");
			}
			if (count > 0) {
				sb.Length--;
			}
			sb.Append("]");

			return sb.ToString();
		}
	}
}
