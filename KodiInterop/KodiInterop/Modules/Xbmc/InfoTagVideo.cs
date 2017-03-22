using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class InfoTagVideo
    {
		public readonly PyVariable Instance;

		public InfoTagVideo(PyVariable instance) {
			this.Instance = instance;
		}

		public InfoTagVideo() {
			this.Instance = PyVariableManager.NewVariable(flags: PyVariableFlags.Object);
		}
	}
}
