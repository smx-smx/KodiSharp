using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Smx.KodiInterop
{
    public abstract class KodiAddon
    {
		//http://stackoverflow.com/a/1373295
		private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args) {
			string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
			if (!File.Exists(assemblyPath)) return null;
			Assembly assembly = Assembly.LoadFrom(assemblyPath);
			return assembly;
		}

		private static void SetAssemblyResolver() {
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
		}

		public KodiAddon() {
			SetAssemblyResolver();
		}

		public int Run() {
			try {
				return this.PluginMain();
			} catch (Exception ex) {
				// This takes the exception and stores it, not allowing it to bubble up
				KodiBridge.SaveException(ex);
				return 1;
			}
		}

		public abstract int PluginMain();
    }
}
