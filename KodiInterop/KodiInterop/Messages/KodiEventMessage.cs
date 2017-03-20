using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class KodiEventMessage : RPCMessage
    {
		[JsonProperty(PropertyName = "sender")]
		public string Sender { get; set; }

		[JsonProperty(PropertyName = "args")]
		public List<string> EventArgs { get; set; }

		[JsonProperty(PropertyName = "kwargs")]
		public Dictionary<string, string> ExtraArgs { get; set; }
	}
}
