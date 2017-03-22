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
using Smx.KodiInterop.Python;

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
					label: "AudioPlayer",
					url: addon.BuildNavUrl("/audio"),
					isFolder: true
				),
				new ListItem(
					label: "Events",
					url: addon.BuildNavUrl("/events"),
					isFolder: true
				),
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

		/*
		 * These routes used to work with the previous threaded implementation but were causing GIL/Context issues
		 * The new routes are based on xbmc.wait() and use a callback instead of a thread to message Python
		 * This means that you must wait() for the time you want to catch events, blocking the UI meanwhile
		 * 
		 * TODO: It would be nice to find a way that works without trashing the Python Context
		 **/
		#region OldRoutes
		[Route("/audio2")]
		public static void GlobalAudioNavHandler(NameValueCollection parameters) {
			Player player = new Player(PyVariableManager.Player);
			GlobalEvents.PlayBackStarted += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback started!");
			});
			GlobalEvents.PlayBackEnded += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback finished!");
			});
		}

		[Route("/events2")]
		public static void GlobalEventsNavHandler(NameValueCollection parameters) {
			GlobalEvents.Notification += new EventHandler<NotificationEventArgs>(delegate (object s, NotificationEventArgs ev) {
				Console.WriteLine(string.Format("=> Notification from {0}({1}) ==> {2}", ev.Sender, ev.Method, ev.Data));
			});

			Thread.Sleep(TimeSpan.FromSeconds(1));
			Console.WriteLine("Triggering screensaver");
			SystemBuiltins.ActivateScreensaver();
		}
		#endregion

		[Route("/audio")]
		public static void AudioNavHandler(NameValueCollection parameters) {
			//Player player = new Player(PyVariableManager.Player);
			Player player = new Player();
			player.PlayBackStarted += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback started!");
			});
			player.PlayBackEnded += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback finished!");
			});
			player.Play("http://www.bensound.com/royalty-free-music?download=memories");

			/* Keep monitoring for a bit */
			Kodi.Sleep(TimeSpan.FromSeconds(20));

		}

		[Route("/events")]
		public static void EventsNavHandler(NameValueCollection parameters) {
			Smx.KodiInterop.Modules.Xbmc.Monitor m = new Smx.KodiInterop.Modules.Xbmc.Monitor();
			m.Notification += new EventHandler<NotificationEventArgs>(delegate (object s, NotificationEventArgs ev) {
				Console.WriteLine(string.Format("=> Notification from {0}({1}) ==> {2}", ev.Sender, ev.Method, ev.Data));
			});

			Thread.Sleep(TimeSpan.FromSeconds(1));
			Console.WriteLine("Triggering screensaver");
			SystemBuiltins.ActivateScreensaver();

			/* Keep monitoring for a bit */
			Kodi.Sleep(TimeSpan.FromSeconds(10));
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
