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
		public int Handle { get; private set; }
		public string BaseUrl { get; private set; }
		public string Parameters { get; private set; }
		public bool IsService { get; private set; }

		public readonly bool IsPersistent;

		public KodiAddonSettings Settings { get; private set; }
		public Addon Addon { get; private set; }
		public RouteManager Router { get; private set; }

		public readonly PyVariableManager PyVariableManager;

		public string BuildNavUrl(string path, NameValueCollection parameters = null){
			if (parameters == null) {
				parameters = new NameValueCollection();
			}
			parameters["action"] = path;

			return Utils.BuildUrl(this.BaseUrl, parameters);
		}

		public readonly bool DebugEnabled;

		private void Initialize() {
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
		}

		public KodiAddon(bool persist = false, bool debug=false){
			DebugEnabled = debug;
			IsPersistent = persist;
			try {
				PyVariableManager = new PyVariableManager();

				// Parse parameters
				this.BaseUrl = PythonInterop.EvalToResult("sys.argv[0]").Value;
				this.IsService = PythonInterop.EvalToResult("len(sys.argv) < 2").Value;

				this.Router = new RouteManager(this);
			} catch(Exception ex) {
				KodiBridge.SaveException(ex);
			}
		}

		/// <summary>
		/// Creates a new addon instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="addonType">The class of the addon to create</param>
		/// <param name="debug">Enables or disabled the debugging console (works on windows only)</param>
		/// <param name="persist">Whether or not to reuse a previous addon instance</param>
		/// <returns></returns>
		public static T GetInstance<T>(Type addonType, bool debug = false, bool persist = false) where T: KodiAddon {
			if (debug) {
				ConsoleHelper.CreateConsole();
			}

			string BaseUrl = PythonInterop.EvalToResult("sys.argv[0]").Value;

			T instance = null;

			if (persist) {
				T previousInstance = (T)KodiBridge.GetPersistentAddon(BaseUrl);
				if (previousInstance != null) {
					Console.WriteLine("REUSING PREVIOUS INSTANCE");
					instance = previousInstance;
				}
			}

			if (instance == null) {
				instance = (T)Activator.CreateInstance(addonType);
				if (persist) {
					KodiBridge.RegisterPersistentAddon(BaseUrl, instance);
				}
			}

			// Set running addon
			KodiBridge.SetRunningAddon(BaseUrl, instance);

			// Initialize addon fields.
			// If the addon is persistent, it updates fields that might have changed
			instance.Initialize();

			return instance;
		}

		public int Run() {
			try {
				// Create the Settings object
				//this.Settings = new KodiAddonSettings();

				// If we have routes, invoke the request handler
				if (Router.Routes.Count > 0) {
					Router.HandleRequest(this.BaseUrl + this.Parameters);
				}
				int result = this.PluginMain();

				return result;
			} catch (Exception ex) {
				// This takes the exception and stores it, not allowing it to bubble up
				KodiBridge.SaveException(ex);
				return 1;
			} finally {
				KodiBridge.SetRunningAddon(null, null);

				if (!IsPersistent) {
					KodiBridge.ScheduleAddonTermination(BaseUrl);
				}
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
