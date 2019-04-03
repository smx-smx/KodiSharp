using Smx.KodiInterop.Python;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Modules.Xbmc
{
    public class InfoTagVideo : IDisposable
    {
		public readonly PyVariable Instance;

		public string Cast {
			get => Instance.CallFunction("getCast");
		}
		public string Director {
			get => Instance.CallFunction("getDirector");
		}
		public string File {
			get => Instance.CallFunction("getFile");
		}
		public string FirstAired {
			get => Instance.CallFunction("firstAired");
		}
		public string Genre {
			get => Instance.CallFunction("getGenre");
		}
		public string IMDBNumber {
			get => Instance.CallFunction("getIMDBNumber");
		}
		public string LastPlayed {
			get => Instance.CallFunction("getLastPlayed");
		}
		public string OriginalTitle {
			get => Instance.CallFunction("getOriginalTitle");
		}
		public string Path {
			get => Instance.CallFunction("getPath");
		}
		public string PictureURL {
			get => Instance.CallFunction("GetPictureURL");
		}
		public int PlayCount {
			get => Instance.CallFunction("getPlayCount");
		}
		public string Plot {
			get => Instance.CallFunction("getPlot");
		}
		public string PlotOutline {
			get => Instance.CallFunction("getPlotOutline");
		}
		public string Premiered {
			get => Instance.CallFunction("getPremiered");
		}
		public float Rating {
			get => Instance.CallFunction("getRating");
		}
		public string TagLine {
			get => Instance.CallFunction("getTagLine");
		}
		public string Title {
			get => Instance.CallFunction("getTitle");
		}
		public string Votes {
			get => Instance.CallFunction("getVotes");
		}
		public string WritingCredits {
			get => Instance.CallFunction("getWritingCredits");
		}
		public int Year {
			get => Instance.CallFunction("getYear");
		}

		public InfoTagVideo(PyVariable instance) {
			this.Instance = instance;
		}

		public InfoTagVideo() {
			this.Instance = PyVariableManager.Get.NewVariable();
		}

		public void Dispose() {
			Instance.Dispose();
		}
	}
}
