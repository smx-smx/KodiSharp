using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class InfoTagRadioRDS
    {
		public readonly PyVariable Instance;

		public InfoTagRadioRDS(PyVariable instance) {
			this.Instance = instance;
		}
		public InfoTagRadioRDS() {
			this.Instance = PyVariableManager.Get.NewVariable();
		}
    }
}
