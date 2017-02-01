using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Smx.KodiInterop.Python
{
	public class PyVariable: IDisposable
	{
		/// <summary>
		/// Name of the variable
		/// </summary>
		public string Name { get; private set; }

		public string PyName {
			get {
				return string.Format("Variables['{0}']", this.Name);
			}
		}

		/// <summary>
		/// Value contained by this value as string
		/// </summary>
		public string Value {
			get {
				if (this.IsObject) {
					return "<object>"; //we can't JSON serialize everything
				} else {
					return PythonInterop.GetVariable(this.Name);
				}
			}
			set {
				PythonInterop.EvalToVar(this.Name, value);
			}
		}

		private bool IsObject;

		public string CallFunction(
			PythonFunction function,
			List<object> arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems
		) {
			if(arguments == null) {
				arguments = new List<object>();
			}
			List<object> nArguments;
			if (escapeMethod.HasFlag(EscapeFlags.StripNullItems)) {
				nArguments = new List<object>();
				foreach(object argument in arguments) {
					if (argument != null)
						nArguments.Add(argument);
				}
			} else {
				nArguments = new List<object>(arguments);
			}

			List<string> textArguments = PythonInterop.EscapeArguments(nArguments, escapeMethod);
			PythonInterop.EvalToVar(this.Name, "{0}({1})", new List<object> {
				function.ToString(), string.Join(", ", textArguments)
			}, EscapeFlags.None);
			return this.Value;
		}

		/// <summary>
		/// Represents a Python variable
		/// </summary>
		/// <param name="varName">Name of the variable</param>
		/// <param name="evalCode">Code that will be evaluated as the variable value/content</param>
		/// <param name="isObject">Indicates the variable will store non-serializable data</param>
		public PyVariable(string varName, string evalCode = null, bool isObject = false) {
			this.Name = varName;
			this.IsObject = isObject;
			if (evalCode == null)
				evalCode = "''";
			this.Value = evalCode;
		}

		public void Dispose() {
			PyVariableManager.DestroyVariable(this.Name);
		}

		public override string ToString() {
			return this.Value;
		}
	}
}