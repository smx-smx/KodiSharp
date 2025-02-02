using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class DatabaseEventArgs : EventArgs
    {
		public string? Database { get; private set; }
		public DatabaseEventArgs(string? database = null) {
			this.Database = database;
		}
    }
}
