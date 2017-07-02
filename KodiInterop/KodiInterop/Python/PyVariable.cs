using Smx.KodiInterop;
using System;
using System.Collections.Generic;

namespace Smx.KodiInterop.Python
{
	public class PyVariable: IDisposable
	{
		/// <summary>
		/// Name of the variable
		/// </summary>
		public string Name { get; private set; }

		public string DictName {
			get {
				if (this.IsMonitor)
					return "Monitors";
				if (this.IsPlayer)
					return "Players";

				return "Variables";
			}
		}

		public string PyName {
			get {
				return string.Format("{0}['{1}']", this.DictName, this.Name);
			}
		}

		public bool Exists {
			get {
				return bool.Parse(
					PythonInterop.EvalToResult(
						string.Format("'{0}' in {1}", this.Name, this.DictName)
					).Value
				);
			}
		}

		public bool IsObject {
			get {
				return this.Flags.HasFlag(PyVariableFlags.Object);
			}
		}

		public bool IsMonitor {
			get {
				return this.Flags.HasFlag(PyVariableFlags.Monitor);
			}
		}

		public bool IsPlayer {
			get {
				return this.Flags.HasFlag(PyVariableFlags.Player);
			}
		}

		public string TypeName {
			get {
				return PythonInterop.GetVariable(this).TypeName;
			}
		}

		private Messages.PythonEvalReply lastMsg;

		public dynamic Value {
			get {
				return PythonInterop.GetVariable(this).Value;
			}
			set {
				PythonInterop.EvalToVar(this, value);
			}
		}

		public T GetValue<T>() where T : IConvertible {
			return (T)Convert.ChangeType(this.Value, typeof(T));
		}

		public readonly PyVariableFlags Flags;

		public string CallFunction(
			PythonFunction function,
			string argumentsBody
		) {
			return PythonInterop.EvalToResult(string.Format("{0}.{1}({2})",
				this.PyName, function.Function, argumentsBody
			)).Value;
		}

		public string CallFunction(
			PythonFunction function,
			List<object> arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems
		) {
			if (arguments == null) {
				arguments = new List<object>();
			}

			List<string> textArguments = PythonInterop.EscapeArguments(arguments, escapeMethod);
			return CallFunction(function, string.Join(", ", textArguments));
		}

		public string CallFunction(
			string function,
			List<object> arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems
		){
			return CallFunction(new PythonFunction(function), arguments, escapeMethod);
		}

		/// <summary>
		/// Calls the specified function and assigns the result to this variable
		/// </summary>
		/// <param name="function"></param>
		/// <param name="arguments"></param>
		/// <param name="escapeMethod"></param>
		/// <returns></returns>
		public string CallAssign(
			PythonFunction function,
			List<object> arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems
		) {
			string argumentsText = "";
			if (arguments != null) {
				List<string> textArguments = PythonInterop.EscapeArguments(arguments, escapeMethod);
				argumentsText = string.Join(", ", textArguments);
			}

			PythonInterop.EvalToVar(this, "{0}({1})", new List<object> {
				function.ToString(), argumentsText
			}, EscapeFlags.None);

			return this.Value;
		}

		/// <summary>
		/// Represents a Python variable
		/// </summary>
		/// <param name="varName">Name of the variable</param>
		/// <param name="evalCode">Code that will be evaluated as the variable value/content</param>
		/// <param name="isObject">Indicates the variable will store non-serializable data</param>
		public PyVariable(
			string varName, string evalCode = null,
			PyVariableFlags flags = PyVariableFlags.Normal, bool keepValue = false
		) {
			this.Name = varName;

			if (flags.HasFlag(PyVariableFlags.Monitor) || flags.HasFlag(PyVariableFlags.Player))
				flags |= PyVariableFlags.Object;

			this.Flags = flags;

			if (evalCode == null && !keepValue) {
				evalCode = "''";
			}

			if(!keepValue)
				this.Value = evalCode;
		}

		~PyVariable() {
			PyConsole.WriteLine("Freeing variable " + this.Name);
			this.Dispose();
		}

		public void Dispose() {
			PyVariableManager.DestroyVariable(this.Name);
		}

		public override string ToString() {
			return string.Format("{0} = {1}", this.PyName, this.Value);
		}
	}
}