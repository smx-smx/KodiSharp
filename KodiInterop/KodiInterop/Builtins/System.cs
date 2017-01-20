using System;
using System.Collections.Generic;
using System.Text;


namespace Smx.KodiInterop.Builtins
{
	/// <summary>
	/// Interacts with System Functionalities
	/// </summary>
    public class SystemBuiltins
    {
		/// <summary>
		/// Starts the screensaver
		/// </summary>
		[KodiMinApiVersion(13)]
		public static void ActivateScreensaver() {
			PythonInterop.CallBuiltin("ActivateScreensaver");
		}

		/// <summary>
		/// Allow the system to shutdown on idle.
		/// </summary>
		[KodiMinApiVersion(12)]
		public static void AllowIdleShutdown() {
			PythonInterop.CallBuiltin("AllowIdleShutdown");
		}

		/// <summary>
		/// Either opens or closes the DVD tray, depending on its current state
		/// </summary>
		public static void EjectTray() {
			PythonInterop.CallBuiltin("EjectTray");
		}

#if __ANDROID__
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
			string intent = null,
			string dataType = null,
			string dataUri = null
		) {
			List<string> arguments = new List<string> { package };
			if (intent != null)
				arguments.Add(intent);
			if (dataType != null)
				arguments.Add(dataType);
			if (dataType != null)
				arguments.Add(dataType);

			PythonInterop.CallBuiltin("StartAndroidActivity", arguments);
		}
#endif

		/// <summary>
		/// Execute shell commands.
		/// </summary>
		/// <param name="shellCommand"></param>
		public static void Exec(string shellCommand) {
			PythonInterop.CallBuiltin("System.Exec", new List<string> { shellCommand });
		}

		/// <summary>
		///	Execute shell commands and freezes Kodi until shell is closed.
		/// </summary>
		/// <param name="shellCommand"></param>
		public static void ExecWait(string shellCommand) {
			PythonInterop.CallBuiltin("System.ExecWait", new List<string> { shellCommand });
		}

		/// <summary>
		/// Log off current user.
		/// </summary>
		public static void LogOff() {
			PythonInterop.CallBuiltin("System.LogOff");
		}

		/// <summary>
		///	Trigger default Shutdown action defined in System Settings
		/// </summary>
		public static void ShutDown() {
			PythonInterop.CallBuiltin("Shutdown");
		}

		/// <summary>
		/// Cold reboots the system (power cycle)
		/// </summary>
		public static void Reboot() {
			PythonInterop.CallBuiltin("Reboot");
		}

		/// <summary>
		///	Reset the system (same as reboot)
		/// </summary>
		public static void Reset() {
			PythonInterop.CallBuiltin("Reset");
		}

		/// <summary>
		/// Suspends (S3 / S1 depending on bios setting) the System
		/// </summary>
		public static void Suspend() {
			PythonInterop.CallBuiltin("Suspend");
		}

		/// <summary>
		/// Hibernate (S4) the System
		/// </summary>
		public static void Hibernate() {
			PythonInterop.CallBuiltin("Hibernate");
		}

		/// <summary>
		/// Powerdown system
		/// </summary>
		public static void Powerdown() {
			PythonInterop.CallBuiltin("System.Powerdown");
		}

		/// <summary>
		/// Sends the wake-up packet to the broadcast address for the specified MAC address
		/// </summary>
		/// <param name="macAddress">MAC address (Format: FF:FF:FF:FF:FF:FF or FF-FF-FF-FF-FF-FF)</param>
		public static void WakeOnLan(string macAddress) {
			PythonInterop.CallBuiltin("System.WakeOnLan", new List<string> { macAddress });
		}
	}
}
