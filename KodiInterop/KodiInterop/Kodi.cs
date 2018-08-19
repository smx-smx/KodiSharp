using Smx.KodiInterop.Modules.Xbmc;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop
{
    public static class Kodi
    {
		/// <summary>
		/// The current DVD drive state
		/// </summary>
		public static DVDState DVDState {
			get {
				return (DVDState)Convert.ToInt32(PythonInterop.CallFunction(
					PyModule.Xbmc, "getDVDState"
				));
			}
		}
		/// <summary>
		/// Amount of free memory in MB.
		/// </summary>
		public static int FreeMem {
			get {
				return Convert.ToInt32(PythonInterop.CallFunction(
					PyModule.Xbmc, "getFreeMem"
				));
			}
		}

		/// <summary>
		/// The elapsed idle time in seconds.
		/// </summary>
		public static int GlobalIdleTime {
			get {
				return Convert.ToInt32(PythonInterop.CallFunction(
					PyModule.Xbmc, "getGlobalIdleTime"
				));
			}
		}

		public static string IPAddress {
			get {
				return PythonInterop.CallFunction(
					PyModule.Xbmc, "getIPAddress"
				);
			}
		}

		public static string SkinDir {
			get {
				return PythonInterop.CallFunction(
					PyModule.Xbmc, "getSkinDir"
				);
			}
		}

		public static string UserAgent {
			get {
				return PythonInterop.CallFunction(
					PyModule.Xbmc, "getUserAgent"
				);
			}
		}

		public static void AudioResume() {
			PythonInterop.CallFunction(
				PyModule.Xbmc, "audioResume"
			);
		}

		public static void AudioSuspend() {
			PythonInterop.CallFunction(
				PyModule.Xbmc, "audioSuspend"
			);
		}

		public static void Sleep(TimeSpan time) {
			/*PythonInterop.CallFunction(
				new PythonFunction(PyModule.Xbmc, "sleep"),
				new List<object> { 10000 }
			);*/
			PythonInterop.CallFunction(
				PyModule.Xbmc, "sleep", new List<object> {
					(ulong)time.TotalMilliseconds
				}
			);
		}

		public static bool StartServer(ServerType type, bool bStart, bool? bWait = null) {
			string typeString = PyModule.Xbmc.GetString() + type.GetString();
			return Convert.ToBoolean(PythonInterop.CallFunction(
				PyModule.Xbmc, "startServer", new List<object> {
					typeString, bStart, bWait
				}
			));
		}

		public static void PlaySFX(string filename, bool? useCached = null) {
			PythonInterop.CallFunction(
				PyModule.Xbmc, "playSFX", new List<object> {
					filename, useCached
				}
			);
		}
    }
}
