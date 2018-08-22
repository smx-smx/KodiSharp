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

		/// <summary>
		/// Creates a new AddOn class.
		/// </summary>
		/// <param name="id">id of the addon as specified in addon.xml.
		/// If not specified, the running addon is used</param>
		public Addon(string id = null) {
			Instance.CallAssign(
				new PythonFunction(PyModule.XbmcAddon, "Addon"),
				new List<object> { id }
			);
		}

		/// <summary>
		/// Returns the value of an addon property as a string.
		/// </summary>
		/// <param name="info">the property that the module needs to access.</param>
		/// <returns></returns>
		public string GetAddonInfo(AddonInfo info) {
			return Instance.CallFunction(
				PythonFunction.ClassFunction("getAddonInfo"),
				new List<object> { info.GetString() }
			);
		}

		/// <summary>
		/// Returns the value of a setting as a unicode string.
		/// </summary>
		/// <param name="setting">the setting that the module needs to access.</param>
		/// <returns></returns>
		public string GetSetting(string setting) {
			return Instance.CallFunction(
				PythonFunction.ClassFunction("getSetting"),
				new List<object> { setting }
			);
		}

		public T GetSetting<T>(string setting) where T : IConvertible {
			PythonFunction func;

			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.Boolean:
					func = PythonFunction.ClassFunction("getSettingBool");
					break;
				case TypeCode.Decimal:
					func = PythonFunction.ClassFunction("getSettingInt");
					break;
				case TypeCode.Double:
					func = PythonFunction.ClassFunction("getSettingNumber");
					break;
				case TypeCode.String:
					func = PythonFunction.ClassFunction("getSettingString");
					break;
				default:
					func = PythonFunction.ClassFunction("getSetting");
					break;

			}
			string value = Instance.CallFunction(func, new List<object>{ setting });
			return (T)Convert.ChangeType(value, typeof(T));
		}

	    public void SetSetting<T>(string setting, object value) where T : IConvertible {
		    PythonFunction func;

		    switch (Type.GetTypeCode(typeof(T))) {
			    case TypeCode.Boolean:
				    func = PythonFunction.ClassFunction("setSettingBool");
				    break;
			    case TypeCode.Decimal:
				    func = PythonFunction.ClassFunction("setSettingInt");
				    break;
			    case TypeCode.Double:
				    func = PythonFunction.ClassFunction("setSettingNumber");
				    break;
			    case TypeCode.String:
				    func = PythonFunction.ClassFunction("setSettingString");
				    break;
				default:
					func = PythonFunction.ClassFunction("setSetting");
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
			Instance.CallFunction(
				PythonFunction.ClassFunction("setSetting"),
				new List<object> { setting, value }
			);
		}

		/// <summary>
		/// Opens this scripts settings dialog.
		/// </summary>
		public void OpenSettings() {
			Instance.CallFunction(
				PythonFunction.ClassFunction("openSettings")
			);
		}

		/// <summary>
		/// Returns an addon's localized 'unicode string'.
		/// </summary>
		/// <param name="id">id# for string you want to localize.</param>
		/// <returns>Localized 'unicode string'</returns>
		public string GetLocalizedString(int id) {
		    return Instance.CallFunction(
				PythonFunction.ClassFunction("getLocalizedString"),
				new List<object> { id }
		    );
	    }
    }
}
