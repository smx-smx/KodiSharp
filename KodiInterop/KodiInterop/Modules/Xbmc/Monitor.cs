using Smx.KodiInterop.Messages;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class Monitor : IDisposable
    {
		public readonly PyVariable Instance;
		
		public event EventHandler<EventArgs> AbortRequested;
		public event EventHandler<LibraryEventArgs> CleanStarted;
		public event EventHandler<LibraryEventArgs> CleanFinished;
		public event EventHandler<DatabaseEventArgs> DatabaseScanStarted;
		public event EventHandler<DatabaseEventArgs> DatabaseUpdated;
		public event EventHandler<EventArgs> DPMSActivated;
		public event EventHandler<EventArgs> DPMSDeactivated;
		public event EventHandler<EventArgs> ScreensaverActivated;
		public event EventHandler<EventArgs> ScreensaverDeactivated;
		public event EventHandler<LibraryEventArgs> ScanStarted;
		public event EventHandler<LibraryEventArgs> ScanFinished;
		public event EventHandler<EventArgs> SettingsChanged;
		public event EventHandler<NotificationEventArgs> Notification;

		private string[] eventNames = new string[] {
			"onAbortRequested", "onCleanStarted", "onCleanFinished",
			"onDPMSActivated", "onDPMSDeactivated", "onDatabaseScanStarted",
			"onDatabaseUpdated", "onNotification", "onScanStarted",
			"onScanFinished", "onScreensaverActivated", "onScreensaverDeactivated",
			"onSettingsChanged"
		};

		private bool onEvent(KodiEventMessage e) {
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

		public void Dispose() {
			Instance.Dispose();
		}

		public Monitor() {
			PyEventClassBuilder cb = new PyEventClassBuilder("xbmc.Monitor", typeof(Monitor));
			cb.Methods.AddRange(this.eventNames);
			cb.Install();
			this.Instance = cb.NewInstance(flags: PyVariableFlags.Monitor);

			// We now register this type so that PostEvent will be able to invoke onMessage in this class
			Console.WriteLine("=> Registering EventClass " + typeof(Monitor).FullName);
			KodiBridge.RegisterEventClass(typeof(Monitor), this);
		}
	}
}
