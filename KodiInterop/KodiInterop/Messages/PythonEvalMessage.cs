using Newtonsoft.Json;

namespace Smx.KodiInterop.Messages
{
	public class PythonEvalMessage : RPCMessage {
		[JsonProperty(PropertyName = "exec_code")]
		public string Code { get; set; }
		
		public PythonEvalMessage() {
			this.MessageType = "eval";
		}
	}
}
