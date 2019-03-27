using Smx.KodiInterop;
using Smx.KodiInterop.Python;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public static class XbmcPath {
		/// <summary>
		/// Returns a thumb cache filename.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string getCacheThumbName(string path) {
			return PythonInterop.CallFunction(
				new PyFunction(PyModule.Xbmc, "getCacheThumbName"),
				path
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
				new PyFunction(PyModule.Xbmc, "makeLegalFilename"),
				filename, fatX
			);
		}

		/// <summary>
		/// Returns the translated path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string translatePath(string path) {
			return PythonInterop.CallFunction(
				new PyFunction(PyModule.Xbmc, "translatePath"),
				path
			);
		}

		/// <summary>
		/// Returns the validated path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string validatePath(string path) {
			return PythonInterop.CallFunction(
				new PyFunction(PyModule.Xbmc, "validatePath"),
				path
			);
		}
	}
}
