using System;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public class PlayBackSpeedChangedEventArgs : EventArgs
	{
		public int Speed { get; private set; }

		public PlayBackSpeedChangedEventArgs(int speed) {
			this.Speed = speed;
		}
	}
}