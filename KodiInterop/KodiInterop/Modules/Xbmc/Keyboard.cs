using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class Keyboard
    {
		private PyVariable Instance = PyVariableManager.NewVariable(flags: PyVariableFlags.Object);

		/// <summary>
		/// Creates a new Keyboard  object with default text
		/// heading and hidden input flag if supplied.
		/// </summary>
		/// <param name="defaultText">default text entry.</param>
		/// <param name="heading">keyboard heading.</param>
		/// <param name="hidden">True for hidden text entry.</param>
		public Keyboard(string defaultText = null, string heading = null, bool hidden = false) {
			Instance.CallAssign(
				new PythonFunction(PyModule.Xbmc, "Keyboard"),
				new List<object> {
					defaultText, heading, hidden
				}
			);
		}

		/// <summary>
		/// Returns the user input as a string.
		/// </summary>
		public string Text {
			get {
				return Instance.CallFunction(
					new PythonFunction("getText")
				);
			}
		}

		private string Heading {
			set {
				Instance.CallFunction(
					new PythonFunction("setHeading"),
					new List<object> { value }
				);
			}
		}

		public string DefaultText {
			set {
				Instance.CallFunction(
					new PythonFunction("setDefault"),
					new List<object> { value }
				);
			}
		}

		public bool HiddenInput {
			set {
				Instance.CallFunction(
					new PythonFunction("setHiddenInput"),
					new List<object> { value }
				);
			}
		}

		/// <summary>
		/// Returns False if the user cancelled the input.
		/// </summary>
		public bool Confirmed {
			get {
				return bool.Parse(Instance.CallFunction(
					new PythonFunction("isConfirmed")
				));
			}
		}


		/// <summary>
		/// Show keyboard and wait for user action.
		/// </summary>
		/// <param name="timeout">milliseconds to autoclose dialog. (default=do not autoclose)</param>
		public void DoModal(TimeSpan? timeout = null) {
			Instance.CallFunction(
				new PythonFunction("doModal"),
				new List<object> {
					timeout?.TotalMilliseconds
				}
			);
		}
	}
}
