using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public enum EventTypes
	{
		[StringValue("onAbortRequested")]
		AbortRequested = 0,
		[StringValue("onCleanStarted")]
		CleanStarted,
		[StringValue("onCleanFinished")]
		CleanFinished,
		[StringValue("onDPMSActivated")]
		DPMSActivated,
		[StringValue("onDPMSDeactivated")]
		DPMSDeactivated,
		[StringValue("onDatabaseScanStarted")]
		DatabaseScanStarted,
		[StringValue("onDatabaseUpdated")]
		DatabaseUpdated,
		[StringValue("onNotification")]
		Notification,
		[StringValue("onScanStarted")]
		ScanStarted,
		[StringValue("onScanFinished")]
		ScanFinished,
		[StringValue("onScreensaverActivated")]
		ScreensaverActivated,
		[StringValue("onScreensaverDeactivated")]
		ScreensaverDeactivated,
		[StringValue("onSettingsChanged")]
		SettingsChanged,
	}
}
