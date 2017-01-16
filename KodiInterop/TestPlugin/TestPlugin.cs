using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using Smx.KodiInterop;
using Smx.KodiInterop.Messages;
using System.Collections.Generic;

namespace TestPlugin {
	public class TestPlugin
    {
		private static string PluginPath = null;
		[DllExport("PluginInit", CallingConvention = CallingConvention.Cdecl)]
		public static void PluginInit(string pluginPath) {
			PluginPath = pluginPath;
		}

		[DllExport("PluginMain", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.AnsiBStr)]
		public static string PluginMain() {
			PythonEvalMessage msg = new PythonEvalMessage {
				Code = "xbmc.executebuiltin(\"Notification(C#, Hello World from C#)\")"
			};
			KodiBridge.SendMessage(msg);
			return "Hello, Python";
		}
	}
}
