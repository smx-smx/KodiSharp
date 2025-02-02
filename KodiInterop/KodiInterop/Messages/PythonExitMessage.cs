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
		public required bool UnloadDLL { get; set; }

		public static PythonExitMessage Create(bool unloadDll = false){
			return new PythonExitMessage {
				MessageType = "exit",
				UnloadDLL = unloadDll
			};
		}
    }
}
