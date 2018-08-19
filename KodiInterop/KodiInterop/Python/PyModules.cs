namespace Smx.KodiInterop.Python
{
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
		XbmcVfs,
        [StringValue("sys")]
        Sys
    }
}
