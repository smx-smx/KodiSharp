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
	public class Player
    {
		public readonly PyVariable Instance;

		private int _subtitleStream;
		private int _videoStream;
		private int _audioStream;

		private string[] eventNames = {
			"onPlayBackEnded",
			"onPlayBackPaused",
			"onPlayBackResumed",
			"onPlayBackSeek",
			"onPlayBackSeekChapter",
			"onPlayBackSpeedChanged",
			"onPlayBackStarted",
			"onQueueNextItem"
		};

		public event EventHandler<EventArgs> PlayBackEnded;
		public event EventHandler<EventArgs> PlayBackPaused;
		public event EventHandler<EventArgs> PlayBackResumed;
		public event EventHandler<PlayBackSeekEventArgs> PlayBackSeek;
		public event EventHandler<PlayBackSeekChapterEventArgs> PlayBackSeekChapter;
		public event EventHandler<PlayBackSpeedChangedEventArgs> PlayBackSpeedChanged;
		public event EventHandler<EventArgs> PlayBackStarted;
		public event EventHandler<EventArgs> PlayBackStopped;
		public event EventHandler<EventArgs> QueueNextItem;

		private bool onEvent(KodiEventMessage e) {
			switch (e.Sender) {
				case "onPlayBackEnded":
					PlayBackEnded?.Invoke(e.Sender, new EventArgs());
					break;
				case "onPlayBackPaused":
					PlayBackPaused?.Invoke(e.Sender, new EventArgs());
					break;
				case "onPlayBackResumed":
					PlayBackResumed?.Invoke(e.Sender, new EventArgs());
					break;
				case "onPlayBackSeek":
					PlayBackSeek?.Invoke(e.Sender, new PlayBackSeekEventArgs(int.Parse(e.EventArgs[0]), int.Parse(e.EventArgs[1])));
					break;
				case "onPlayBackSeekChapter":
					PlayBackSeekChapter?.Invoke(e.Sender, new PlayBackSeekChapterEventArgs(int.Parse(e.EventArgs[0])));
					break;
				case "onPlayBackSpeedChanged":
					PlayBackSpeedChanged?.Invoke(e.Sender, new PlayBackSpeedChangedEventArgs(int.Parse(e.EventArgs[0])));
					break;
				case "onPlayBackStarted":
					PlayBackStarted?.Invoke(e.Sender, new EventArgs());
					break;
				case "onPlayBackStopped":
					PlayBackStopped?.Invoke(e.Sender, new EventArgs());
					break;
				case "onQueueNextItem":
					QueueNextItem?.Invoke(e.Sender, new EventArgs());
					break;
				default:
					PyConsole.WriteLine(string.Format("Unknown event '{0}' not handled", e.Sender));
					return false;
			}
			return true;
		}

		public Player() {
			PyEventClassBuilder cb = new PyEventClassBuilder("xbmc.Player", typeof(Player));
			cb.Methods.AddRange(this.eventNames);
			cb.Install();
			this.Instance = cb.NewInstance(flags: PyVariableFlags.Player);
			Console.WriteLine("=> Registering EventClass " + typeof(Player).FullName);
			KodiBridge.RegisterEventClass(typeof(Player), this);
		}

		public Player(PyVariable player) {
			PyVariableManager.CopyVariable(Instance, player);
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
				var InfoTag = PyVariableManager.NewVariable(flags: PyVariableFlags.Object);
				Instance.CallFunction("getRadioRDSInfoTag");
				PyVariableManager.CopyVariable(InfoTag, PyVariableManager.LastResult);
				return new InfoTagRadioRDS(InfoTag);
			}
		}

		public InfoTagMusic MusicInfoTag {
			get {
				var InfoTag = PyVariableManager.NewVariable(flags: PyVariableFlags.Object);
				Instance.CallFunction("getMusicInfoTag");
				PyVariableManager.CopyVariable(InfoTag, PyVariableManager.LastResult);
				return new InfoTagMusic(InfoTag);
			}
		}

		public InfoTagVideo VideoInfoTag {
			get {
				var InfoTag = PyVariableManager.NewVariable(flags: PyVariableFlags.Object);
				Instance.CallFunction("getVideoInfoTag");
				PyVariableManager.CopyVariable(InfoTag, PyVariableManager.LastResult);
				return new InfoTagVideo(InfoTag);
			}
		}

		public double PlayTime {
			get {
				return double.Parse(Instance.CallFunction("getTime"));
			}
		}

		public double TotalPlayTime {
			get {
				return double.Parse(Instance.CallFunction("getTotalTime"));
			}
		}

		public bool IsPlaying {
			get {
				return bool.Parse(Instance.CallFunction("isPlaying"));
			}
		}

		public bool IsPlayingAudio {
			get {
				return bool.Parse(Instance.CallFunction("isPlayingAudio"));
			}
		}

		public bool IsPlayingRDS {
			get {
				return bool.Parse(Instance.CallFunction("isPlayingRDS"));
			}
		}

		public bool IsPlayingVideo {
			get {
				return bool.Parse(Instance.CallFunction("isPlayingVideo"));
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
    }
}
