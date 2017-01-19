using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
    class Control
    {
		/// <summary>
		/// Restarts Kodi (only implemented under Windows and Linux)
		/// </summary>
		public static void RestartApp() {
			PythonInterop.CallBuiltin("RestartApp");
		}

		/// <summary>
		/// Quits Kodi
		/// </summary>
		public static void Quit() {
			PythonInterop.CallBuiltin("Quit");
		}

		/// <summary>
		///	Mutes (or unmutes) the volume.
		/// </summary>
		public static void Mute() {
			PythonInterop.CallBuiltin("Mute");
		}

		/// <summary>
		/// Minimizes Kodi
		/// </summary>
		public static void Minimize() {
			PythonInterop.CallBuiltin("Minimize");
		}

		/// <summary>
		/// Send a page down event to the pagecontrol with given id.
		/// </summary>
		public static void PageDown() {
			PythonInterop.CallBuiltin("PageDown");
		}

		/// <summary>
		///	Send a page up event to the pagecontrol with given id.
		/// </summary>
		public static void PageUp() {
			PythonInterop.CallBuiltin("PageUp");
		}


		/// <summary>
		/// Will play the inserted CD or DVD media from the DVD-ROM drive.
		/// </summary>
		public static void PlayDVD() {
			PythonInterop.CallBuiltin("PlayDVD");
		}

		/// <summary>
		/// Reload RSS feeds from RSSFeeds.xml
		/// </summary>
		public static void RefreshRSS() {
			PythonInterop.CallBuiltin("RefreshRSS");
		}
	}
}
