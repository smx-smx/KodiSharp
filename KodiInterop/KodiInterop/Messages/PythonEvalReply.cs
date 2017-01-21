using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Messages
{
    public class PythonEvalReply
    {
		[JsonProperty(PropertyName = "result")]
		public string Result { get; set; }
		[JsonProperty(PropertyName = "exit_code")]
		public int ExitCode { get; set; }
	}
}
