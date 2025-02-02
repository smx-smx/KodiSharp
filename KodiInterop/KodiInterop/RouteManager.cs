using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Smx.KodiInterop
{
	public class RouteManager
    {
		public delegate int RouteCallback(NameValueCollection parameters);
		public readonly Dictionary<string, RouteCallback> Routes = new Dictionary<string, RouteCallback>();

		private static string RemoveTrailingSlash(string path) {
			if (path.EndsWith("/") && path.Length > 1)
				path = path.Remove(path.Length - 1);
			return path;
		}

		public RouteManager(KodiAddon addon) {
			RegisterRoutes(addon);
		}

		/// <summary>
		/// Install Routes from the specified class
		/// </summary>
		/// <param name="addonClass">The class that will be looked up</param>
		private void RegisterRoutes(KodiAddon addon) {
			Type addonClass = addon.GetType();

			Routes.Clear();
			Console.WriteLine("REGISTER ROUTES");

			var methods = addonClass.GetMethods().Where(m => Attribute.IsDefined(m, typeof(RouteAttribute), false));
			Console.WriteLine("Num Attribs: {0}", methods.Count());
			foreach(var method in addonClass.GetMethods()) {
				var attributes = method.GetCustomAttributes(typeof(RouteAttribute), false);
				if(attributes.Length > 0) {
					RouteAttribute route = (RouteAttribute)attributes[0];

					string url = RemoveTrailingSlash(route.Url);
					// Delegate from Class instance
					RouteCallback callback = (RouteCallback)Delegate.CreateDelegate(typeof(RouteCallback), addon, method);
					// Delegate from Static method
					//RouteCallback callback = method.CreateDelegate(typeof(RouteCallback)) as RouteCallback;
					//RouteCallback callback = Delegate.CreateDelegate(typeof(RouteCallback), method) as RouteCallback;
					Routes.Add(url, callback);

					
					Console.WriteLine(string.Format("--> Registering route for '{0}'", url));
				}
			}
		}

		/// <summary>
		/// Handle a request by using the registered routes
		/// </summary>
		/// <param name="request">request URL</param>
		public int HandleRequest(string request) {
			Uri url = new Uri(request);
			var qs = HttpUtility.ParseQueryString(url.Query);


			Console.WriteLine("URL IS " + request);
			string path = "/";
			if (qs.AllKeys.Contains("action")) {
				path = RemoveTrailingSlash(qs["action"]);
			}

			PyConsole.WriteLine(string.Format("Checking route for '{0}'", path));
			if (!Routes.ContainsKey(path)) {
				PyConsole.WriteLine(string.Format("Route not found for path '{0}'", path));
				return 1;
			}
			Console.WriteLine("Going to call " + path);

			return Routes[path](qs);
		}
	}
}
