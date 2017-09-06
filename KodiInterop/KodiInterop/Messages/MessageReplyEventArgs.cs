using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class MessageReplyEventArgs : EventArgs
    {
		public string Reply { get; private set; }
		public MessageReplyEventArgs(string reply) {
			this.Reply = reply;
		}
    }
}
