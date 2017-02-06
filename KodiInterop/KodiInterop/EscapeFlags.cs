using System;

namespace Smx.KodiInterop
{
	[Flags]
    public enum EscapeFlags
    {
		None = 0,
		Quotes = 1 << 0,
		RawString = 1 << 1,
		EscapeBuiltin = 1 << 2, //to escape ","
		StripNullItems = 1 << 3
    }
}
