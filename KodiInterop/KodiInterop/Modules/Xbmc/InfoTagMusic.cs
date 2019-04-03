using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class InfoTagMusic : IDisposable
    {
		public readonly PyVariable Instance;

		public string Album {
			get => Instance.CallFunction("getAlbum");
		}
		public string AlbumArtist {
			get => Instance.CallFunction("getAlbumArtist");
		}
		public string Artist {
			get => Instance.CallFunction("getArtist");
		}
		public string Comment {
			get => Instance.CallFunction("getComment");
		}
		public string Disc {
			get => Instance.CallFunction("getDisc");
		}
		public string Duration {
			get => Instance.CallFunction("getDuration");
		}
		public string Genre {
			get => Instance.CallFunction("getGenre");
		}
		public string LastPlayed {
			get => Instance.CallFunction("getLastPlayed");
		}
		public int Listeners {
			get => Instance.CallFunction("getListeners");
		}
		public string Lyrics {
			get => Instance.CallFunction("getListeners");
		}
		public int PlayCount {
			get => Instance.CallFunction("getPlayCount");
		}
		public string ReleaseDate {
			get => Instance.CallFunction("getReleaseDate");
		}
		public string Title {
			get => Instance.CallFunction("getTitle");
		}
		public int Track {
			get => Instance.CallFunction("getTrack");
		}
		public string URL {
			get => Instance.CallFunction("getURL");
		}

		public InfoTagMusic(PyVariable instance) {
			this.Instance = instance;
		}

		public InfoTagMusic() {
			this.Instance = PyVariableManager.Get.NewVariable();
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
