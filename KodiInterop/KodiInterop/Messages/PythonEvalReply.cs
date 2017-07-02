using Newtonsoft.Json;

namespace Smx.KodiInterop.Messages
{
	public class PythonEvalReply
    {
		[JsonProperty(PropertyName = "type")]
		public string TypeName { get; set; }
		[JsonProperty(PropertyName = "value")]
		public dynamic Value { get; set; }
		[JsonProperty(PropertyName = "exit_code")]
		public int ExitCode { get; set; }
	}
}
