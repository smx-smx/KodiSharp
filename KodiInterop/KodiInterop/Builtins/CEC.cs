namespace Smx.KodiInterop.Builtins
{
	public static class CECBuiltins
    {
		/// <summary>
		/// Wake up playing device via a CEC peripheral
		/// </summary>
		[KodiMinApiVersion(13)]
		public static void ActivateSource() {
			PythonInterop.CallBuiltin("CECActivateSource");
		}

		/// <summary>
		/// Put playing device on standby via a CEC peripheral 
		/// </summary>
		[KodiMinApiVersion(13)]
		public static void Standby() {
			PythonInterop.CallBuiltin("CECStandby");
		}

		/// <summary>
		/// Toggle state of playing device via a CEC peripheral
		/// </summary>
		[KodiMinApiVersion(13)]
		public static void ToggleState() {
			PythonInterop.CallBuiltin("CECToggleState");
		}
	}
}
