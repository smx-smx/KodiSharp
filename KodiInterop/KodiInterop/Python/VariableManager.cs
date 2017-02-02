using System;
using System.Collections.Generic;

namespace Smx.KodiInterop.Python
{
	public static class PyVariableManager {
		private const string LastResultVarName = "LastResult";
		private static readonly Dictionary<string, PyVariable> _variables;

		static PyVariableManager() {
			_variables = new Dictionary<string, PyVariable>() {
				{ LastResultVarName, new PyVariable(LastResultVarName) }
			};
		}

		public static PyVariable LastResult {
			get {
				return _variables[LastResultVarName];
			}
		}

		/// <summary>
		/// Allocates a new variable
		/// </summary>
		/// <param name="variableName">Name of the variable to add</param>
		/// <param name="isObject">Indicates the variable will store non-serializable data</param>
		/// <returns>The newly added variable, or null if the variable already exists</returns>
		public static PyVariable NewVariable(string variableName = null, bool isObject = false) {
			if(variableName == null) {
				variableName = "_var" + _variables.Count;
			}
			if (!_variables.ContainsKey(variableName)) {
				PyVariable variable = new PyVariable(variableName, isObject: isObject);
				_variables.Add(variableName, variable);
				return variable;
			}
			return null;
		}

		/// <summary>
		/// Deletes the specified python variable
		/// </summary>
		/// <param name="variableName">Name of the variable to delete</param>
		public static void DestroyVariable(string variableName) {
			if(variableName == LastResultVarName) {
				throw new ArgumentException("Cannot delete reserved '{0}' variable", LastResultVarName);
			}
			if (_variables.ContainsKey(variableName)) {
				PythonInterop.Eval(string.Format("del Variables['{0}']", variableName));
				_variables.Remove(variableName);
			}
		}

		public static void DestroyVariable(PyVariable variable) {
			DestroyVariable(variable.Name);
		}
	}
}
