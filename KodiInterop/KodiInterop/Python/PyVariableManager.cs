using System;
using System.Linq;
using System.Collections.Generic;
using Smx.KodiInterop;

namespace Smx.KodiInterop.Python
{
	public class PyVariableManager {
		public static PyVariableManager Get {
			get {
				return KodiBridge.EnsureRunningAddon().PyVariableManager;
			}
		}

		private const string LastResultVarName = "LastResult";

        private PyDict variables = new PyDict(new PyVariable("self.Variables"));
		public static PyVariable LastResult {
			get => new PyVariable($"self.{LastResultVarName}");
		}

		public PyVariableManager()
		{
			variables.Refresh();
		}

		/// <summary>
		/// Invoked when Python re-runs the script, to wipe variables from the previous script
		/// </summary>
		public void Reset()
		{
			variables.Clear();
		}

		/// <summary>
		/// Allocates a new variable
		/// </summary>
		/// <param name="variableName">Name of the variable to add</param>
		/// <param name="isObject">Indicates the variable will store non-serializable data (like class instances)</param>
		/// <returns>The newly added variable</returns>
		public PyVariable NewVariable(string? variableName = null, string? evalCode = null) {
			if(variableName == null) {
				variableName = "_var" + variables.Count;
			}
			if (!variables.ContainsKey(variableName)) {
                variables.Add(variableName, evalCode);
				return variables.GetVariable(variableName);
			}
			throw new InvalidOperationException($"Variable {variableName} already exists");
		}

		public string GetFreeVariableName() {
			return "_var" + variables.Count;
		}

		/// <summary>
		/// Copies a Python variable to another by using an assignment (var_dest=var_src)
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="source"></param>
		public void CopyVariable(PyVariable dest, PyVariable source) {
			PythonInterop.EvalToVar(dest, source.PyName);
		}

		public void DeleteVariable(PyVariable pyVariable)
		{
			if(pyVariable.Basename == null){
				throw new InvalidOperationException("Variable cannot be removed because it is unnamed");
			}
			//pyVariable has absolute path here, dict['var']
			variables.Remove(pyVariable.Basename);
		}
	}
}
