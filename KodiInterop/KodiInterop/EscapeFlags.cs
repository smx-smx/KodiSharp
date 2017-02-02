using System;

namespace Smx.KodiInterop
{
	[Flags]
    public enum EscapeFlags
    {
		None = 0,
		Quotes,
		RawString,
		StripNullItems
    }
}
