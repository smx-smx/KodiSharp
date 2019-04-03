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
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace TestPlugin
{
	public class TestPlugin : KodiAddon
	{
		public static string TempDir = XbmcPath.translatePath(SpecialPaths.Temp);

		public override string PluginId => "plugin.video.test";

#if DEBUG
		const bool DEBUG = true;
#else
		const bool DEBUG = false;
#endif

		public TestPlugin() : base(persist: true)
        {
        }

		private static void testException()
		{
			throw new Exception("I should appear in kodi.log");
		}

		[Route("/")]
		public int MainHandler(NameValueCollection parameters)
		{
			List<ListItem> items = new List<ListItem> {
				new ListItem(
					label: "AudioPlayer",
					url: BuildNavUrl("/audio"),
					isFolder: true,
					art: new Dictionary<Art, string> {
						{ Art.Poster, "https://upload.wikimedia.org/wikipedia/commons/2/25/Kodi-logo-Thumbnail-light-transparent.png" },
						{ Art.Fanart, "https://images.pexels.com/photos/167092/pexels-photo-167092.jpeg?dl&fit=crop&crop=entropy&w=6000&h=4000" }
					}
				),
				new ListItem(
					label: "Events",
					url: BuildNavUrl("/events"),
					isFolder: true
				),
				new ListItem(
					label: "Nav",
					url: BuildNavUrl("/nav"),
					isFolder: true
				),
				new ListItem(
					label: "Playlist",
					url: BuildNavUrl("/playlist"),
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
				duration: TimeSpan.FromSeconds(1)
			);

			/*
			 * Persistent variables preserve their value between multiple plugin invokations,
			 * unlike python in XBMC where everything is destroyed when the plugin is invoked again (e.g navigating between pages).
			 * This is possible due to Assembly Domain that is created by the CLR once the plugin is loaded for the first time. 
			 **/
			TestPluginState.LastMainPageVisitTime = DateTime.Now;

			return 0;
		}

		[Route("/playlist")]
		public int PlaylistHandler(NameValueCollection parameters) {
			Uri uri = new Uri("http://www.amclassical.com/piano/");

			var data = new HttpClient()
				.GetAsync(uri)
				.Result.Content
				.ReadAsStringAsync()
				.Result;

			PlayList pl = new PlayList(PlayListType.Music);

			var matches = new Regex("<a href=\"?(.*?)\"?>.*</a>").Matches(data);
			var number = Math.Min(5, matches.Count);
			for (var i = 0; i < number; i++) {
				string path = matches[i].Groups[1].Value.ToLower();
				if (!path.EndsWith("mp3"))
					continue;

				string url = uri.GetLeftPart(UriPartial.Authority) + path;

				pl.Add(url);
			}

			var player = new XbmcPlayer();
			player.Play(pl);

			for(int i=0; i<pl.Count; i++) {
				while (!player.IsPlayingAudio) {
					Kodi.Sleep(TimeSpan.FromMilliseconds(200));
				}
				var info = player.MusicInfoTag;
				Console.WriteLine($"[NOW PLAYING]: {player.PlayingFile}");
				Console.WriteLine("=====================");
				Console.WriteLine($"{info.URL}, {info.Title}");

				Kodi.Sleep(TimeSpan.FromSeconds(5));
				player.PlayNext();
			}

			return 0;
		}

		[Route("/audio")]
		public int AudioNavHandler(NameValueCollection parameters)
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

			while (!player.IsPlayingAudio) {
				Kodi.Sleep(TimeSpan.FromSeconds(1));
			}

			for(int i=10; i<100; i+=5) {
				ApplicationBuiltins.SetVolume(i, true);
				Kodi.Sleep(TimeSpan.FromMilliseconds(200));
			}

			/* Keep monitoring for a bit */
			Kodi.Sleep(TimeSpan.FromSeconds(10));

			return 0;
		}

		[Route("/events")]
		public int EventsNavHandler(NameValueCollection parameters)
		{
			using (XbmcMonitor m = new XbmcMonitor()) {
				m.OnScreensaverActivated += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
					Console.WriteLine("=> Screensaver Activated!");
				});

				m.OnScreensaverDeactivated += new EventHandler<EventArgs>(delegate(object s, EventArgs ev) {
					Console.WriteLine("=> Screensaver Deactivated!");
				});

				m.OnNotification += new EventHandler<NotificationEventArgs>(delegate (object s, NotificationEventArgs ev) {
					Console.WriteLine(string.Format("=> Notification from {0}({1}) ==> {2}", ev.Sender, ev.Method, ev.Data));
				});

				Kodi.Sleep(TimeSpan.FromSeconds(1));
				Console.WriteLine("Triggering screensaver");
				SystemBuiltins.ActivateScreensaver();

				if (!m.AbortRequested) {
					m.WaitForAbort(TimeSpan.FromSeconds(10));
				}
			}
			return 0;
		}

		[Route("/nav")]
		public int NavHandler(NameValueCollection parameters)
		{
			string itemLabel = "Go to Main";
			if (TestPluginState.LastMainPageVisitTime != null) {
				itemLabel += string.Format(" (Last Visited: {0})", TestPluginState.LastMainPageVisitTime.Value.ToString());
			}

			List<ListItem> items = new List<ListItem> {
				new ListItem(
					label: itemLabel,
					url: BuildNavUrl("/"),
					isFolder: true
				),
			};

			Console.WriteLine(string.Format("ListItem label is '{0}'", items[0].Label));

			List.Add(items);
			List.Show();

			return 0;
		}

		public override int DefaultRoute()
		{
			TestPlugin addon = KodiBridge.RunningAddon as TestPlugin;

			//ConsoleHelper.CreateConsole();
			Console.WriteLine("TestPlugin v1.0 - Smx");

			using(var sum = PyVariableManager.NewVariable()) {
				sum.Value = "1+2";
				PyConsole.WriteLine("Result: " + sum);
			}

			//Console.WriteLine("Settings['test'] => " + addon.Settings["test"]);

			using (var dict = PyVariableManager.NewVariable()) {
				Dictionary<string, string> TestDict = new Dictionary<string, string> {
					{"hello", "python" },
					{"dict", "test" }
				};
				dict.Value = TestDict.ToPythonCode();
				PyConsole.WriteLine("Dict: " + dict);
			}

			PyConsole.WriteLine("Hello Python");
			
			//ConsoleHelper.FreeConsole();
			return 0;
		}

		[PluginEntry]
		public static int PluginMain() {
			return GetInstance<TestPlugin>(typeof(TestPlugin), enableDebug: DEBUG, persist: true).Run();
		}
	}
}
