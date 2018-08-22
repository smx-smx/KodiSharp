using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.XbmcAddon
{
    public class Addon
    {
		public readonly PyVariable Instance = PyVariableManager.Get.NewVariable();

		private static PythonFunction _ctor = new PythonFunction(PyModule.XbmcAddon, "Addon");
		private static PythonFunction _getAddonInfo = PythonFunction.ClassFunction("getAddonInfo");

		private static PythonFunction _getSetting = PythonFunction.ClassFunction("getSetting");
		private static PythonFunction _getSettingBool = PythonFunction.ClassFunction("getSettingBool");
		private static PythonFunction _getSettingInt = PythonFunction.ClassFunction("getSettingInt");
		private static PythonFunction _getSettingNumber = PythonFunction.ClassFunction("getSettingNumber");
		private static PythonFunction _getSettingString = PythonFunction.ClassFunction("getSettingString");

		private static PythonFunction _setSetting = PythonFunction.ClassFunction("setSetting");
		private static PythonFunction _setSettingBool = PythonFunction.ClassFunction("setSettingBool");
		private static PythonFunction _setSettingInt = PythonFunction.ClassFunction("setSettingInt");
		private static PythonFunction _setSettingNumber = PythonFunction.ClassFunction("setSettingNumber");
		private static PythonFunction _setSettingString = PythonFunction.ClassFunction("setSettingString");

		private static PythonFunction _openSettings = PythonFunction.ClassFunction("openSettings");
		private static PythonFunction _getLocalizedString = PythonFunction.ClassFunction("getLocalizedString");

		/// <summary>
		/// Creates a new AddOn class.
		/// </summary>
		/// <param name="id">id of the addon as specified in addon.xml.
		/// If not specified, the running addon is used</param>
		public Addon(string id = null) {
			Instance.CallAssign(_ctor, id);
		}

		/// <summary>
		/// Returns the value of an addon property as a string.
		/// </summary>
		/// <param name="info">the property that the module needs to access.</param>
		/// <returns></returns>
		public string GetAddonInfo(AddonInfo info) {
			return Instance.CallFunction(_getAddonInfo, info.GetString());
		}

		/// <summary>
		/// Returns the value of a setting as a unicode string.
		/// </summary>
		/// <param name="setting">the setting that the module needs to access.</param>
		/// <returns></returns>
		public string GetSetting(string setting) {
			return Instance.CallFunction(_getSetting, setting);
		}

		public T GetSetting<T>(string setting) where T : IConvertible {
			PythonFunction func;

			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.Boolean:
					func = _getSettingBool;
					break;
				case TypeCode.Decimal:
					func = _getSettingInt;
					break;
				case TypeCode.Double:
					func = _getSettingNumber;
					break;
				case TypeCode.String:
					func = _getSettingString;
					break;
				default:
					func = _getSetting;
					break;

			}
			string value = Instance.CallFunction(func, new List<object>{ setting });
			return (T)Convert.ChangeType(value, typeof(T));
		}

	    public void SetSetting<T>(string setting, object value) where T : IConvertible {
		    PythonFunction func;

		    switch (Type.GetTypeCode(typeof(T))) {
			    case TypeCode.Boolean:
				    func = _setSettingBool;
				    break;
			    case TypeCode.Decimal:
					func = _setSettingInt;
				    break;
			    case TypeCode.Double:
				    func = _setSettingNumber;
				    break;
			    case TypeCode.String:
				    func = _getSettingString;
				    break;
				default:
					func = _setSetting;
					break;
		    }

		    Instance.CallFunction(func, new List<object> { setting, value });
	    }

	    /// <summary>
		/// Sets a script setting.
		/// </summary>
		/// <param name="setting">the setting that the module needs to access.</param>
		/// <param name="value">value of the setting.</param>
		public void SetSetting(string setting, object value) {
			Instance.CallFunction(_setSetting, setting, value);
		}

		/// <summary>
		/// Opens this scripts settings dialog.
		/// </summary>
		public void OpenSettings() {
			Instance.CallFunction(_openSettings);
		}

		/// <summary>
		/// Returns an addon's localized 'unicode string'.
		/// </summary>
		/// <param name="id">id# for string you want to localize.</param>
		/// <returns>Localized 'unicode string'</returns>
		public string GetLocalizedString(int id) {
		    return Instance.CallFunction(_getLocalizedString, id);
	    }
    }
}
