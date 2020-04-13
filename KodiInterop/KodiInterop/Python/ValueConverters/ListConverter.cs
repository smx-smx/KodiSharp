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


			var iter = list.GetEnumerator();
			while (iter.MoveNext()) {
				string value = iter.Current.ToPythonCode();
				sb.Append(value + ",");
			}
			sb.Length--;
			sb.Append("]");

			return sb.ToString();
		}
	}
}
