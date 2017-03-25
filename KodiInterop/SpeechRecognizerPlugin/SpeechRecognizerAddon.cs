using RGiesecke.DllExport;
using Smx.KodiInterop;
using Smx.KodiInterop.Builtins;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace SpeechRecognizerAddon
{
	public class SpeechRecognizerAddon : KodiAddon
    {
		public async Task<SpeechRecognitionResult> RecognizeSpeech() {
			var speechRecognizer = new SpeechRecognizer();
			await speechRecognizer.CompileConstraintsAsync();
			SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeAsync();
			return speechRecognitionResult;
		}

		public override int PluginMain() {
			ConsoleHelper.CreateConsole();

			while (true) {
				Console.WriteLine(" == Recognizing == ");
				Task<SpeechRecognitionResult> recognizer = RecognizeSpeech();
				SpeechRecognitionResult result = recognizer.Result;
				if(result == null) {
					Console.WriteLine("Couldn't recognize, try again");
					continue;
				}

				if (result.Text == "exit")
					break;

				UiBuiltins.Notification(
					header: "Text spoken",
					message: result.Text
				);
			}
			return 0;
		}

		[DllExport("PluginMain", CallingConvention = CallingConvention.Cdecl)]
		public static int Main() {
			return new SpeechRecognizerAddon().Run();
		}
	}
}
