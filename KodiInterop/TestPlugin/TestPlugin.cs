using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using Smx.KodiInterop;
using Smx.KodiInterop.Messages;
using System.Collections.Generic;
using System;
using System.Threading;

using Smx.KodiInterop.Builtins;
using System.Diagnostics;

namespace TestPlugin {
	public class TestPlugin
    {
		private static string PluginPath = null;

		[DllExport("PluginInit", CallingConvention = CallingConvention.Cdecl)]
		public static void PluginInit(string pluginPath) {
			PluginPath = pluginPath;
		}

		private static void testException() {
			throw new Exception("I should appear in kodi.log");
		}

		/// <summary>
		/// Plugin Main Logic
		/// </summary>
		/// <returns></returns>
		public static int Main() {
			ConsoleHelper.CreateConsole();
			Console.WriteLine("TestPlugin v1.0 - Smx");

			PyConsole.WriteLine("Hello Python");
			PythonInterop.EvalToVar("sum", "1 + 2");
			string opResult = PythonInterop.GetVariable("sum");
			PythonInterop.DestroyVariable("sum");

			PyConsole.WriteLine("Result: " + opResult);
			UiBuiltins.Notification(
				header: "My Notification",
				message: "Hello World from C#",
				duration: TimeSpan.FromSeconds(10)
			);
			testException();
			return 0;
		}

		[DllExport("PluginMain", CallingConvention = CallingConvention.Cdecl)]
		public static int PluginMain() {
			try {
				return Main();
			}
			catch(Exception ex) {
				// This takes the exception and stores it, not allowing it to bubble up
				KodiBridge.SaveException(ex);
				return 1;
			}
		}
	}
}
