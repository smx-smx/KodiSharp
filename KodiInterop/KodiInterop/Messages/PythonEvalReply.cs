using Newtonsoft.Json;

namespace Smx.KodiInterop.Messages
{
	public class PythonEvalReply
    {
		[JsonProperty(PropertyName = "value")]
		public dynamic Value { get; set; }
		[JsonProperty(PropertyName = "exit_code")]
		public int ExitCode { get; set; }
		[JsonProperty(PropertyName = "error")]
		public string Error { get; set; }
	}
}
