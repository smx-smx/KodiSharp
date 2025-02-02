using Newtonsoft.Json;

namespace Smx.KodiInterop.Messages
{
	public class PythonEvalReply
    {
		[JsonProperty(PropertyName = "value")]
		public required dynamic Value { get; set; }
		[JsonProperty(PropertyName = "exit_code")]
		public required int ExitCode { get; set; }
		[JsonProperty(PropertyName = "error")]
		public required string Error { get; set; }
	}
}
