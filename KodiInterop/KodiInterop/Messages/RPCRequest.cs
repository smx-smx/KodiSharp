using System;
using System.Collections.Generic;
using System.Text;
using static Smx.KodiInterop.KodiBridgeInstance;

namespace Smx.KodiInterop.Messages
{
    public class RPCRequest
    {
		public RPCMessage Request { get; private set; }
		public string Reply { get; private set; }

		public RPCRequest(RPCMessage message) {
			this.Request = Request;
		}
    }
}
