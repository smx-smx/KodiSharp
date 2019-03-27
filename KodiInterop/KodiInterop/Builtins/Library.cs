using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Builtins
{
	public enum CleanLibraryType
	{
		[StringValue("video")]
		Video,
		[StringValue("music")]
		Music,
		[StringValue("tvshows")]
		TvShows,
		[StringValue("musicvideos")]
		MusicVideos
	}

	public enum ExportLibraryType
	{
		[StringValue("video")]
		Video,
		[StringValue("music")]
		Music
	}

	public static class LibraryBuiltins
	{
		public static void CleanLibrary(CleanLibraryType type) {
			PythonInterop.CallBuiltin("cleanlibrary", new List<string> { type.GetString() });
		}

		public static void ExportLibrary(
			ExportLibraryType type,
			bool exportSingleFile = false,
			bool exportThumbs = false,
			bool overwrite = false,
			bool exportActorThumbs = false
		) {

		}
	}
}
