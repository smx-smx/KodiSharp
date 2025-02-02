using Newtonsoft.Json.Linq;
using Smx.KodiInterop;
using Smx.KodiInterop.Python.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Python
{
	public class PyVariable: IDisposable, PythonConvertible
	{
		public readonly string? Basename;
		public readonly string PyName;
		private readonly bool Disposable;

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

		public dynamic CallFunction(
			PyFunction function,
			string argumentsBody,
			PyVariable? target = null
		) {

			if (target == null)
				target = PyVariableManager.LastResult;

			return (PythonInterop.EvalToVar(target, string.Format("{0}.{1}({2})",
				this.PyName, function.Function, argumentsBody
			))).Value;
		}

		public dynamic CallFunction(
			PyFunction function,
			List<object?>? arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems,
			PyVariable? target = null
		) {
			if (arguments == null) {
				arguments = new List<object?>();
			}

			List<string> textArguments = PythonInterop.EscapeArguments(arguments, escapeMethod);
			return CallFunction(function, string.Join(", ", textArguments), target: target);
		}

		public dynamic CallFunction(
			string function,
			List<object?>? arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems,
			PyVariable? target = null
		) {
			return CallFunction(PyFunction.ClassFunction(function), arguments, escapeMethod, target: target);
		}

		public dynamic CallFunction(PyFunction function, params object?[] args) {
			return CallFunction(function, args.ToList());
		}

		/// <summary>
		/// Calls the specified function and assigns the result to this variable
		/// </summary>
		/// <param name="function"></param>
		/// <param name="arguments"></param>
		/// <param name="escapeMethod"></param>
		/// <returns></returns>
		public dynamic CallAssign(
			PyFunction function,
			List<object?>? arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems,
			PyVariable? target = null
		) {
			string argumentsText = "";
			if (arguments != null) {
				List<string> textArguments = PythonInterop.EscapeArguments(arguments, escapeMethod);
				argumentsText = string.Join(", ", textArguments);
			}

			if (target == null) {
				target = this;
			}

			PythonInterop.EvalToVar(target, "{0}({1})", new List<object> {
				function.ToString(), argumentsText
			}, EscapeFlags.None);

			return this.Value;
		}

		public dynamic CallAssign(
			string function,
			List<object?>? arguments = null,
			EscapeFlags escapeMethod = EscapeFlags.Quotes | EscapeFlags.StripNullItems,
			PyVariable? target = null
		)
		{
			return CallAssign(PyFunction.ClassFunction(function), arguments, escapeMethod, target);
		}

		public dynamic CallAssign(PyFunction function, params object?[] args) {
			return CallAssign(function, args.ToList());
		}

        public void EvalAssign(string code)
        {
            PythonInterop.EvalToVar(this, code);
        }

		/// <summary>
		/// Represents a Python variable
		/// </summary>
		/// <param name="evalCode">Code that will be evaluated to get/set the variable value/content</param>
		/// <param name="isObject">Indicates the variable will store non-serializable data</param>
		public PyVariable(
			string evalCode,
			string? basename = null,
			bool disposable = true
		) {
			this.PyName = evalCode;
            this.Disposable = disposable;
			this.Basename = basename;
		}

		public void Dispose() {
            if (this.Disposable)
            {
				Console.WriteLine("...Deleting " + this.PyName);
				PyVariableManager.Get.DeleteVariable(this);
            }
		}

		public override string ToString() {
			return string.Format("{0} = {1}", this.PyName, this.Value);
		}

		public string ToPythonCode() {
			return PyName;
		}
	}
}