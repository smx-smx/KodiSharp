using Newtonsoft.Json;
using Smx.KodiInterop;
using Smx.KodiInterop.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Smx.KodiInterop
{
    public static class PythonInterop
    {
		private const string LastResultVarName = "LastResult";
		//WIP: Sync with Python variables value
		private static Dictionary<string, PyVariable> _variables = new Dictionary<string, PyVariable> {
			{ LastResultVarName, new PyVariable(LastResultVarName) }
		};

		public static string EscapeArgument(object argument, EscapeMethod escapeMethod = EscapeMethod.Quotes) {
			string text = argument.ToString();

			switch (escapeMethod) {
				case EscapeMethod.Quotes:
					text = Regex.Replace(argument.ToString(), "\r?\n", "\\n");
					return '"' + text + '"';
				case EscapeMethod.RawString:
					return "r'" + text + "'";
				case EscapeMethod.None:
				default:
					return text;
			}
		}

		public static List<string> EscapeArguments(List<object> arguments, EscapeMethod escapeMethod = EscapeMethod.Quotes) {
			List<string> textArguments = new List<string>();
			foreach (object argument in arguments) {
				textArguments.Add(EscapeArgument(argument, escapeMethod));
			}
			return textArguments;
		}

		public static string CallFunction(string moduleName, string functionName, List<string> arguments) {
			return EvalToResult(string.Format("{0}.{1}({2})", moduleName, functionName, string.Join(",", arguments.ToArray())));
		}

		public static string GetVariable(string variableName) {
			return EvalToResult(string.Format("Variables['{0}']", variableName));
		}

		public static void DestroyVariable(string variableName) {
			Eval(string.Format("del Variables['{0}']", variableName));
		}

		public static string EvalToResult(string code) {
			string replyString = EvalToVar(LastResultVarName, code);
			PythonEvalReply reply = JsonConvert.DeserializeObject<PythonEvalReply>(replyString);
			return reply.Result;
		}

		public static string EvalToVar(string variableName, string code) {
			return Eval(string.Format("Variables['{0}'] = {1}", variableName, code));
		}

		public static string Eval(string code) {
			PythonEvalMessage msg = new PythonEvalMessage {
				Code = code
			};

			return KodiBridge.SendMessage(msg);
		}

		public static string CallFunction(string moduleName, string functionName, List<object> arguments) {
			List<string> textArguments = EscapeArguments(arguments);
			return CallFunction(moduleName, functionName, textArguments);
		}

		public static string  CallBuiltin(string builtinName, List<string> arguments) {
			return CallFunction(PythonModules.Xbmc, "executebuiltin", new List<object> {
				string.Format("{0}({1})",
					builtinName,
					string.Join(",", arguments.ToArray())
				)
			});
		}

		public static string CallBuiltin(string builtinName) {
			return CallBuiltin(builtinName, new List<string> { });
		}

		public static string CallBuiltin(string builtinName, List<object> arguments) {
			List<string> textArguments = EscapeArguments(arguments, EscapeMethod.None);
			return CallBuiltin(builtinName, textArguments);
		}
    }
}
