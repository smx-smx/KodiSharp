using Smx.KodiInterop.Modules.Xbmc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
	/// <summary>
	/// Example class to store static data that lives between plugin invokations
	/// </summary>
	public static class TestPluginState
	{
		public static DateTime? LastMainPageVisitTime = null;
		public static XbmcPlayer Player = null;
		public static XbmcMonitor Monitor = null;
	}
}
