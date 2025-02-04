﻿using System.Runtime.InteropServices;
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
using System.IO;
using System.Linq;

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

		private static void testException()
		{
			throw new Exception("I should appear in kodi.log");
		}

		private DateTime? LastNavVisited = null;

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
				new ListItem(
					label: "Video",
					url: BuildNavUrl("/video"),
					art: new Dictionary<Art, string>() {
						{ Art.Fanart, "https://mango.blender.org/wp-content/gallery/4k-renders/26_thom_robot.jpg" },
						{ Art.Poster, "https://mango.blender.org/wp-content/themes/tearsofsteel/images/logo.png" },
					}
				),
				new ListItem(
					label: "Video2",
					url: BuildNavUrl("/video2"),
					art: new Dictionary<Art, string>() {
						{ Art.Poster, "https://peach.blender.org/wp-content/uploads/poster_bunny_small.jpg?x81236" },
						{ Art.Fanart, "https://peach.blender.org/wp-content/uploads/bbb-splash.png?x81236" }
					}
				)
			};

			List.Add(items);
			List.Show();

			UiBuiltins.Notification(
				header: "My Notification",
				message: "Hello World from C#",
				duration: TimeSpan.FromSeconds(1)
			);

			return 0;
		}


		[Route("/video")]
		public int VideoHandler(NameValueCollection parameters) {
			new XbmcPlayer().Play("http://ftp.halifax.rwth-aachen.de/blender/demo/movies/ToS/ToS-4k-1920.mov");
			return 0;
		}

		[Route("/video2")]
		public int VideoHandler2(NameValueCollection parameters) {
			new XbmcPlayer().Play("http://distribution.bbb3d.renderfarming.net/video/mp4/bbb_sunflower_1080p_60fps_normal.mp4");
			return 0;
		}

		[Route("/playlist")]
		public int PlaylistHandler(NameValueCollection parameters) {
			Uri uri = new Uri("http://www.download2mp3.com/beethoven.htm");

			var data = new HttpClient()
				.GetAsync(uri)
				.Result.Content
				.ReadAsStringAsync()
				.Result;

			PlayList pl = new PlayList(PlayListType.Music);

			List<ListItem> items = new List<ListItem> {};

			var links = new Regex("<a href=\"(.*?)\"")
				.Matches(data)
				.Cast<Match>()
				.Where(m => m.Groups[1].Value.EndsWith(".mp3"))
				.Select(m => {
					return uri.GetLeftPart(UriPartial.Authority) + "/" + m.Groups[1].Value;
				})
				.Take(5)
				.ToArray();

			foreach(var linkUri in links) {
				pl.Add(linkUri);
				items.Add(new ListItem(
					label: new Uri(linkUri).LocalPath,
					url: linkUri));
			}

			List.Add(items);
			List.Show();

			var player = new XbmcPlayer();
			player.Play(pl);

			/**
			 * go to the next playlist entry after 5 seconds
			 * counted from when the playback started
			 */
			int currentIndex = 0;
			player.AVStarted += (o, ev) => {
				var info = player.MusicInfoTag;
				Console.WriteLine($"[NOW PLAYING]: {player.PlayingFile}");
				Console.WriteLine("=====================");
				Console.WriteLine($"{info.URL}, {info.Title}");

				Kodi.Sleep(TimeSpan.FromSeconds(5));
				player.PlayNext();
				++currentIndex;
			};

			// keep the script alive until the playlist if over
			while(currentIndex < pl.Count) {
				Kodi.Sleep(TimeSpan.FromSeconds(1));
			}

			return 0;
		}

		[Route("/audio")]
		public int AudioNavHandler(NameValueCollection parameters)
		{
			// workaround for https://github.com/xbmc/xbmc/issues/16756 - "busy dialog already running"
			Dialog.CloseAll(force: true);

			XbmcPlayer player = new XbmcPlayer();
			player.PlayBackStarted += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback started!");
			});
			player.PlayBackEnded += new EventHandler<EventArgs>(delegate (object s, EventArgs ev) {
				Console.WriteLine("=> Playback finished!");
			});

			//Thread.Sleep(TimeSpan.FromSeconds(1));
			player.Play("https://archive.scene.org/pub/music/groups/farb-rausch/008b-kb-the_product_main.mp3");

			while (!player.IsPlayingAudio) {
				Kodi.Sleep(TimeSpan.FromSeconds(1));
			}

			for(int i=10; i<=100; i+=5) {
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
			if (LastNavVisited != null) {
				itemLabel += string.Format(" (Last Visited: {0})", LastNavVisited.Value.ToString());
			}
			LastNavVisited = DateTime.Now;

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
			TestPlugin addon = (TestPlugin)KodiBridge.EnsureRunningAddon();

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
			return GetInstance<TestPlugin>(enableDebug: DEBUG, persist: true).Run();
		}
	}
}
