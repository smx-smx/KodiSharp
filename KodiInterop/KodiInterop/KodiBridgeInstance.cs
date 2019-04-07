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
	public delegate void PyMessageDelegate(string reply);

	public class KodiBridgeInstance
	{
		private object MessageLock = new object();
		public readonly Dictionary<Type, List<IKodiEventConsumer>> EventClasses = new Dictionary<Type, List<IKodiEventConsumer>>();

		private readonly IKodiBridge nativeBridge;
		private readonly PyExitDelegate exitCallback;


		public KodiBridgeInstance(IntPtr sendStringFuncPtr, IntPtr exitFuncPtr) {
			this.nativeBridge = new KodiBridgeABI(sendStringFuncPtr);
			this.exitCallback = Marshal.GetDelegateForFunctionPointer<PyExitDelegate>(exitFuncPtr);
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
			SendMessage(exitMessage);
		}

		public bool StopRPC(bool UnloadDll) {
			Console.WriteLine("Shutting Down...");
			CloseRPC(UnloadDLL);
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
		public string SendMessage(RPCMessage message) {
			string messageString = EncodeNonAsciiCharacters(JsonConvert.SerializeObject(message));
			string reply;
			lock (MessageLock) {
				reply = nativeBridge.PySendMessage(messageString);
			}
			return reply;
		}
	}
}
