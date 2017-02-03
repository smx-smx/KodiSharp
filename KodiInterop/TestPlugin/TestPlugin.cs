using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using Smx.KodiInterop;
using System.Collections.Generic;
using System;

using Smx.KodiInterop.Builtins;
using mgr = Smx.KodiInterop.Python.PyVariableManager;
using Smx.KodiInterop.XbmcGui;
using System.Collections.Specialized;

namespace TestPlugin
{
	public class TestPlugin : KodiAddon
    {
		private static void testException() {
			throw new Exception("I should appear in kodi.log");
		}

		[Route("/")]
		public static void MainHandler(NameValueCollection parameters) {
			KodiAddon addon = KodiBridge.RunningAddon;
			List<ListItem> items = new List<ListItem> {
				new ListItem(
					label: "Nav",
					url: addon.BuildNavUrl("/nav"),
					isFolder: true
				),
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

			TestPluginState.LastMainPageVisitTime = DateTime.Now;
		}

		[Route("/nav")]
		public static void NavHandler(NameValueCollection parameters) {
			KodiAddon addon = KodiBridge.RunningAddon;

			string itemLabel = "Go to Main";
			if(TestPluginState.LastMainPageVisitTime != null) {
				itemLabel += string.Format(" (Last Visited: {0})", TestPluginState.LastMainPageVisitTime.Value.ToString());
			}

			List<ListItem> items = new List<ListItem> {
				new ListItem(
					label: itemLabel,
					url: addon.BuildNavUrl("/"),
					isFolder: true
				),
			};

			Console.WriteLine(string.Format("ListItem label is '{0}'", items[0].getLabel()));

			List.Add(items);
			List.Show();
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

			//ConsoleHelper.FreeConsole();
			return 0;
		}

		[DllExport("PluginMain", CallingConvention = CallingConvention.Cdecl)]
		public static int Main() {
			return new TestPlugin().Run();
		}
	}
}
