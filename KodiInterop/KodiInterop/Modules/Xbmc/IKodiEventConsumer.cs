using Smx.KodiInterop.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public interface IKodiEventConsumer
    {
		bool TriggerEvent(KodiEventMessage e);
	}
}
