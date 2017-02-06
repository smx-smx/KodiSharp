using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Smx.KodiInterop
{
    public class UrlNavigationEventArgs
    {
		public NameValueCollection Parameters { get; private set; }
		public UrlNavigationEventArgs(NameValueCollection parameters) {
			this.Parameters = parameters;
		}
    }
}
