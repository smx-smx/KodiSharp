using System;
using System.Collections.Generic;

namespace Smx.KodiInterop.Builtins
{
	public static class UiBuiltins
    {
		/// <summary>
		/// Will display a notification dialog with the specified header and message
		/// </summary>
		/// <param name="header"></param>
		/// <param name="message"></param>
		/// <param name="duration">length of time in milliseconds</param>
		/// <param name="iconPath">icon image</param>
		public static void Notification(string header = "", string message = "", TimeSpan? duration = null, string iconPath = null) {
			List<string> arguments = new List<string> { header, message };
			if (duration != null)
				arguments.Add(duration.Value.TotalMilliseconds.ToString());
			if (iconPath != null)
				arguments.Add(iconPath);

			PythonInterop.CallBuiltin("Notification", arguments);
		}
	}
}
