using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
	public static class ApplicationBuiltins
	{
		/// <summary>
		/// Extracts a specified archive to an optionally specified 'absolute' path. 
		/// </summary>
		/// <param name="url">The archive URL.</param>
		/// <param name="dest">Destination path (optional). </param>
		public static void Extract(string url, string? dest = null) {
			PythonInterop.CallBuiltin("Extract", url, dest);
		}

		/// <summary>
		/// Mutes (or unmutes) the volume. 
		/// </summary>
		public static void Mute() {
			PythonInterop.CallBuiltin("Mute");
		}

		/// <summary>
		/// Notify all connected clients 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		/// <param name="json"></param>
		public static void NotifyAll(string sender, string data, string? json = null) {
			PythonInterop.CallBuiltin("NotifyAll", sender, data, json);
		}

		/// <summary>
		/// Sets the volume to the percentage specified. Optionally, show the Volume Dialog in Kodi when setting the volume. 
		/// </summary>
		/// <param name="percent">Volume level</param>
		/// <param name="showVolumeBar">Show the volume bar</param>
		public static void SetVolume(int percent, bool showVolumeBar = false) {
			PythonInterop.CallBuiltin("SetVolume", percent, (showVolumeBar) ? "showVolumeBar" : null);
		}

		/// <summary>
		/// Toggle DPMS mode manually 
		/// </summary>
		public static void ToggleDPMS() {
			PythonInterop.CallBuiltin("ToggleDPMS");
		}
	}
}
