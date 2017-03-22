using System;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public class PlayBackSeekEventArgs : EventArgs
	{
		public int Time { get; private set; }
		public int SeekOffset { get; private set; }

		public PlayBackSeekEventArgs(int time, int seekOffset) {
			this.Time = time;
			this.SeekOffset = seekOffset;
		}
	}
}