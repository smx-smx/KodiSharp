using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    class PythonDeleteVariableMessage : RPCMessage
	{
		[JsonProperty(PropertyName = "var_name")]
		public string VariableName { get; set; }

		public PythonDeleteVariableMessage() {
			this.MessageType = "del_var";
		}
	}
}
