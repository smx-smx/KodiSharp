using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Smx.KodiInterop.Modules.XbmcAddon;
using Smx.KodiInterop.Python;

namespace Smx.KodiInterop
{
	public abstract class KodiAddon
	{
		public readonly int Handle;
		public readonly string BaseUrl;
		public readonly string Parameters;
		public readonly bool IsService;

		public readonly KodiAddonSettings Settings;
		public readonly Addon Addon;

		public readonly PyVariableManager PyVariableManager;

		public string BuildNavUrl(string path, NameValueCollection parameters = null){
			if (parameters == null) {
				parameters = new NameValueCollection();
			}
			parameters["action"] = path;

			return Utils.BuildUrl(this.BaseUrl, parameters);
		}

		public KodiAddon(bool debug=false){
			try {
                if (debug)
                {
                    ConsoleHelper.CreateConsole();
                }

				// Set running addon
				KodiBridge.RunningAddon = this;

				PyVariableManager = new PyVariableManager();

				// Parse parameters
				this.BaseUrl = PythonInterop.EvalToResult("sys.argv[0]").Value;
				this.IsService = PythonInterop.EvalToResult("len(sys.argv) < 2").Value;

				// Instance of XbmcAddon
				this.Addon = new Addon();

				// Settings accessor
				this.Settings = new KodiAddonSettings(this.Addon);

				//string addonName = BaseUrl.Replace("plugin://", "").Replace("/", "");

				// If we're being started as a service, don't run addon specific tasks
				if (this.IsService) {
					PyConsole.WriteLine(string.Format("Starting as Service: {0}", this.BaseUrl));
					return;
				}

				this.Handle = Convert.ToInt32(PythonInterop.EvalToResult("sys.argv[1]").Value);
				this.Parameters = PythonInterop.EvalToResult("sys.argv[2]").Value;
				PyConsole.WriteLine(string.Format("BaseUrl: {0}, Handle: {1}, Parameters: {2}",
					this.BaseUrl, this.Handle, this.Parameters));

				// Register routes for derived type
				RouteManager.RegisterRoutes(this);
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
			} finally {
                /*
				 * When we get here, we have already returned from PluginMain
				 * tell Python that we are done (TODO: Wait for threads here)
				 * */
				KodiBridge.StopRPC();
			}
		}

		public abstract int PluginMain();
    }
}
