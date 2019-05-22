using Smx.KodiInterop;
using Smx.KodiInterop.Builtins;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Windows.Data.Json;
using Windows.Media.SpeechRecognition;

namespace SpeechRecognizerAddon
{
	public class SpeechRecognizerAddon : KodiAddon
    {
		public override string PluginId => "plugin.video.test";
		private static string ExitKeyword = Translate("exit", "en").Result;

		private ManualResetEvent exitEvent = new ManualResetEvent(false);

		public async void RecognizeSpeech() {
			var speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);

			var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "dictation");
			speechRecognizer.Constraints.Add(dictationConstraint);

			var srcpres = await speechRecognizer.CompileConstraintsAsync();
			if (srcpres.Status != SpeechRecognitionResultStatus.Success) {
				Console.WriteLine("Failed to compile constraints");
				exitEvent.Set();
				return;
			}

			while (true) {
				var res = await speechRecognizer.RecognizeAsync();
				switch (res.Status) {
					case SpeechRecognitionResultStatus.Success:
						break;
					default:
						Console.WriteLine($"Failed ({res.Status.ToString()}), try again");
						continue;
				}

				switch (res.Confidence) {
					case SpeechRecognitionConfidence.Low:
					case SpeechRecognitionConfidence.Rejected:
						Console.WriteLine("Not enough confidence...");
						continue;
				}

				UiBuiltins.Notification(
					header: "Text spoken",
					message: res.Text
				);

				if (res.Text == ExitKeyword) {
					exitEvent.Set();
					break;
				}
			}
		}

		public static async Task<string> Translate(string text, string sourceLang) {
			string uri = "https://translate.googleapis.com/translate_a/single?" +
				"client=gtx" +
				"&dt=t" +
				// source language
				"&sl=" + sourceLang +
				// target language
				"&tl=" + CultureInfo.CurrentUICulture.TwoLetterISOLanguageName +
				// query
				"&q=" + HttpUtility.UrlEncode(text);

			HttpWebRequest req = HttpWebRequest.CreateHttp(uri);
			req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";

			WebResponse resp = await req.GetResponseAsync();
			using (Stream strm = resp.GetResponseStream())
			using (StreamReader rdr = new StreamReader(strm)) {
				string data = rdr.ReadToEnd();
				JsonArray values = JsonArray.Parse(data);
				return values.GetArrayAt(0).GetArrayAt(0).GetStringAt(0);
			}

		}

		[Route("/")]
		public int PluginMain(NameValueCollection parameters) {
			ConsoleHelper.CreateConsole();

			Console.WriteLine(" == Recognizing == ");
			Console.WriteLine($"Say \"{ExitKeyword}\" to exit");
			UiBuiltins.Notification("Info", $"Say \"{ExitKeyword}\" to exit");
			RecognizeSpeech();

			exitEvent.WaitOne();
			return 0;
		}

		[PluginEntry]
		public static int PluginMain() {
			return GetInstance<SpeechRecognizerAddon>(typeof(SpeechRecognizerAddon), enableDebug: true, persist: true).Run();
		}
	}
}
