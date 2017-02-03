using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using TestPlugin;

namespace Smx.KodiInterop
{
	public static class RouteManager
    {
		public delegate void RouteCallback(NameValueCollection parameters);
		public static readonly Dictionary<string, RouteCallback> Routes;

		static RouteManager() {
			Routes = new Dictionary<string, RouteCallback>();
		}

		private static string RemoveTrailingSlash(string path) {
			if (path.EndsWith("/") && path.Length > 1)
				path = path.Remove(path.Length - 1);
			return path;
		}

		public static void RegisterRoutes(Type addonClass) {
			Routes.Clear();

			var methods = addonClass.GetMethods().Where(m => Attribute.IsDefined(m, typeof(RouteAttribute), false));
			foreach(var method in addonClass.GetMethods()) {
				var attributes = method.GetCustomAttributes(typeof(RouteAttribute), false);
				if(attributes.Length > 0) {
					RouteAttribute route = attributes[0] as RouteAttribute;

					string url = RemoveTrailingSlash(route.Url);
					RouteCallback callback = method.CreateDelegate(typeof(RouteCallback)) as RouteCallback;
					//RouteCallback callback = Delegate.CreateDelegate(typeof(RouteCallback), method) as RouteCallback;
					Routes.Add(url, callback);

					
					Console.WriteLine(string.Format("--> Registering route for '{0}'", url));
				}
			}
		}

		public static void HandleRequest(KodiAddon addon, string request) {
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
				return;
			}
			Console.WriteLine("Going to call " + path);

			Routes[path](qs);
		}
	}
}
