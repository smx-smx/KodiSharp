using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python
{
	[Flags]
    public enum PyVariableFlags
    {
		Normal = 0,
		Object = 1 << 0,
		Monitor = 1 << 1,
		Player = 1 << 2
    }
}
