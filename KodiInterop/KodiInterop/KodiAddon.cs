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
		public int Handle { get; private set; }
		public string BaseUrl { get; private set; }
		public string Parameters { get; private set; }

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
				// Parse parameters
				this.BaseUrl = PythonInterop.EvalToResult("sys.argv[0]");
				this.Handle = int.Parse(PythonInterop.EvalToResult("sys.argv[1]"));
				this.Parameters = PythonInterop.EvalToResult("sys.argv[2]");
				PyConsole.WriteLine(string.Format("BaseUrl: {0}, Handle: {1}, Parameters: {2}",
					this.BaseUrl, this.Handle, this.Parameters));

				// Register routes for derived type
				RouteManager.RegisterRoutes(this.GetType());

				// Set running addon
				KodiBridge.RunningAddon = this;
			} catch(Exception ex) {
				KodiBridge.SaveException(ex);
			}
		}

		public int Run() {
			try {
				// Clean the variables list from the previous run (we're in a new python instance so they don't exist anymore)
				Python.PyVariableManager.Initialize();

				// If we have routes, invoke the request handler
				if (RouteManager.Routes.Count > 0) {
					RouteManager.HandleRequest(this, this.BaseUrl + this.Parameters);
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
