using Smx.KodiInterop;
using Smx.KodiInterop.Messages;
using Smx.KodiInterop.Modules.Xbmc;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public static class Events
	{
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

		/// <summary>
		/// Handles a Kodi event message
		/// </summary>
		/// <param name="e">The event message to handle</param>
		/// <returns>true if the event was handled</returns>
		public static bool DispatchEvent(KodiEventMessage e) {
			switch (e.Sender) {
				case "onAbortRequested":
					AbortRequested?.Invoke(e.Sender, new EventArgs());
					break;
				case "onCleanStarted":
					CleanStarted?.Invoke(e.Sender, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onCleanFinished":
					CleanFinished?.Invoke(e.Sender, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onDPMSActivated":
					DPMSActivated?.Invoke(e.Sender, new EventArgs());
					break;
				case "onDPMSDeactivated":
					DPMSDeactivated?.Invoke(e.Sender, new EventArgs());
					break;
				case "onDatabaseScanStarted":
					DatabaseScanStarted?.Invoke(e.Sender, new DatabaseEventArgs(e.EventArgs[0]));
					break;
				case "onDatabaseUpdated":
					DatabaseUpdated?.Invoke(e.Sender, new DatabaseEventArgs(e.EventArgs[0]));
					break;
				case "onNotification":
					Notification?.Invoke(e.Sender, new NotificationEventArgs(
						e.EventArgs[0], e.EventArgs[1], e.EventArgs[2]
					));
					break;
				case "onScanStarted":
					ScanStarted?.Invoke(e.Sender, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onScanFinished":
					ScanFinished?.Invoke(e.Sender, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onScreensaverActivated":
					ScreensaverActivated?.Invoke(e.Sender, new EventArgs());
					break;
				case "onScreensaverDeactivated":
					ScreensaverDeactivated?.Invoke(e.Sender, new EventArgs());
					break;
				case "onSettingsChanged":
					SettingsChanged?.Invoke(e.Sender, new EventArgs());
					break;
				default:
					PyConsole.WriteLine(string.Format("Unknown event '{0}' not handled", e.Sender));
					return false;
			}
			return true;
		}
	}
}
