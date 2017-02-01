using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop
{
    public static class SpecialPaths
    {
		/// <summary>
		/// Kodi's installation root directory. This path is read-only contains the Kodi binary, support libraries and default configuration files, skins, scripts and plugins. Users should not modify files or install addons in this directory.
		/// </summary>
		public const string Xbmc = "special://xbmc";

		/// <summary>
		/// Kodi's user specific (OS user) configuration directory. This path contains a writable version of the special://xbmc directories. Any addons should be installed here.
		/// </summary>
		public const string Home = "special://home";

		/// <summary>
		/// Kodi's temporary directory. This path is used to cache various data during normal usage. Unless you need the log, nothing in this directory is detrimental to Kodi's operation. Normally special://home/temp
		/// </summary>
		public const string Temp = "special://temp";

		/// <summary>
		/// Kodi's main configuration directory. Normally located at special://home/userdata, this directory contains global settings and sources, as well as any Kodi profile directories. Normally special://home/userdata
		/// </summary>
		public const string MasterProfile = "special://masterprofile";

		/// <summary>
		/// 	Kodi's currently active profile directory. This directory points at special://masterprofile/profile_name (or special://masterprofile if no profile is in use) and contains per profile overrides for settings and sources.
		/// </summary>
		public const string Profile = "special://profile";

		/// <summary>
		/// User defined custom subtitle path. Set it in Video Settings.
		/// </summary>
		public const string Subtitles = "special://subtitles";

		/// <summary>
		/// Alias from special://masterprofile.
		/// </summary>
		public const string UserData = "special://userdata";

		/// <summary>
		/// This path contains the database files Kodi uses to store library info. Normally special://masterprofile/Database.
		/// </summary>
		public const string Database = "special://database";

		/// <summary>
		/// This path contains cached thumbnails. Normally special://masterprofile/Thumbnails
		/// </summary>
		public const string Thumbnails = "special://thumbnails";

		/// <summary>
		/// This path contains saved PVR recordings.
		/// </summary>
		public const string Recordings = "special://recordings";

		/// <summary>
		/// This path contains Kodi screen shots. You will be asked to specify this directory the first time you take a screen shot.
		/// </summary>
		public const string Screenshots = "special://screenshots";

		/// <summary>
		/// This path contains saved music playlists. Normally special://profile/playlists/music.
		/// </summary>
		public const string MusicPlayLists = "special://musicplaylists";

		/// <summary>
		/// This path contains saved video playlists. Normally special://profile/playlists/video.
		/// </summary>
		public const string VideoPlayLists = "special://videoplaylists";

		/// <summary>
		/// This path contains the tracks from CDs you rip with Kodi. You will be asked to specify this directory the first time you rip a CD.
		/// </summary>
		public const string CDRips = "special://cdrips";

		/// <summary>
		/// This path points to the currently active skin's root directory.
		/// </summary>
		public const string Skin = "special://skin";

		/// <summary>
		/// This path points to the path where the log file is saved.
		/// </summary>
		public const string LogPath = "special://logpath";
	}
}
