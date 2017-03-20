using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class NotificationEventArgs : EventArgs
	{
		/// <summary>
		/// Sender of the notification
		/// </summary>
		public string Sender { get; private set; }
		/// <summary>
		/// Name of the notification
		/// </summary>
		public string Method { get; private set; }
		/// <summary>
		/// JSON-encoded data of the notification
		/// </summary>
		public string Data { get; private set; }

		public NotificationEventArgs(string sender = null, string method = null, string data = null) {
			this.Sender = sender;
			this.Method = method;
			this.Data = data;
		}
    }
}
