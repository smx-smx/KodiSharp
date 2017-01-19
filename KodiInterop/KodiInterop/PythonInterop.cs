using Smx.KodiInterop;
using Smx.KodiInterop.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop
{
    class PythonInterop
    {
		public static List<string> EscapeArguments(List<object> arguments, bool quote = true) {
			List<string> textArguments = new List<string>();
			foreach (object argument in arguments) {
				if (argument is string && quote == true) {
					textArguments.Add('"' + argument.ToString() + '"');
				} else {
					textArguments.Add(argument.ToString());
				}
			}
			return textArguments;
		}

		public static void CallFunction(string moduleName, string functionName, List<string> arguments) {
			PythonEvalMessage msg = new PythonEvalMessage {
				Code = string.Format("{0}.{1}({2})", moduleName, functionName, string.Join(",", arguments.ToArray()))
			};
			KodiBridge.SendMessage(msg);
		}

		public static void CallFunction(string moduleName, string functionName, List<object> arguments) {
			List<string> textArguments = EscapeArguments(arguments);
			CallFunction(moduleName, functionName, textArguments);
		}

		public static void CallBuiltin(string builtinName, List<string> arguments) {
			CallFunction(PythonModules.Xbmc, "executebuiltin", new List<object> {
				string.Format("{0}({1})",
					builtinName,
					string.Join(",", arguments.ToArray())
				)
			});
		}

		public static void CallBuiltin(string builtinName) {
			CallBuiltin(builtinName, new List<string> { });
		}

		public static void CallBuiltin(string builtinName, List<object> arguments) {
			List<string> textArguments = EscapeArguments(arguments, false);
			CallBuiltin(builtinName, textArguments);
		}
    }
}
