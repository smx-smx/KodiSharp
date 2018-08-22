using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public enum ServerType
    {
		[StringValue("SERVER_WEBSERVER")]
		WebServer = 1,
		[StringValue("SERVER_AIRPLAYSERVER")]
		AirplayServer = 2,
		[StringValue("SERVER_JSONRPCSERVER")]
		JsonRpcServer = 3,
		[StringValue("SERVER_UPNPRENDERER")]
		UpnpRenderer = 4,
		[StringValue("SERVER_UPNPSERVER")]
		UpnpServer = 5,
		[StringValue("SERVER_EVENTSERVER")]
		EventServer = 6,
		[StringValue("SERVER_ZEROCONF")]
		ZeroConfServer = 7,
	}
}
