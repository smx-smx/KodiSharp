﻿using Newtonsoft.Json;

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
