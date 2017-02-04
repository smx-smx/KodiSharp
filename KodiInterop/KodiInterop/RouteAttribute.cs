using System;

namespace Smx.KodiInterop
{
	public class RouteAttribute : Attribute
	{
		public string Url { get; private set; }

		public RouteAttribute(string url) {
			this.Url = url;
		}
	}
}