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
		public static event EventHandler<EventArgs> CleanStarted;
		public static event EventHandler<EventArgs> CleanFinished;
		public static event EventHandler<EventArgs> DPMSActivated;
		public static event EventHandler<EventArgs> DPMSDeactivated;
		public static event EventHandler<EventArgs> ScreensaverActivated;
		public static event EventHandler<EventArgs> ScreensaverDeactivated;

		public static event EventHandler<EventArgs> ScanStarted;
		public static event EventHandler<EventArgs> ScanFinished;
		public static event EventHandler<EventArgs> SettingsChanged;
		public static event EventHandler<EventArgs> Notification;

		/// <summary>
		/// Handles a Kodi event message
		/// </summary>
		/// <param name="e">The event message to handle</param>
		/// <returns>true if the event was handled</returns>
		public static bool DispatchEvent(KodiEventMessage e) {
			switch (e.Sender) {
				case "onAbortRequested":
					AbortRequested?.Invoke(e.Sender, null);
					break;
				case "onCleanStarted":
					CleanStarted?.Invoke(e.Sender, null);
					break;
				case "onCleanFinished":
					CleanFinished?.Invoke(e.Sender, null);
					break;
				case "onDPMSActivated":
					DPMSActivated?.Invoke(e.Sender, null);
					break;
				case "onDPMSDeactivated":
					DPMSDeactivated?.Invoke(e.Sender, null);
					break;
				/*
				// deprecated?
				case "onDatabaseScanStarted":
					break;
				case "onDatabaseUpdated":
					break;
				*/
				case "onNotification":
					Notification?.Invoke(e.Sender, null);
					break;
				case "onScanStarted":
					ScanStarted?.Invoke(e.Sender, null);
					break;
				case "onScanFinished":
					ScanFinished?.Invoke(e.Sender, null);
					break;
				case "onScreensaverActivated":
					ScreensaverActivated?.Invoke(e.Sender, null);
					break;
				case "onScreensaverDeactivated":
					ScreensaverDeactivated?.Invoke(e.Sender, null);
					break;
				case "onSettingsChanged":
					SettingsChanged?.Invoke(e.Sender, null);
					break;
				default:
					PyConsole.WriteLine(string.Format("Unknown event '{0}' not handled", e.Sender));
					return false;
			}
			return true;
		}
	}
}
