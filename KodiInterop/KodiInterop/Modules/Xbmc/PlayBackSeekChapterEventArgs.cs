using System;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public class PlayBackSeekChapterEventArgs : EventArgs
	{
		public int Chapter { get; private set; }
		public PlayBackSeekChapterEventArgs(int chapter) {
			this.Chapter = chapter;
		}
	}
}