using Smx.KodiInterop.Modules.XbmcGui;
using Smx.KodiInterop.Python;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public class Player
    {
		private PyVariable Instance;

		/*
		public Player() {
			Instance.CallAssign(
				new PythonFunction(PyModule.Xbmc, "Player");
			);
		}
		*/

		public static void Play(
			string location = null,
			ListItem listItem = null,
			bool? windowed = null,
			int? startPos = null
		) {

		}

		public static void Play(
			PlayList playList = null
		) {
		}
    }
}
