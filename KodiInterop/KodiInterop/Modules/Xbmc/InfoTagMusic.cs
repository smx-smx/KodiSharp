using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class InfoTagMusic : IDisposable
    {
		public readonly PyVariable Instance;

		public InfoTagMusic(PyVariable instance) {
			this.Instance = instance;
		}

		public InfoTagMusic() {
			this.Instance = PyVariableManager.Get.NewVariable();
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
