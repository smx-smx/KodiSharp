using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class PythonVarArgsMessage : RPCMessage
    {
		[JsonProperty(PropertyName = "args")]
		public List<string> args { get; set; }

		[JsonProperty(PropertyName = "kwargs")]
		public Dictionary<string, string> kwargs { get; set; }
	}
}
