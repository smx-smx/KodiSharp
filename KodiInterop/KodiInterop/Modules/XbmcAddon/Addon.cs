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
				new PythonFunction("getAddonInfo"),
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
				new PythonFunction("getSetting"),
				new List<object> { setting }
			);
		}

		public T GetSetting<T>(string setting) where T : IConvertible {
			PythonFunction func;

			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.Boolean:
					func = new PythonFunction("getSettingBool");
					break;
				case TypeCode.Decimal:
					func = new PythonFunction("getSettingInt");
					break;
				case TypeCode.Double:
					func = new PythonFunction("getSettingNumber");
					break;
				case TypeCode.String:
					func = new PythonFunction("getSettingString");
					break;
				default:
					func = new PythonFunction("getSetting");
					break;

			}
			string value = Instance.CallFunction(func, new List<object>{ setting });
			return (T)Convert.ChangeType(value, typeof(T));
		}

	    public void SetSetting<T>(string setting, object value) where T : IConvertible {
		    PythonFunction func;

		    switch (Type.GetTypeCode(typeof(T))) {
			    case TypeCode.Boolean:
				    func = new PythonFunction("setSettingBool");
				    break;
			    case TypeCode.Decimal:
				    func = new PythonFunction("setSettingInt");
				    break;
			    case TypeCode.Double:
				    func = new PythonFunction("setSettingNumber");
				    break;
			    case TypeCode.String:
				    func = new PythonFunction("setSettingString");
				    break;
				default:
					func = new PythonFunction("setSetting");
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
				new PythonFunction("setSetting"),
				new List<object> { setting, value }
			);
		}

		/// <summary>
		/// Opens this scripts settings dialog.
		/// </summary>
		public void OpenSettings() {
			Instance.CallFunction(
				new PythonFunction("openSettings")
			);
		}

		/// <summary>
		/// Returns an addon's localized 'unicode string'.
		/// </summary>
		/// <param name="id">id# for string you want to localize.</param>
		/// <returns>Localized 'unicode string'</returns>
		public string GetLocalizedString(int id) {
		    return Instance.CallFunction(
				new PythonFunction("getLocalizedString"),
				new List<object> { id }
		    );
	    }
    }
}
