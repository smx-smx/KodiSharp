using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python
{
    public class PyThread
    {
		private PyVariable instance = PyVariableManager.NewVariable(flags: PyVariableFlags.Object);
		public PyVariable Instance {
			get {
				return instance;
			}
		}

		/// <summary>
		/// Creates a new threading.Thread
		/// </summary>
		/// <param name="threadFuncName">The name of the function</param>
		public PyThread(string threadFuncName) {
			instance.CallAssign(
				new PythonFunction("threading", "Thread"),
				new List<object> {
					null, threadFuncName
				}, EscapeFlags.None
			);
		}

		public void Start() {
			instance.CallFunction("start");
		}

		public void Join() {
			instance.CallFunction("join");
		}
    }
}
