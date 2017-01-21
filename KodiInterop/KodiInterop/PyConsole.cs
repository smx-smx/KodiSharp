using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop
{
    public static class PyConsole
    {
		public static string NewLine {
			get	{
				if(Environment.NewLine.Length > 1) {
					return @"\r\n";
				} else {
					return @"\n";
				}
			}
		}
		public static void Print(object value) {
			string valueStr = PythonInterop.EscapeArgument(value.ToString());
			PythonInterop.Eval(string.Format("print {0}", valueStr));
		}

		public static void FancyPrint(string value) {
			string printStr = NewLine + NewLine;
			for (int i = 0; i < value.Length + 4; i++)
				printStr += '-';

			printStr += "| " + value + " |" + NewLine;

			for (int i = 0; i < value.Length + 4; i++)
				printStr += '-' + NewLine;

			Write(printStr);
		}

		public static void WriteLine(string value) {
			string valueStr = PythonInterop.EscapeArgument(value);
			string newlineStr = PythonInterop.EscapeArgument(NewLine, quote: true, rawstr: false);
			PythonInterop.Eval(string.Format("sys.stdout.write({0} + {1})", valueStr, newlineStr));
		}

		public static void Write(object value) {
			string valueStr = PythonInterop.EscapeArgument(value);
			PythonInterop.Eval(string.Format("sys.stdout.write({0})", valueStr));
		}
	}
}
