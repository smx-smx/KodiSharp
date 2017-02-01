using System;
using System.Collections.Generic;
using System.Text;

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
