using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public enum ServerType
    {
		[StringValue("SERVER_WEBSERVER")]
		SERVER_WEBSERVER = 1,
		[StringValue("SERVER_AIRPLAYSERVER")]
		SERVER_AIRPLAYSERVER = 2,
		[StringValue("SERVER_JSONRPCSERVER")]
		SERVER_JSONRPCSERVER = 3,
		[StringValue("SERVER_UPNPRENDERER")]
		SERVER_UPNPRENDERER = 4,
		[StringValue("SERVER_UPNPSERVER")]
		SERVER_UPNPSERVER = 5,
		[StringValue("SERVER_EVENTSERVER")]
		SERVER_EVENTSERVER = 6,
		[StringValue("SERVER_ZEROCONF")]
		SERVER_ZEROCONF = 7,
	}
}
