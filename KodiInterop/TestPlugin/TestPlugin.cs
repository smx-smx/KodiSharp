using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using Smx.KodiInterop;
using System.Collections.Generic;
using System;

using Smx.KodiInterop.Builtins;
using System.Collections.Specialized;

using mgr = Smx.KodiInterop.Python.PyVariableManager;
using Smx.KodiInterop.Modules.XbmcGui;
using Smx.KodiInterop.Modules.Xbmc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestPlugin
{
	public class TestPlugin : KodiAddon
    {
		public static string TempDir = Path.translatePath(SpecialPaths.Temp);

		private static void testException() {
			throw new Exception("I should appear in kodi.log");
		}

		[Route("/")]
		public static void MainHandler(NameValueCollection parameters) {
			TestPlugin addon = KodiBridge.RunningAddon as TestPlugin;
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

			//Console.WriteLine("Settings Test: " + addon.Settings["test"]);

			UiBuiltins.Notification(
				header: "My Notification",
				message: "Hello World from C#",
				duration: TimeSpan.FromSeconds(10)
			);

			/*
			 * Persistent variables preserve their value between multiple plugin invokations,
			 * unlike python in XBMC where everything is destroyed when the plugin is invoked again (e.g navigating between pages).
			 * This is possible due to Assembly Domain that is created by the CLR once the plugin is loaded for the first time. 
			 **/
			TestPluginState.LastMainPageVisitTime = DateTime.Now;
		}

		[Route("/nav")]
		public static void NavHandler(NameValueCollection parameters) {
			TestPlugin addon = KodiBridge.RunningAddon as TestPlugin;

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

			Console.WriteLine(string.Format("ListItem label is '{0}'", items[0].Label));

			//var rep = Smx.KodiInterop.JsonRpc.Addons.ExecuteAddon("plugin.video.dplay2", wait: true);
			//PyConsole.WriteLine(rep.);

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

			Events.ScreensaverActivated += new EventHandler<EventArgs>(delegate (object s, EventArgs ev)
			{
				//Note, EventArgs is null for now
				Console.WriteLine("Screensaver Triggered!");
			});

			Thread.Sleep(TimeSpan.FromSeconds(1));
			Console.WriteLine("Triggering screensaver");
			SystemBuiltins.ActivateScreensaver();

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
