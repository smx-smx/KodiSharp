using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Smx.KodiInterop.Python.ValueConverters
{
    public static class StringConverter
    {
		public static string ToPythonCode(this string text, EscapeFlags flags = EscapeFlags.Quotes) {
			if (flags.HasFlag(EscapeFlags.Quotes)) {
				text = Regex.Replace(text, "\r?\n", "\\n");
				text = '"' + text.Replace("\"", "\\\"") + '"';
			}
			if (flags.HasFlag(EscapeFlags.EscapeBuiltin)) {
				text = Regex.Replace(text, ",", "\\,");
			}
			if (flags.HasFlag(EscapeFlags.RawString)) {
				text = "r'" + text + "'";
			}

			return text;
		}
    }
}
