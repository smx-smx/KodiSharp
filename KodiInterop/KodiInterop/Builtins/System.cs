using System.Collections.Generic;


namespace Smx.KodiInterop.Builtins
{
	/// <summary>
	/// Interacts with System Functionalities
	/// </summary>
	public static class SystemBuiltins
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

		/// <summary>
		/// Execute shell commands.
		/// </summary>
		/// <param name="shellCommand"></param>
		public static void Exec(string shellCommand) {
			PythonInterop.CallBuiltin("System.Exec", shellCommand);
		}

		/// <summary>
		///	Execute shell commands and freezes Kodi until shell is closed.
		/// </summary>
		/// <param name="shellCommand"></param>
		public static void ExecWait(string shellCommand) {
			PythonInterop.CallBuiltin("System.ExecWait", shellCommand);
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
		/// Enables/disables debug mode
		/// </summary>
		[KodiMinApiVersion(12)]
		public static void ToggleDebug() {
			PythonInterop.CallBuiltin("ToggleDebug");
		}

		/// <summary>
		/// Takes a Screenshot. Only .png files are supported
		/// </summary>
		/// <param name="filenameAndPath">filename (including the path)</param>
		/// <param name="sync">whether to run synchronously</param>
		public static void TakeScreenshot(string filenameAndPath = null, bool sync = false) {
			PythonInterop.CallBuiltin("TakeScreenshot", filenameAndPath, sync);
		}

		/// <summary>
		/// Sends the wake-up packet to the broadcast address for the specified MAC address
		/// </summary>
		/// <param name="macAddress">MAC address (Format: FF:FF:FF:FF:FF:FF or FF-FF-FF-FF-FF-FF)</param>
		public static void WakeOnLan(string macAddress) {
			PythonInterop.CallBuiltin("System.WakeOnLan", macAddress);
		}
	}
}
