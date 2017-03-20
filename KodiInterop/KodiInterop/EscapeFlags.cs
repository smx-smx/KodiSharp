using System;

namespace Smx.KodiInterop
{
	[Flags]
    public enum EscapeFlags
    {
		/// <summary>
		/// No escaping, value is passed from toString() as-is
		/// </summary>
		None = 0,
		/// <summary>
		/// Escapes using double quotes
		/// </summary>
		Quotes = 1 << 0,
		/// <summary>
		/// Value is escaped as a python literal. Special characters are ignored
		/// </summary>
		RawString = 1 << 1,
		/// <summary>
		/// Escapes "," in Builtin calls
		/// </summary>
		EscapeBuiltin = 1 << 2, //to escape ","
		/// <summary>
		/// Removes null arguments from right until a non null element is found
		/// </summary>
		StripNullItems = 1 << 3
    }
}
