using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class LibraryEventArgs : EventArgs
    {
		/// <summary>
		/// Video / music as string
		/// </summary>
		public string? Library { get; private set; }
		public LibraryEventArgs(string? library = null) {
			this.Library = library;
		}
    }
}
