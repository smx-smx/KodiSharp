using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python.ValueConverters
{
    public static class ObjectConverter
    {
		public static string ToPythonCode(this object obj) {
			return obj.ToString();
		}
    }
}
