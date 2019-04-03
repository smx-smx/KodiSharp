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
		
		public event EventHandler<EventArgs> OnAbortRequested;
		public event EventHandler<LibraryEventArgs> OnCleanStarted;
		public event EventHandler<LibraryEventArgs> OnCleanFinished;
		public event EventHandler<DatabaseEventArgs> OnDatabaseScanStarted;
		public event EventHandler<DatabaseEventArgs> OnDatabaseUpdated;
		public event EventHandler<EventArgs> OnDPMSActivated;
		public event EventHandler<EventArgs> OnDPMSDeactivated;
		public event EventHandler<EventArgs> OnScreensaverActivated;
		public event EventHandler<EventArgs> OnScreensaverDeactivated;
		public event EventHandler<LibraryEventArgs> OnScanStarted;
		public event EventHandler<LibraryEventArgs> OnScanFinished;
		public event EventHandler<EventArgs> OnSettingsChanged;
		public event EventHandler<NotificationEventArgs> OnNotification;

		public bool TriggerEvent(KodiEventMessage e) {
			switch (e.Source) {
				case "onAbortRequested":
					OnAbortRequested?.Invoke(null, new EventArgs());
					break;
				case "onCleanStarted":
					OnCleanStarted?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onCleanFinished":
					OnCleanFinished?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onDPMSActivated":
					OnDPMSActivated?.Invoke(null, new EventArgs());
					break;
				case "onDPMSDeactivated":
					OnDPMSDeactivated?.Invoke(null, new EventArgs());
					break;
				case "onDatabaseScanStarted":
					OnDatabaseScanStarted?.Invoke(null, new DatabaseEventArgs(e.EventArgs[0]));
					break;
				case "onDatabaseUpdated":
					OnDatabaseUpdated?.Invoke(null, new DatabaseEventArgs(e.EventArgs[0]));
					break;
				case "onNotification":
					OnNotification?.Invoke(null, new NotificationEventArgs(
						e.EventArgs[0], e.EventArgs[1], e.EventArgs[2]
					));
					break;
				case "onScanStarted":
					OnScanStarted?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onScanFinished":
					OnScanFinished?.Invoke(null, new LibraryEventArgs(e.EventArgs[0]));
					break;
				case "onScreensaverActivated":
					OnScreensaverActivated?.Invoke(null, new EventArgs());
					break;
				case "onScreensaverDeactivated":
					OnScreensaverDeactivated?.Invoke(null, new EventArgs());
					break;
				case "onSettingsChanged":
					OnSettingsChanged?.Invoke(null, new EventArgs());
					break;
				default:
					PyConsole.WriteLine(string.Format("Unknown event '{0}' not handled", e.Source));
					return false;
			}
			return true;
		}

		public void Dispose() {
			KodiBridge
				.RunningAddon
				.Bridge
				.UnregisterEventClass(this);
			Instance.Dispose();
		}

		public bool AbortRequested {
			get => Convert.ToBoolean(Instance.CallFunction("abortRequested"));
		}

		public bool WaitForAbort(TimeSpan timeSpan) {
			return Convert.ToBoolean(Instance.CallFunction("waitForAbort", new List<object> {
				(ulong)timeSpan.TotalSeconds
			}));
		}

		public XbmcMonitor() {
            this.Instance = PyVariableManager.Get.NewVariable(evalCode: "self.new_monitor()");

			// We now register this type so that PostEvent will be able to invoke onMessage in this class
			Console.WriteLine("=> Registering EventClass " + typeof(XbmcMonitor).FullName);
			KodiBridge
				.RunningAddon
				.Bridge
				.RegisterMonitor(this);
		}
	}
}
