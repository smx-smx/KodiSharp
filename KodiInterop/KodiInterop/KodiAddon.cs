using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace Smx.KodiInterop
{
	public abstract class KodiAddon
    {
		public readonly int Handle;
		public readonly string BaseUrl;
		public readonly string Parameters;
		public readonly bool IsService;

		//public KodiAddonSettings Settings { get; private set; }

		public string BuildNavUrl(string path, NameValueCollection parameters = null) {
			if (parameters == null) {
				parameters = new NameValueCollection();
			}
			parameters["action"] = path;

			return Utils.BuildUrl(this.BaseUrl, parameters);
		}

		public KodiAddon() {
			try {
#if DEBUG
				ConsoleHelper.CreateConsole();
#endif
				// Clean the variables list from the previous run (we're in a new python instance so they don't exist anymore)
				Python.PyVariableManager.Initialize();

				// Parse parameters
				this.BaseUrl = PythonInterop.EvalToResult("sys.argv[0]").Value;
				this.IsService = PythonInterop.EvalToResult("len(sys.argv) < 2").Value;

				// Initialize the Event Monitor
				//Modules.Xbmc.Events.Initialize();

				// Set running addon
				KodiBridge.RunningAddon = this;

				// If we're being started as a service, don't run addon specific tasks
				if (this.IsService) {
					PyConsole.WriteLine(string.Format("Starting as Service: {0}", this.BaseUrl));
					return;
				}

				this.Handle = int.Parse(PythonInterop.EvalToResult("sys.argv[1]").Value);
				this.Parameters = PythonInterop.EvalToResult("sys.argv[2]").Value;
				PyConsole.WriteLine(string.Format("BaseUrl: {0}, Handle: {1}, Parameters: {2}",
					this.BaseUrl, this.Handle, this.Parameters));

				// Register routes for derived type
				RouteManager.RegisterRoutes(this.GetType());
			} catch(Exception ex) {
				KodiBridge.SaveException(ex);
			}
		}

		public int Run() {
			try {
				// Create the Settings object
				//this.Settings = new KodiAddonSettings();

				// If we have routes, invoke the request handler
				if (RouteManager.Routes.Count > 0) {
					RouteManager.HandleRequest(this.BaseUrl + this.Parameters);
				}
				int result = this.PluginMain();

				return result;
			} catch (Exception ex) {
				// This takes the exception and stores it, not allowing it to bubble up
				KodiBridge.SaveException(ex);
				return 1;
			}
		}

		public abstract int PluginMain();
    }
}
