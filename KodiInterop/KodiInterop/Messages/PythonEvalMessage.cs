using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Messages {
	public class PythonEvalMessage : RPCMessage {
		[JsonProperty(PropertyName = "code")]
		public string Code { get; set; }
	}
}
