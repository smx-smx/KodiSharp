using Smx.KodiInterop;

namespace Smx.KodiInterop.Python
{
	public class PythonFunction
    {
		public string Module = "";
		public string Function = "";

		/// <summary>
		/// Constructor from common Kodi modules
		/// </summary>
		/// <param name="module"></param>
		/// <param name="functionName"></param>
		public PythonFunction(PyModule module, string functionName) {
			this.Module = module.GetString();
			this.Function = functionName;
		}

		/// <summary>
		/// Constructor from generic python module as string
		/// </summary>
		/// <param name="moduleName"></param>
		/// <param name="functionName"></param>
		public PythonFunction(string moduleName, string functionName) {
			this.Module = moduleName;
			this.Function = functionName;
		}

		/// <summary>
		/// Constructor for function call on class instance
		/// </summary>
		/// <param name="functionName"></param>
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
