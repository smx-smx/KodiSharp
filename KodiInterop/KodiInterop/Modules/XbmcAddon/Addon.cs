using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Modules.XbmcAddon
{
    public class Addon : IDisposable
    {
		public readonly PyVariable Instance = PyVariableManager.Get.NewVariable();

		private static PyFunction _ctor = new PyFunction(PyModule.XbmcAddon, "Addon");
		private static PyFunction _getAddonInfo = PyFunction.ClassFunction("getAddonInfo");

		private static PyFunction _getSetting = PyFunction.ClassFunction("getSetting");
		private static PyFunction _getSettingBool = PyFunction.ClassFunction("getSettingBool");
		private static PyFunction _getSettingInt = PyFunction.ClassFunction("getSettingInt");
		private static PyFunction _getSettingNumber = PyFunction.ClassFunction("getSettingNumber");
		private static PyFunction _getSettingString = PyFunction.ClassFunction("getSettingString");

		private static PyFunction _setSetting = PyFunction.ClassFunction("setSetting");
		private static PyFunction _setSettingBool = PyFunction.ClassFunction("setSettingBool");
		private static PyFunction _setSettingInt = PyFunction.ClassFunction("setSettingInt");
		private static PyFunction _setSettingNumber = PyFunction.ClassFunction("setSettingNumber");
		private static PyFunction _setSettingString = PyFunction.ClassFunction("setSettingString");

		private static PyFunction _openSettings = PyFunction.ClassFunction("openSettings");
		private static PyFunction _getLocalizedString = PyFunction.ClassFunction("getLocalizedString");

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
			PyFunction func;

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
		    PyFunction func;

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

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
