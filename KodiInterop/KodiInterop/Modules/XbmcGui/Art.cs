using Smx.KodiInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.XbmcGui
{
    public enum Art
    {
		[StringValue("thumb")]
		Thumb = 0,
		[StringValue("poster")]
		Poster,
		[StringValue("banner")]
		Banner,
		[StringValue("fanart")]
		Fanart,
		[StringValue("clearart")]
		Clearart,
		[StringValue("clearlogo")]
		Clearlogo,
		[StringValue("landscape")]
		Landscape,
		[StringValue("icon")]
		Icon
	}
}
