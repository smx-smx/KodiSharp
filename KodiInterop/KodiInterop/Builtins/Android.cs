using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
	public static class AndroidBuiltins
	{
		/// <summary>
		/// Launch an Android native app with the given package name
		/// </summary>
		/// <param name="package"></param>
		/// <param name="intent"></param>
		/// <param name="dataType"></param>
		/// <param name="dataUri"></param>
		[KodiMinApiVersion(13)]
		public static void StartAndroidActivity(
			string package,
			string? intent = null,
			string? dataType = null,
			string? dataUri = null
		) {
			PythonInterop.CallBuiltin("StartAndroidActivity", package, intent, dataType, dataUri);
		}
	}
}
