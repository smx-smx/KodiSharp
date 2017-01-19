using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
    class Skin
    {
		/// <summary>
		///	Resets the specified skin setting.
		///	If it's a bool value (i.e. set via SetBool or ToggleSetting) then the setting is reset to false.
		///	If it's a string (Set via SetString, SetImage, or SetPath) then it is set to empty.
		/// </summary>
		/// <param name="setting">Name of the setting to reset</param>
		public static void Reset(string setting) {
			PythonInterop.CallBuiltin("Skin.Reset", new List<string> { setting });
		}

		/// <summary>
		/// Resets all the above skin settings to their defaults (toggles all set to false, strings all set to empty.)
		/// </summary>
		public static void ResetSettings() {
			PythonInterop.CallBuiltin("Skin.ResetSettings");
		}

		/// <summary>
		/// Pops up a select dialog and allows the user to select an add-on of the given type
		/// to be used elsewhere in the skin via the info tag Skin.String(string).
		/// The most common types are xbmc.addon.video, xbmc.addon.audio, xbmc.addon.image and xbmc.addon.executable.
		/// </summary>
		public static void SetAddon(string itemName, string typeName) {
			PythonInterop.CallBuiltin("Skin.SetAddon", new List<string> {
				itemName, typeName
			});
		}

		/// <summary>
		/// Sets the skin setting ?setting? to true, for use with the conditional visibility tags containing Skin.HasSetting(setting).
		/// The settings are saved per-skin in settings.xml just like all the other Kodi settings.	
		/// </summary>
		/// <param name="setting"></param>
		public static void SetBool(string setting) {
			PythonInterop.CallBuiltin("Skin.SetBool", new List<string> { setting });
		}

		/// <summary>
		/// Pops up a folder browser and allows the user to select a file off the hard-disk to be used else where in the skin
		/// via the info tag Skin.String(string).
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="mask">If specified the file browser will only search for the extension specified (.avi,.mp3,.m3u,.png,.bmp,etc.,etc.).
		/// To use multiple extensions separate them using "|" (minus quotes)</param>
		/// <param name="folderpath">If specified the file browser will start in that folder</param>
		public static void SetFile(string setting, string mask = null, string folderpath = null) {
			List<string> arguments = new List<string> { setting };
			if (mask != null)
				arguments.Add(mask);
			if (folderpath != null)
				arguments.Add(folderpath);
			PythonInterop.CallBuiltin("Skin.SetFile", arguments);
		}

		/// <summary>
		/// Pops up a folder browser and allows the user to select a file off the hard-disk to be used else where in the skin
		/// via the info tag Skin.String(string).
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="masks"></param>
		/// <param name="folderpath"></param>
		public static void SetFile(string setting, List<string> masks = null, string folderpath = null) {
			SetFile(setting, string.Join("|", masks.ToArray()), folderpath);
		}


		/// <summary>
		/// Pops up a file browser and allows the user to select an image file to be used in an image control elsewhere in the skin
		/// via the info tag Skin.String(string).
		/// </summary>
		/// <param name="value">If specified the keyboard dialog does not pop up and the image path is set directly.</param>
		/// <param name="folderpath">If specified the file browser will start in that folder</param>
		public static void SetImage(string setting, object value = null, string folderpath = null) {
			List<object> arguments = new List<object> { setting };
			if (value != null)
				arguments.Add(value);
			if (folderpath != null)
				arguments.Add(folderpath);

			PythonInterop.CallBuiltin("Skin.SetImage", arguments);
		}

		/// <summary>
		///	Pops up a keyboard dialog and allows the user to input a numerical.
		/// </summary>
		/// <param name="value">If specified the keyboard dialog does not pop up and the numeric value is set directly.</param>
		public static void SetNumeric(object number, object value) {
			List<object> arguments = new List<object> { number };
			if (value != null)
				arguments.Add(value);
			PythonInterop.CallBuiltin("Skin.SetNumeric", arguments);
		}

		/// <summary>
		/// Pops up a folder browser and allows the user to select a folder of images to be used in a multi image control else where in the skin
		/// via the info tag Skin.String(string).
		/// </summary>
		/// <param name="folderpath">If specified the file browser will start in that folder</param>
		public static void SetPath(string setting, string folderpath = null) {
			List<string> arguments = new List<string> { setting };
			if (folderpath != null)
				arguments.Add(folderpath);

			PythonInterop.CallBuiltin("Skin.SetPath", arguments);
		}

		/// <summary>
		/// Pops up a keyboard dialog and allows the user to input a string which can be used in a label control elsewhere in the skin
		/// via the info tag Skin.String(string)
		/// </summary>
		/// <param name="value">If specified the keyboard dialog does not pop up and the string is set directly.</param>
		public static void SetString(string setting, object value = null) {
			List<object> arguments = new List<object> { setting };
			if(value != null) {
				arguments.Add(value);
			}
			PythonInterop.CallBuiltin("Skin.SetString", arguments);
		}

		/// <summary>
		///	Cycles the skin theme. Skin.Theme(-1) will go backwards.
		/// </summary>
		public static void Theme(int direction) {
			PythonInterop.CallBuiltin("Skin.Theme");
		}

		/// <summary>
		///	Toggles the skin setting ?setting? for use with conditional visibility tags containing Skin.HasSetting(setting).
		/// </summary>
		public static void ToggleSetting(string setting) {
			PythonInterop.CallBuiltin("Skin.ToggleSetting", new List<string> { setting });
		}

		/// <summary>
		/// Toggles skin debug info on/off
		/// </summary>
		public static void ToggleDebug() {
			PythonInterop.CallBuiltin("Skin.ToggleDebug");
		}
	}
}
