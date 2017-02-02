using System.Collections.Generic;

namespace Smx.KodiInterop.Builtins
{
	public static class WeatherBuiltins
    {
		/// <summary>
		/// Switch to next weather location
		/// </summary>
		public static void LocationNext() {
			PythonInterop.CallBuiltin("Weather.LocationNext");
		}

		/// <summary>
		/// Switch to previous weather location
		/// </summary>
		public static void LocationPrevious() {
			PythonInterop.CallBuiltin("Weather.LocationPrevious");
		}

		/// <summary>
		/// Switch to given weather location
		/// </summary>
		/// <param name="locationIndex">A number between 1 and 3</param>
		public static void LocationSet(int locationIndex) {
			PythonInterop.CallBuiltin("Weather.LocationSet", new List<object> { locationIndex });
		}

		/// <summary>
		/// Force weather data refresh
		/// </summary>
		public static void Refresh() {
			PythonInterop.CallBuiltin("Weather.Refresh");
		}
	}
}
