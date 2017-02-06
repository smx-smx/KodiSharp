namespace Smx.KodiInterop.Python
{
	[StringEnum]
	public enum PyModule
    {
		[StringValue("xbmc")]
		Xbmc = 0,
		[StringValue("xbmcgui")]
		XbmcGui,
		[StringValue("xbmcplugin")]
		XbmcPlugin,
		[StringValue("xbmcaddon")]
		XbmcAddon,
		[StringValue("xbmcvfs")]
		XbmcVfs
	}
}
