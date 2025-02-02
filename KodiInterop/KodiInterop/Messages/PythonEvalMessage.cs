using Newtonsoft.Json;

namespace Smx.KodiInterop.Messages
{
	public class PythonEvalMessage : RPCMessage {
		[JsonProperty(PropertyName = "exec_code")]
		public required string Code { get; set; }

		public static PythonEvalMessage Create(string code) {
			return new PythonEvalMessage {
				MessageType = "eval",
				Code = code
			};
		}
	}
}
