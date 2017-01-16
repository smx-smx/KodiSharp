using Smx.KodiInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
	public class Notification
	{
		public string Header;
		public string Message;
		public TimeSpan? Duration = null;
		public string IconPath;

		public void Show() {
			ShowNotification(this.Header, this.Message, this.Duration, this.IconPath);
		}

		public static void ShowNotification(string header, string message, TimeSpan? duration = null, string iconPath = null) {
			List<string> arguments = new List<string> { header, message };
			if (duration != null)
				arguments.Add(duration.Value.TotalMilliseconds.ToString());
			if (iconPath != null)
				arguments.Add(iconPath);

			PythonInterop.CallBuiltin("Notification", arguments);
		}
	}
}
