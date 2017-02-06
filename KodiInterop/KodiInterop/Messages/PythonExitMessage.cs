using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class PythonExitMessage : RPCMessage
    {
		/// <summary>
		/// Unused for now, for future PluginLoader
		/// </summary>
		[JsonProperty(PropertyName = "unload_dll")]
		public bool UnloadDLL { get; set; }

		public PythonExitMessage() {
			this.UnloadDLL = false;
			this.MessageType = "exit";
		}
    }
}
