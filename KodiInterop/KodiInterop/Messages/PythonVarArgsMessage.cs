using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class PythonVarArgsMessage : RPCMessage
    {
		[JsonProperty(PropertyName = "args")]
		public required List<string> args { get; set; }

		[JsonProperty(PropertyName = "kwargs")]
		public required Dictionary<string, string> kwargs { get; set; }
	}
}
