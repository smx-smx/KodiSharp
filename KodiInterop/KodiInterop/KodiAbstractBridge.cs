using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop
{
	public abstract class KodiAbstractBridge : IKodiBridge
	{
		public delegate IntPtr PySendStringDelegate(string messageData);
		protected abstract Delegate PySendString { get; }

		/// <summary>
		/// Wrapper to PySendMessageDelegate that does *not* free the string, like in MarshalAs (causing a python crash when it tries to free the string again)
		/// </summary>
		/// <param name="messageData"></param>
		/// <returns></returns>
		public string PySendMessage(string messageData) {
			IntPtr pyStr = (IntPtr)PySendString.DynamicInvoke(new object[] { messageData });
			if (pyStr == IntPtr.Zero)
				return "";
			return Marshal.PtrToStringAnsi(pyStr);
		}

		public static MethodInfo FindPluginMain() {
			Assembly thisAsm = Assembly.GetExecutingAssembly();

			var assemblies = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(asm => asm != thisAsm);

			MethodInfo pluginEntry = null;
			foreach (Assembly asm in assemblies) {
				pluginEntry = asm.GetTypes()
					.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
					.Where(m => m.GetCustomAttribute<PluginEntryAttribute>() != null)
					.FirstOrDefault();
				if (pluginEntry != null)
					break;
			}

			return pluginEntry;
		}
	}
}
