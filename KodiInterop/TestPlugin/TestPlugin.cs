using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using Smx.KodiInterop;
using Smx.KodiInterop.Builtins;
using Smx.KodiInterop.Messages;
using System.Collections.Generic;
using System;
using System.Threading;

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
			new Notification {
				Header = "My Notification",
				Message = "Hello World from C#",
				Duration = TimeSpan.FromSeconds(10)
			}.Show();
			return "Hello, Python";
		}
	}
}
