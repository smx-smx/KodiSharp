using Smx.KodiInterop;
using Smx.KodiInterop.Messages;
using Smx.KodiInterop.Modules.Xbmc;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public static class GlobalEvents
	{
		#region MonitorEvents
		public static event EventHandler<EventArgs> AbortRequested;
		public static event EventHandler<LibraryEventArgs> CleanStarted;
		public static event EventHandler<LibraryEventArgs> CleanFinished;
		public static event EventHandler<DatabaseEventArgs> DatabaseScanStarted;
		public static event EventHandler<DatabaseEventArgs> DatabaseUpdated;
		public static event EventHandler<EventArgs> DPMSActivated;
		public static event EventHandler<EventArgs> DPMSDeactivated;
		public static event EventHandler<EventArgs> ScreensaverActivated;
		public static event EventHandler<EventArgs> ScreensaverDeactivated;
		public static event EventHandler<LibraryEventArgs> ScanStarted;
		public static event EventHandler<LibraryEventArgs> ScanFinished;
		public static event EventHandler<EventArgs> SettingsChanged;
		public static event EventHandler<NotificationEventArgs> Notification;
		#endregion

		#region PlayerEvents
		public static event EventHandler<EventArgs> PlayBackEnded;
		public static event EventHandler<EventArgs> PlayBackPaused;
		public static event EventHandler<EventArgs> PlayBackResumed;
		public static event EventHandler<PlayBackSeekEventArgs> PlayBackSeek;
		public static event EventHandler<PlayBackSeekChapterEventArgs> PlayBackSeekChapter;
		public static event EventHandler<PlayBackSpeedChangedEventArgs> PlayBackSpeedChanged;
		public static event EventHandler<EventArgs> PlayBackStarted;
		public static event EventHandler<EventArgs> PlayBackStopped;
		public static event EventHandler<EventArgs> QueueNextItem;
		#endregion

		public static bool DispatchEvent(KodiEventMessage e) {
			switch (e.Sender) {
				case "Monitor":
					return DispatchMonitorEvent(e);
				case "Player":
					return DispatchPlayerEvent(e);
				default:
					throw new Exception(string.Format("Unknown event source {0}", e.Source));
			}
		}

		public static bool DispatchPlayerEvent(KodiEventMessage e) {
			switch (e.Source) {
				case "onPlayBackEnded":
					PlayBackEnded?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackPaused":
					PlayBackPaused?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackResumed":
					PlayBackResumed?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackSeek":
					PlayBackSeek?.Invoke(null, new PlayBackSeekEventArgs(int.Parse(e.EventArgs[0]), int.Parse(e.EventArgs[1])));
					break;
				case "onPlayBackSeekChapter":
					PlayBackSeekChapter?.Invoke(null, new PlayBackSeekChapterEventArgs(int.Parse(e.EventArgs[0])));
					break;
				case "onPlayBackSpeedChanged":
					PlayBackSpeedChanged?.Invoke(null, new PlayBackSpeedChangedEventArgs(int.Parse(e.EventArgs[0])));
					break;
				case "onPlayBackStarted":
					PlayBackStarted?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackStopped":
					PlayBackStopped?.Invoke(null, new EventArgs());
					break;
				case "onQueueNextItem":
					QueueNextItem?.Invoke(null, new EventArgs());
					break;
				default:
					PyConsole.WriteLine(string.Format("Unknown event '{0}' not handled", null));
					return false;
			}
			return true;
		}

		/// <summary>
		/// Handles a Kodi event message
		/// </summary>
		/// <param name="e">The event message to handle</param>
		/// <returns>true if the event was handled</returns>
		public static bool DispatchMonitorEvent(KodiEventMessage e) {
			switch (e.Source) {
				case "onAbortRequested":
					AbortRequested?.Invoke(null, new EventArgs());
					break;
				case "onCleanStarted":
					CleanStarted?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onCleanFinished":
					CleanFinished?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onDPMSActivated":
					DPMSActivated?.Invoke(null, new EventArgs());
					break;
				case "onDPMSDeactivated":
					DPMSDeactivated?.Invoke(null, new EventArgs());
					break;
				case "onDatabaseScanStarted":
					DatabaseScanStarted?.Invoke(null, new DatabaseEventArgs(e.EventArgs[0]));
					break;
				case "onDatabaseUpdated":
					DatabaseUpdated?.Invoke(null, new DatabaseEventArgs(e.EventArgs[0]));
					break;
				case "onNotification":
					Notification?.Invoke(null, new NotificationEventArgs(
						e.EventArgs[0], e.EventArgs[1], e.EventArgs[2]
					));
					break;
				case "onScanStarted":
					ScanStarted?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onScanFinished":
					ScanFinished?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onScreensaverActivated":
					ScreensaverActivated?.Invoke(null, new EventArgs());
					break;
				case "onScreensaverDeactivated":
					ScreensaverDeactivated?.Invoke(null, new EventArgs());
					break;
				case "onSettingsChanged":
					SettingsChanged?.Invoke(null, new EventArgs());
					break;
				default:
					PyConsole.WriteLine(string.Format("Unknown event '{0}' not handled", e.Source));
					return false;
			}
			return true;
		}
	}
}
