using Smx.KodiInterop.Messages;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class XbmcMonitor : IDisposable, IKodiEventConsumer
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

		public bool TriggerEvent(KodiEventMessage e) {
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

		public void Dispose() {
			KodiBridge.RunningAddon.Bridge.UnregisterEventClass(this);
			Instance.Dispose();
		}

		public XbmcMonitor() {
            this.Instance = PyVariableManager.Get.NewVariable(evalCode: "self.new_monitor()");

			// We now register this type so that PostEvent will be able to invoke onMessage in this class
			Console.WriteLine("=> Registering EventClass " + typeof(XbmcMonitor).FullName);
			KodiBridge.RunningAddon.Bridge.RegisterMonitor(this);
		}
	}
}
