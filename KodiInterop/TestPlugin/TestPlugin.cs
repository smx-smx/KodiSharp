using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using Smx.KodiInterop;
using Smx.KodiInterop.Messages;
using System.Collections.Generic;
using System;
using System.Threading;

using Smx.KodiInterop.Builtins;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace TestPlugin {
	public class TestPlugin : KodiAddon
    {
		private static void testException() {
			throw new Exception("I should appear in kodi.log");
		}

		/// <summary>
		/// Plugin Main Logic
		/// </summary>
		/// <returns></returns>
		public override int PluginMain() {
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
		public static int Main() {
			return new TestPlugin().Run();
		}
	}
}
