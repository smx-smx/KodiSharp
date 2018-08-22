using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public enum DVDState
    {
		[StringValue("DRIVE_NOT_READY")]
		DriveNotReady = 1,
		[StringValue("TRAY_OPEN")]
		TrayOpen = 16,
		[StringValue("TRAY_CLOSED_NO_MEDIA")]
		TrayClosedNoMedia = 64,
		[StringValue("TRAY_CLOSED_MEDIA_PRESENT")]
		TrayClosedMediaPresent = 96
	}
}
