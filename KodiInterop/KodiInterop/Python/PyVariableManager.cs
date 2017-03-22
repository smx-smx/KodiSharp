using System;
using System.Linq;
using System.Collections.Generic;
using Smx.KodiInterop;

namespace Smx.KodiInterop.Python
{
	public static class PyVariableManager {
		public const string LastResultVarName = "LastResult";
		public const string MonitorVarName = "__csharpMonitor";
		public const string PlayerVarName = "__csharpPlayer";


		private static readonly PyVariable _lastResult = new PyVariable(LastResultVarName);
		private static readonly PyVariable _monitor = new PyVariable(MonitorVarName, flags: PyVariableFlags.Object, keepValue: true);
		private static readonly PyVariable _player = new PyVariable(PlayerVarName, flags: PyVariableFlags.Object, keepValue: true);

		private static readonly Dictionary<string, WeakReference<PyVariable>> _variables;

		static PyVariableManager() {
			_variables = new Dictionary<string, WeakReference<PyVariable>>();
		}

		public static PyVariable LastResult {
			get {
				return _lastResult;
			}
		}
		public static PyVariable Monitor {
			get {
				return _monitor;
			}
		}
		public static PyVariable Player {
			get {
				return _player;
			}
		}

		/// <summary>
		/// Allocates a new variable
		/// </summary>
		/// <param name="variableName">Name of the variable to add</param>
		/// <param name="isObject">Indicates the variable will store non-serializable data (like class instances)</param>
		/// <returns>The newly added variable, or null if the variable already exists</returns>
		public static PyVariable NewVariable(string variableName = null, PyVariableFlags flags = PyVariableFlags.Normal) {
			if(variableName == null) {
				variableName = "_var" + _variables.Count;
			}
			if (!_variables.ContainsKey(variableName)) {
				PyVariable variable = new PyVariable(variableName, flags: flags);
				_variables.Add(variableName, new WeakReference<PyVariable>(variable));
				return variable;
			}
			return null;
		}

		public static string GetFreeVariableName() {
			return "_var" + _variables.Count;
		}

		/// <summary>
		/// Deletes the specified python variable
		/// </summary>
		/// <param name="variableName">Name of the variable to delete</param>
		public static void DestroyVariable(string variableName) {
			if (_variables.ContainsKey(variableName)) {
				Messages.PythonDeleteVariableMessage msg = new Messages.PythonDeleteVariableMessage {
					VariableName = variableName
				};
				KodiBridge.SendMessage(msg);
				_variables.Remove(variableName);
			}
		}

		/// <summary>
		/// Copies a Python variable to another by using an assignment (var_dest=var_src)
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="source"></param>
		public static void CopyVariable(PyVariable dest, PyVariable source) {
			PythonInterop.EvalToVar(dest, source.PyName);
		}

		public static void DestroyVariable(PyVariable variable) {
			DestroyVariable(variable.Name);
		}

		public static void DestroyAllVariables() {
			foreach (string name in _variables.Keys) {
				DestroyVariable(name);
			}
		}

		public static void Initialize() {
			_variables.Clear();
		}
	}
}
