using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smx.KodiInterop.Python.ValueConverters
{
    public class ArrayConverter : IValueConverter<ICollection<string>>
    {
	    public string ToPythonCode(ICollection<string> array) {
		    StringBuilder sb = new StringBuilder("[");


			var iter = array.GetEnumerator();
		    while (iter.MoveNext()) {
			    string value = PythonInterop.EscapeArgument(iter.Current.ToString());
				sb.Append(value + ",");
			}
		    sb.Length--;
			sb.Append("]");

		    return sb.ToString();
	    }

        public ICollection<string> FromPythonCode(string code)
        {
            return code
                .Replace("[", "")
                .Replace("]", "")
                .Replace(" ", "")
                .Split(',');
        }
	}
}
