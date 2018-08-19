using Smx.KodiInterop.Messages;
using Smx.KodiInterop.Modules.XbmcGui;
using Smx.KodiInterop.Python;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Smx.KodiInterop.Modules.Xbmc
{
	public class XbmcPlayer : IDisposable, IKodiEventConsumer
	{
		public readonly PyVariable Instance;

		private int _subtitleStream;
		private int _videoStream;
		private int _audioStream;

		public event EventHandler<EventArgs> PlayBackEnded;
		public event EventHandler<EventArgs> PlayBackPaused;
		public event EventHandler<EventArgs> PlayBackResumed;
		public event EventHandler<PlayBackSeekEventArgs> PlayBackSeek;
		public event EventHandler<PlayBackSeekChapterEventArgs> PlayBackSeekChapter;
		public event EventHandler<PlayBackSpeedChangedEventArgs> PlayBackSpeedChanged;
		public event EventHandler<EventArgs> PlayBackStarted;
		public event EventHandler<EventArgs> PlayBackStopped;
		public event EventHandler<EventArgs> QueueNextItem;

		public bool TriggerEvent(KodiEventMessage e) {
			switch (e.Source) {
				case "onPlayBackEnded":
					PlayBackEnded?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackPaused":
					PlayBackPaused?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackResumed":
					PlayBackResumed?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackSeek":
					PlayBackSeek?.Invoke(null, new PlayBackSeekEventArgs(Convert.ToInt32(e.EventArgs[0]), Convert.ToInt32(e.EventArgs[1])));
					break;
				case "onPlayBackSeekChapter":
					PlayBackSeekChapter?.Invoke(null, new PlayBackSeekChapterEventArgs(Convert.ToInt32(e.EventArgs[0])));
					break;
				case "onPlayBackSpeedChanged":
					PlayBackSpeedChanged?.Invoke(null, new PlayBackSpeedChangedEventArgs(Convert.ToInt32(e.EventArgs[0])));
					break;
				case "onPlayBackStarted":
					PlayBackStarted?.Invoke(null, new EventArgs());
					break;
				case "onPlayBackStopped":
					PlayBackStopped?.Invoke(null, new EventArgs());
					break;
				case "onQueueNextItem":
					QueueNextItem?.Invoke(null, new EventArgs());
					break;
				default:
					PyConsole.WriteLine(string.Format("Unknown event '{0}' not handled", e.Source));
					return false;
			}
			return true;
		}

		public XbmcPlayer() {
			this.Instance = PyVariableManager.Get.NewVariable(evalCode: "self.new_player()");

			// We now register this type so that PostEvent will be able to invoke onMessage in this class
			Console.WriteLine("=> Registering EventClass " + typeof(XbmcPlayer).FullName);
			KodiBridge.RegisterPlayer(this);
		}

		public XbmcPlayer(PyVariable player) {
			PyVariableManager.Get.CopyVariable(Instance, player);
		}

		public int SubtitleStream {
			get {
				return _subtitleStream;
			}
			set {
				_subtitleStream = value;
				Instance.CallFunction("setSubtitleStream", new List<object> {
					_subtitleStream
				});
			}
		}

		public int VideoStream {
			get {
				return _videoStream;
			}
			set {
				_videoStream = value;
				Instance.CallFunction("setVideoStream", new List<object> {
					_videoStream
				});
			}
		}

		public int AudioStream {
			get {
				return _audioStream;
			}
			set {
				_audioStream = value;
				Instance.CallFunction("setAudioStream", new List<object> {
					_audioStream
				});
			}
		}

		public string PlayingFile {
			get {
				return Instance.CallFunction("getPlayingFile");
			}
		}

		public InfoTagRadioRDS RadioRDSInfoTag {
			get {
				var InfoTag = PyVariableManager.Get.NewVariable();
				Instance.CallAssign("getRadioRDSInfoTag", target: InfoTag);
				return new InfoTagRadioRDS(InfoTag);
			}
		}

		public InfoTagMusic MusicInfoTag {
			get {
				var InfoTag = PyVariableManager.Get.NewVariable();
				Instance.CallAssign("getMusicInfoTag", target: InfoTag);
				return new InfoTagMusic(InfoTag);
			}
		}

		public InfoTagVideo VideoInfoTag {
			get {
				var InfoTag = PyVariableManager.Get.NewVariable();
				Instance.CallAssign("getVideoInfoTag", target: InfoTag);
				return new InfoTagVideo(InfoTag);
			}
		}

		public double PlayTime {
			get {
				return Convert.ToDouble(Instance.CallFunction("getTime"));
			}
		}

		public double TotalPlayTime {
			get {
				return Convert.ToDouble(Instance.CallFunction("getTotalTime"));
			}
		}

		public bool IsPlaying {
			get {
				return Convert.ToBoolean(Instance.CallFunction("isPlaying"));
			}
		}

		public bool IsPlayingAudio {
			get {
				return Convert.ToBoolean(Instance.CallFunction("isPlayingAudio"));
			}
		}

		public bool IsPlayingRDS {
			get {
				return Convert.ToBoolean(Instance.CallFunction("isPlayingRDS"));
			}
		}

		public bool IsPlayingVideo {
			get {
				return Convert.ToBoolean(Instance.CallFunction("isPlayingVideo"));
			}
		}

		public void Pause() {
			Instance.CallFunction("pause");
		}

		public void Stop() {
			Instance.CallFunction("stop");
		}

		public void Play(
			string item = null,
			ListItem listItem = null,
			bool? windowed = null,
			int? startPos = null
		) {
			Instance.CallFunction("play", new List<object> {
				item,
				listItem?.Instance,
				windowed,
				startPos
			});
		}

		public void Play(
			PlayList item = null,
			ListItem listItem = null,
			bool? windowed = null,
			int? startPos = null
		) {
			Instance.CallFunction("play", new List<object> {
				item?.Instance,
				listItem?.Instance,
				windowed,
				startPos
			});
		}

		public void PlayNext() {
			Instance.CallFunction("playnext");
		}

		public void PlayPrevious() {
			Instance.CallFunction("playprevious");
		}

		public void PlaySelected(int item) {
			Instance.CallFunction("playselected", new List<object> { item });
		}

		public void Seek(TimeSpan seekTime) {
			Instance.CallFunction("seekTime", new List<object> {
				seekTime.TotalSeconds
			});
		}

		public void Dispose()
		{
			Instance.Dispose();
			KodiBridge.UnregisterEventClass(this);
		}
	}
}
