using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python.ValueConverters
{
    public static class PyVariableConverter
    {
		public static string ToPythonCode(this PyVariable var) {
			return var.PyName;
		}
    }
}
