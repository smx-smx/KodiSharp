using Smx.KodiInterop;
using System;

namespace Smx.KodiInterop.Python
{
	public class PyFunction {
		public string LeftHand = "";
		public string Function = "";

		/// <summary>
		/// Constructor from common Kodi modules
		/// </summary>
		/// <param name="module"></param>
		/// <param name="functionName"></param>
		public PyFunction(PyModule module, string functionName) {
			this.LeftHand = module.GetString();
			this.Function = functionName;
		}

		/// <summary>
		/// Constructor from generic python module as string
		/// </summary>
		/// <param name="moduleName"></param>
		/// <param name="functionName"></param>
		public PyFunction(string moduleName, string functionName) {
			this.LeftHand = moduleName;
			this.Function = functionName;
		}

		public dynamic Call(params object[] args) {
			return PythonInterop.CallFunction(this, args);
		}

		/// <summary>
		/// Specifies a function inside a class instance
		/// </summary>
		/// <param name="functionName"></param>
		/// <returns></returns>
		public static PyFunction ClassFunction(string functionName) {
			return new PyFunction("", functionName);
		}

		public override string ToString() {
			string result = this.LeftHand;
			if (result.Length > 0)
				result += ".";
			result += this.Function;
			return result;
		}
	}
}
