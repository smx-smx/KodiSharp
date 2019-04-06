using Newtonsoft.Json;
using Smx.KodiInterop.Messages;
using Smx.KodiInterop.Modules.Xbmc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Smx.KodiInterop.KodiBridge;

namespace Smx.KodiInterop
{
	internal struct RPCRequest
	{
		public RPCMessage message;
		public PyMessageDelegate onReply;
	}

	public delegate void PyMessageDelegate(string reply);

	public class KodiBridgeInstance
	{
		/// <summary>
		/// Cancellation Token for the periodic message queue flusher
		/// </summary>
		private readonly CancellationTokenSource taskCts = new CancellationTokenSource();

		private object MessageLock = new object();

		private readonly ManualResetEvent RPCCanSend = new ManualResetEvent(false);

		private readonly BlockingCollection<RPCRequest> MessageQueue = new BlockingCollection<RPCRequest>();
		private readonly Task asyncMessageConsumer;

		public readonly Dictionary<Type, List<IKodiEventConsumer>> EventClasses = new Dictionary<Type, List<IKodiEventConsumer>>();

		//private readonly PySendStringDelegate sendMessageCallback;
		private readonly IKodiBridge PlatformSendString;

		private readonly PyExitDelegate exitCallback;


		public KodiBridgeInstance(IntPtr sendStringFuncPtr, IntPtr exitFuncPtr) {
			this.PlatformSendString = new KodiBridgeABI(sendStringFuncPtr);
			this.exitCallback = Marshal.GetDelegateForFunctionPointer<PyExitDelegate>(exitFuncPtr);

			asyncMessageConsumer = new Task(new Action(_messageConsumer), taskCts.Token);
			asyncMessageConsumer.Start();
			RPCCanSend.Set();
		}

		private void _messageConsumer() {
			while (!taskCts.IsCancellationRequested) {
				try {
					RPCRequest req = MessageQueue.Take(taskCts.Token);
					SendMessage(req);
				} catch (OperationCanceledException) {
					break;
				}
			}
		}

		private void RegisterEventClass(IKodiEventConsumer classInstance) {
			Type classType = classInstance.GetType();
			if (!EventClasses.ContainsKey(classType))
				EventClasses.Add(classType, new List<IKodiEventConsumer>());
			EventClasses[classType].Add(classInstance);
		}

		public void UnregisterEventClass(IKodiEventConsumer classInstance) {
			Type classType = classInstance.GetType();
			EventClasses[classType].Remove(classInstance);
		}

		public void RegisterMonitor(XbmcMonitor monitor) => RegisterEventClass(monitor);
		public void RegisterPlayer(XbmcPlayer player) => RegisterEventClass(player);

		public void SaveException(Exception ex) {
			PyConsole.Write(ex.ToString());
		}

		/// <summary>
		/// Instructs python to abort message fetching
		/// </summary>
		private void CloseRPC(bool UnloadDll) {
			PythonExitMessage exitMessage = new PythonExitMessage();
			exitMessage.UnloadDLL = UnloadDLL;
			SendMessage(new RPCRequest {
				message = exitMessage,
				onReply = (reply) => {
					RPCCanSend.Reset();
				},
			});
		}

		public bool StopRPC(bool UnloadDll) {
			Console.WriteLine("Shutting Down...");
			CloseRPC(UnloadDLL);
			taskCts.Cancel();
			asyncMessageConsumer.Wait();
			Console.WriteLine("Done!");
			return true;
		}

		public string EncodeNonAsciiCharacters(string value) {
			StringBuilder sb = new StringBuilder();
			foreach (char c in value) {
				if (c > 127) {
					// This character is too big for ASCII
					string encodedValue = "\\u" + ((int)c).ToString("x4");
					sb.Append(encodedValue);
				} else {
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Sends an RPC message to python
		/// </summary>
		/// <param name="request">message object to send</param>
		/// <returns></returns>
		private string SendMessage(RPCRequest request) {
			string reply = null;
			lock (MessageLock) {
				RPCCanSend.WaitOne();
				RPCCanSend.Reset();

				string messageString = EncodeNonAsciiCharacters(JsonConvert.SerializeObject(request.message));
				reply = PlatformSendString.PySendMessage(messageString);

				request.onReply(reply);
			}
			return reply;
		}

		public string QueueMessage(RPCMessage message) {
			TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();

			PyMessageDelegate onReplyDelegate = (reply) => {
				RPCCanSend.Set();
				taskCompletionSource.SetResult(reply);
			};

			RPCRequest req = new RPCRequest {
				message = message,
				onReply = onReplyDelegate
			};
			MessageQueue.Add(req);

			return taskCompletionSource.Task.Result; 
		}
	}
}
