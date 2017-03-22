using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public enum DVDState
    {
		[StringValue("DRIVE_NOT_READY")]
		DRIVE_NOT_READY = 1,
		[StringValue("TRAY_OPEN")]
		TRAY_OPEN = 16,
		[StringValue("TRAY_CLOSED_NO_MEDIA")]
		TRAY_CLOSED_NO_MEDIA = 64,
		[StringValue("TRAY_CLOSED_MEDIA_PRESENT")]
		TRAY_CLOSED_MEDIA_PRESENT = 96
	}
}
