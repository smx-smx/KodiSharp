using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using Smx.KodiInterop;
using System.Collections.Generic;
using System;

using Smx.KodiInterop.Builtins;
using mgr = Smx.KodiInterop.Python.PyVariableManager;
using Smx.KodiInterop.XbmcGui;

namespace TestPlugin
{
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

			var sum = mgr.NewVariable();
			sum.Value = "1+2";
			PyConsole.WriteLine("Result: " + sum);
			sum.Dispose();

			PyConsole.WriteLine("Hello Python");

			List<ListItem> items = new List<ListItem> {
				new ListItem("My ListItem"),
				new ListItem("Item 2"),
				new ListItem("Item 3")
			};
			List.Add(items);
			List.Show();

			UiBuiltins.Notification(
				header: "My Notification",
				message: "Hello World from C#",
				duration: TimeSpan.FromSeconds(10)
			);
			testException();

			//ConsoleHelper.FreeConsole();
			return 0;
		}

		[DllExport("PluginMain", CallingConvention = CallingConvention.Cdecl)]
		public static int Main() {
			return new TestPlugin().Run();
		}
	}
}
