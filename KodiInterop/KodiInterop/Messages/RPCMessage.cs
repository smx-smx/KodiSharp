using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Messages
{
	public class RPCMessage
	{
		//public string PluginName;
		//public string MessageType;
		[JsonProperty(PropertyName = "type")]
		public string MessageType { get; set; }
	}
}
