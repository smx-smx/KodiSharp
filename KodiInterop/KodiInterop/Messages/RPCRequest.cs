using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class RPCRequest
    {
		public RPCMessage Request { get; private set; }
		public string Reply { get; private set; }

		public event EventHandler<MessageReplyEventArgs> OnReply;

		public RPCRequest(RPCMessage message) {
			this.Request = Request;
		}

		public string Send() {
			Reply = KodiBridge.SendMessage(this.Request);
			OnReply?.Invoke(this, new MessageReplyEventArgs(Reply));
			return Reply;
		}
    }
}
