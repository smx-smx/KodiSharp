using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python
{
    public class PyThread
    {
		private PyVariable Instance = PyVariableManager.NewVariable(flags: PyVariableFlags.Object);

		/// <summary>
		/// Creates a new threading.Thread
		/// </summary>
		/// <param name="threadFuncName">The name of the function</param>
		public PyThread(string threadFuncName) {
			Instance.CallAssign(
				new PythonFunction("threading", "Thread"),
				new List<object> {
					null, threadFuncName
				}
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
