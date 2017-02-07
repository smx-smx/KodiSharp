using Smx.KodiInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Xbmc
{
    public enum PlayListType
    {
		[StringValue("xbmc.PLAYLIST_MUSIC")]
		Music = 0,
		[StringValue("xbmc.PLAYLIST_VIDEO")]
		Video = 1
    }
}
