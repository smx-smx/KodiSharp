using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop
{
    public class PythonFunction
    {
		public string Module = "";
		public string Function = "";

		public PythonFunction(string moduleName, string functionName) {
			this.Module = moduleName;
			this.Function = functionName;
		}

		public PythonFunction(string functionName) : this("", functionName) { }

		public override string ToString() {
			string result = this.Module;
			if (result.Length > 0)
				result += ".";
			result += this.Function;
			return result;
		}
	}
}
