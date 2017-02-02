using System;
using System.IO;
using System.Reflection;

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

		public int Handle { get; private set; }
		public string BaseUrl { get; private set; }
		public string Parameters { get; private set; }

		public KodiAddon() {
			SetAssemblyResolver();
			this.BaseUrl = PythonInterop.EvalToResult("sys.argv[0]");
			this.Handle = int.Parse(PythonInterop.EvalToResult("sys.argv[1]"));
			this.Parameters = PythonInterop.EvalToResult("sys.argv[2]");
			PyConsole.WriteLine(string.Format("BaseUrl: {0}, Handle: {1}, Parameters: {2}",
				this.BaseUrl, this.Handle, this.Parameters));
			KodiBridge.RunningAddon = this;
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
