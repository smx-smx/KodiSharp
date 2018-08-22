using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python
{
    public class PyThread
    {
		public PyVariable Instance { get; } = PyVariableManager.Get.NewVariable();

		/// <summary>
		/// Creates a new threading.Thread
		/// </summary>
		/// <param name="threadFuncName">The name of the function</param>
		public PyThread(string threadFuncName) {
			Instance.CallAssign(
				new PyFunction("threading", "Thread"),
				new List<object> {
					null, threadFuncName
				}, EscapeFlags.None
			);
		}

		public void Start() {
			Instance.CallFunction("start");
		}

		public void Join() {
			Instance.CallFunction("join");
		}
    }
}
