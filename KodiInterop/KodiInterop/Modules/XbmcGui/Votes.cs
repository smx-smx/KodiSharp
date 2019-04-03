using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python.XbmcGui
{
	public enum Votes
	{
		[StringValue("imdb")]
		imdb,
		[StringValue("tvdb")]
		tvdb,
		[StringValue("tmdb")]
		tmdb,
		[StringValue("anidb")]
		anidb
	}
}
