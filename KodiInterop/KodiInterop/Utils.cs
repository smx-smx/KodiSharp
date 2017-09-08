using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Smx.KodiInterop
{
    class Utils
    {
		public static bool IsUnixOS()
		{
			int p = (int)Environment.OSVersion.Platform;
			return ((p == 4) || (p == 6) || (p == 128));
		}

		public static string BuildUrl(string url, NameValueCollection getParams = null) {
			if(getParams == null) {
				getParams = new NameValueCollection();
			}

			List<string> items = new List<string>();
			foreach (string key in getParams) {
				items.Add(string.Concat(key, "=", HttpUtility.UrlEncode(getParams[key])));
			}
			return url + "?" + string.Join("&", items);
		}
    }
}
