using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class KodiEventMessage : RPCMessage
    {
		[JsonProperty(PropertyName = "source")]
		public required string Source { get; set; }

        [JsonProperty(PropertyName = "sender")]
        public required string Sender { get; set; }

        [JsonProperty(PropertyName = "args")]
		public required List<string> EventArgs { get; set; }

		[JsonProperty(PropertyName = "kwargs")]
		public required Dictionary<string, string> ExtraArgs { get; set; }
	}
}
