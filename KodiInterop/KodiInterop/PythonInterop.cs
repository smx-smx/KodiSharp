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
		public static string EscapeArgument(object argument, bool quote = true, bool rawstr = true) {
			string ret = "";
			if (quote) {
				if (rawstr)
					return "r'" + argument.ToString() + "'";
				else
					return '"' + argument.ToString() + '"';
			} else {
				return argument.ToString();
			}
		}

		public static List<string> EscapeArguments(List<object> arguments, bool quote = true) {
			List<string> textArguments = new List<string>();
			foreach (object argument in arguments) {
				textArguments.Add(EscapeArgument(argument, quote));
			}
			return textArguments;
		}

		public static string CallFunction(string moduleName, string functionName, List<string> arguments) {
			return EvalResult(string.Format("{0}.{1}({2})", moduleName, functionName, string.Join(",", arguments.ToArray())));
		}

		public static void DestroyVariable(string variableName) {
			Eval(string.Format("del Variables.{0}", variableName));
		}

		public static string EvalResult(string code) {
			string replyString = Eval(string.Format("Variables['LastResult'] = {0}", code));
			PythonEvalReply reply = JsonConvert.DeserializeObject<PythonEvalReply>(replyString);
			return reply.Result;
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
			List<string> textArguments = EscapeArguments(arguments, false);
			return CallBuiltin(builtinName, textArguments);
		}
    }
}
