using System.Collections.Generic;

namespace Smx.KodiInterop.Xbmc
{
	public static class Path {
		/// <summary>
		/// Returns a thumb cache filename.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string getCacheThumbName(string path) {
			return PythonInterop.CallFunction(
				new PythonFunction(PyModules.Xbmc, "getCacheThumbName"),
				new List<object> { path }
			);
		}

		/// <summary>
		/// Returns a legal filename or path as a string.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="fatX"></param>
		/// <returns></returns>
		public static string makeLegalFilename(string filename, bool fatX = false) {
			return PythonInterop.CallFunction(
				new PythonFunction(PyModules.Xbmc, "makeLegalFilename"),
				new List<object> { filename, fatX }
			);
		}

		/// <summary>
		/// Returns the translated path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string translatePath(string path) {
			return PythonInterop.CallFunction(
				new PythonFunction(PyModules.Xbmc, "translatePath"),
				new List<object> { path }
			);
		}

		/// <summary>
		/// Returns the validated path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string validatePath(string path) {
			return PythonInterop.CallFunction(
				new PythonFunction(PyModules.Xbmc, "validatePath"),
				new List<object> { path }
			);
		}
	}
}
