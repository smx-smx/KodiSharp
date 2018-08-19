﻿#if !UNIX
using RGiesecke.DllExport;
#endif

using System.Runtime.InteropServices;
using Smx.KodiInterop;
using System.Collections.Generic;
using System;

using Smx.KodiInterop.Builtins;
using System.Collections.Specialized;

using Smx.KodiInterop.Modules.XbmcGui;
using Smx.KodiInterop.Modules.Xbmc;
using System.Threading;
using Smx.KodiInterop.Python;
using Smx.KodiInterop.Python.ValueConverters;

namespace TestPlugin
{
	public class TestPlugin : KodiAddon
	{
		public static string TempDir = XbmcPath.translatePath(SpecialPaths.Temp);

        public TestPlugin() : base(true)
        {
        }

		private static void testException()
		{
			throw new Exception("I should appear in kodi.log");
		}

		[Route("/")]
		public void MainHandler(NameValueCollection parameters)
		{
			TestPlugin addon = KodiBridge.RunningAddon as TestPlugin;
			List<ListItem> items = new List<ListItem> {
				new ListItem(
					label: "AudioPlayer",
					url: addon.BuildNavUrl("/audio"),
					isFolder: true,
					art: new Dictionary<Art, string> {
						{ Art.Poster, "https://upload.wikimedia.org/wikipedia/commons/2/25/Kodi-logo-Thumbnail-light-transparent.png" },
						{ Art.Fanart, "https://images.pexels.com/photos/167092/pexels-photo-167092.jpeg?dl&fit=crop&crop=entropy&w=6000&h=4000" }
					}
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

		[Route("/audio")]
		public void AudioNavHandler(NameValueCollection parameters)
		{
			XbmcPlayer player = new XbmcPlayer();
			player.PlayBackStarted += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback started!");
			});
			player.PlayBackEnded += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback finished!");
			});

			//Thread.Sleep(TimeSpan.FromSeconds(1));
			player.Play("http://www.bensound.com/royalty-free-music?download=memories");
			/* Keep monitoring for a bit */
			Kodi.Sleep(TimeSpan.FromSeconds(10));
		}

		[Route("/events")]
		public void EventsNavHandler(NameValueCollection parameters)
		{
            XbmcMonitor m = new XbmcMonitor();
            m.ScreensaverActivated += new EventHandler<EventArgs>(delegate (object s, EventArgs ev)
            {
                Console.WriteLine("=> Screensaver Activated!");
            });

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
		public void NavHandler(NameValueCollection parameters)
		{
			TestPlugin addon = KodiBridge.RunningAddon as TestPlugin;

			string itemLabel = "Go to Main";
			if (TestPluginState.LastMainPageVisitTime != null) {
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
		public override int PluginMain()
		{
			TestPlugin addon = KodiBridge.RunningAddon as TestPlugin;

			ConsoleHelper.CreateConsole();
			Console.WriteLine("TestPlugin v1.0 - Smx");

			var sum = PyVariableManager.NewVariable();
			sum.Value = "1+2";
			PyConsole.WriteLine("Result: " + sum);

			Console.WriteLine("Settings['test'] => " + addon.Settings["test"]);

			var dict = PyVariableManager.NewVariable();
			Dictionary<string, string> TestDict = new Dictionary<string, string> {
				{"hello", "python" },
				{"dict", "test" }
			};
			dict.Value = TestDict.ToPythonCode();
			PyConsole.WriteLine("Dict: " + dict);

			PyConsole.WriteLine("Hello Python");

			//ConsoleHelper.FreeConsole();
			return 0;
		}

#if !UNIX
		[DllExport("PluginMain", CallingConvention = CallingConvention.Cdecl)]
#endif
		public static int Main() {
			return new TestPlugin().Run();
		}
	}
}
