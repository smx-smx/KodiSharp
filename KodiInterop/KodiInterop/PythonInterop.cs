using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using Smx.KodiInterop.Messages;

namespace Smx.KodiInterop
{
	/// <summary>
	/// Supporting class that handles basic Python operations on top of KodiBridge
	/// </summary>
	public static class PythonInterop {
		#region Variables
		public static PythonEvalReply GetVariable(PyVariable variable) {
			return EvalToResult(variable.PyName);
		}

		#endregion

		#region Escape
		public static string EscapeArgument(object argument, EscapeFlags escapeMethod = EscapeFlags.Quotes) {
			if (argument == null) {
				return "None";
			}

			//If it's a variable, return it's unquoted python name
			if (argument is PyVariable)
				return (argument as PyVariable).PyName;

			string text = argument.ToString();

			//TODO: Find a better way
			//Invokes Enum.GetString<T>(argument)
			if (argument is Enum) {
				text = typeof(EnumExtensions)
					.GetMethod("GetString")
					.MakeGenericMethod(argument.GetType())
					.Invoke(null, new object[] { argument }) as string;
			}

			//Don't escape primitives
			if (
				argument is bool ||
				argument is int ||
				argument is uint ||
				argument is long ||
				argument is ulong ||
				argument is float ||
				argument is double
			) {
				return text;
			}

			if (escapeMethod.HasFlag(EscapeFlags.Quotes)) {
				text = Regex.Replace(text, "\r?\n", "\\n");
				text = '"' + text.Replace("\"", "\\\"") + '"';
			}
			if (escapeMethod.HasFlag(EscapeFlags.EscapeBuiltin)) {
				text = Regex.Replace(text, ",", "\\,");
			}
			if (escapeMethod.HasFlag(EscapeFlags.RawString)) {
				text = "r'" + text + "'";
			}

			return text;
		}

		public static List<string> EscapeArguments(IEnumerable<object> arguments, EscapeFlags escapeMethod = EscapeFlags.Quotes) {
			List<string> textArguments = new List<string>();

			int count = arguments.Count();
			if (escapeMethod.HasFlag(EscapeFlags.StripNullItems) && count > 0){
				List<object> argumentsList = arguments.ToList();
				int nulls = 0;
				// Start from end, go backwards
				for (int i = count - 1; i >= 0; --i) {
					// Found the end of the null series
					if (argumentsList[i] != null) {
						argumentsList.RemoveRange(i + 1, nulls);
						break;
					}
					nulls++;
				}
				if(count == nulls) {
					argumentsList.Clear();
				}
				arguments = argumentsList;
			}

			foreach (object argument in arguments) {
				textArguments.Add(EscapeArgument(argument, escapeMethod));
			}

			return textArguments;
		}

		public static List<string> EscapeArguments(EscapeFlags escapeMethod = EscapeFlags.Quotes, params object[] arguments) {
			return EscapeArguments(arguments, escapeMethod);
		}
		#endregion

		#region FunctionCall
		public static dynamic CallFunction(string functionName, IEnumerable<string> arguments = null) {
			string argumentsText = "";
			if (arguments != null)
				argumentsText = string.Join(",", arguments.ToArray());

			return EvalToResult(string.Format("{0}({1})", functionName, argumentsText)).Value;
		}

		public static dynamic CallFunction(PyFunction pythonFunction, IEnumerable<object> arguments = null, EscapeFlags flags = EscapeFlags.Quotes) {
			List<string> textArguments = null;
			if (arguments != null)
				 textArguments = EscapeArguments(arguments, flags);

			return CallFunction(
				pythonFunction.ToString(),
				textArguments
			);
		}

		public static dynamic CallFunction(PyModule module, string functionName, List<object> arguments = null, EscapeFlags flags = EscapeFlags.Quotes) {
			return CallFunction(new PyFunction(module, functionName), arguments, flags);
		}

		public static dynamic CallFunction(PyFunction pythonFunction, params object[] arguments) {
			return CallFunction(pythonFunction, arguments.ToList());
		}

		public static dynamic CallBuiltin(string builtinName, List<string> arguments = null) {
			return CallFunction(PyModule.Xbmc, "executebuiltin", new List<object> {
				//Kodi builtins shouldn't have quotes, so we pass a single parameter with the joined parameters
				string.Format("{0}({1})",
					builtinName,
					string.Join(",", arguments.Select(a => EscapeArgument(a, EscapeFlags.EscapeBuiltin)).ToArray())
				)
			});
		}

		public static dynamic CallBuiltin(string builtinName) {
			return CallBuiltin(builtinName, new List<string> { });
		}

		public static dynamic CallBuiltin(string builtinName, List<object> arguments) {
			List<string> textArguments = EscapeArguments(arguments, EscapeFlags.None);
			return CallBuiltin(builtinName, textArguments);
		}

		public static dynamic CallBuiltin(string builtinName, params string[] arguments) {
			return CallBuiltin(builtinName, arguments.ToList());
		}

		public static dynamic CallBuiltin(string builtinName, params object[] arguments) {
			return CallBuiltin(builtinName, arguments.ToList());
		}
		#endregion

		#region Eval
		public static PythonEvalReply Eval(string code) {
			PythonEvalMessage msg = new PythonEvalMessage {
				Code = code
			};

			string replyString = KodiBridge.SendMessage(msg);
			PythonEvalReply reply = JsonConvert.DeserializeObject<PythonEvalReply>(replyString);
			return reply;
		}

		public static PythonEvalReply EvalToVar(string pyVarName, string code) {
			Console.WriteLine(pyVarName + " = " + code);
			return Eval(string.Format("{0}={1}", pyVarName, code));
		}

		public static PythonEvalReply EvalToVar(PyVariable variable, string code) {
			return EvalToVar(variable.PyName, code);
		}

		public static PythonEvalReply EvalToVar(PyVariable variable, string codeFormat, List<object> arguments, EscapeFlags escapeMethod) {
			List<string> textArguments = EscapeArguments(arguments, escapeMethod);
			return EvalToVar(variable, string.Format(codeFormat, textArguments.ToArray()));
		}

		public static PythonEvalReply EvalToResult(string code) {
			return EvalToVar(PyVariableManager.LastResult, code);
		}
		#endregion
	}
}
