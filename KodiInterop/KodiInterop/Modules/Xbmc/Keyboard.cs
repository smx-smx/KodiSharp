using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class Keyboard
    {
		private PyVariable Instance = PyVariableManager.Get.NewVariable();

		private static PyFunction _getText = PyFunction.ClassFunction("getText");
		private static PyFunction _setHeading = PyFunction.ClassFunction("setHeading");
		private static PyFunction _setDefault = PyFunction.ClassFunction("setDefault");
		private static PyFunction _setHiddenInput = PyFunction.ClassFunction("setHiddenInput");
		private static PyFunction _isConfirmed = PyFunction.ClassFunction("isConfirmed");
		private static PyFunction _doModal = PyFunction.ClassFunction("doModal");

		/// <summary>
		/// Creates a new Keyboard  object with default text
		/// heading and hidden input flag if supplied.
		/// </summary>
		/// <param name="defaultText">default text entry.</param>
		/// <param name="heading">keyboard heading.</param>
		/// <param name="hidden">True for hidden text entry.</param>
		public Keyboard(string defaultText = null, string heading = null, bool hidden = false) {
			Instance.CallAssign(
				new PyFunction(PyModule.Xbmc, "Keyboard"),
				defaultText, heading, hidden
			);
		}

		/// <summary>
		/// Returns the user input as a string.
		/// </summary>
		public string Text {
			get {
				return Instance.CallFunction(_getText);
			}
		}

		private string Heading {
			set {
				Instance.CallFunction(_setHeading, value);
			}
		}

		public string DefaultText {
			set {
				Instance.CallFunction(_setDefault, value);
			}
		}

		public bool HiddenInput {
			set {
				Instance.CallFunction(_setHiddenInput, value);
			}
		}

		/// <summary>
		/// Returns False if the user cancelled the input.
		/// </summary>
		public bool Confirmed {
			get {
				return Convert.ToBoolean(Instance.CallFunction(_isConfirmed));
			}
		}


		/// <summary>
		/// Show keyboard and wait for user action.
		/// </summary>
		/// <param name="timeout">milliseconds to autoclose dialog. (default=do not autoclose)</param>
		public void DoModal(TimeSpan? timeout = null) {
			Instance.CallFunction(_doModal, timeout?.TotalMilliseconds);
		}
	}
}
