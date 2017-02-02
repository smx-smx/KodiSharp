using Newtonsoft.Json;
using System.Threading;
using System.Runtime.InteropServices;
using Smx.KodiInterop.Messages;
using RGiesecke.DllExport;
using System;
using System.Diagnostics;

namespace Smx.KodiInterop
{
	public static class KodiBridge {
		public static string LastMessage = null;
		public static string LastReply = null;

		private static AutoResetEvent MessageReady = new AutoResetEvent(false);
		private static AutoResetEvent ReplyReady = new AutoResetEvent(false);
		
		/// <summary>
		/// The currently running addon
		/// </summary>
		public static KodiAddon RunningAddon;

		public static void SaveException(Exception ex) {
			PyConsole.Write(ex.ToString());
		}

		/// <summary>
		/// Instructs python to abort message fetching
		/// </summary>
		private static void CloseRPC() {
			RPCMessage amsg = new RPCMessage { MessageType = "exit" };
			SendMessage(amsg);
		}

		/// <summary>
		/// Sends an RPC message to python
		/// </summary>
		/// <param name="message">message object to send</param>
		/// <returns></returns>
		public static string SendMessage(RPCMessage message) {
			string messageString = JsonConvert.SerializeObject(message);
			Debug.WriteLine(messageString);

			LastMessage = messageString;
			MessageReady.Set();

			Debug.WriteLine("Waiting Reply...");
			ReplyReady.WaitOne();

			Debug.WriteLine(LastReply);
			return LastReply;
		}

		/// <summary>
		/// Called by python to send a message to C#
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[DllExport("PutMessage", CallingConvention=CallingConvention.Cdecl)]
		private static bool PutMessage([MarshalAs(UnmanagedType.AnsiBStr)] string message) {
			LastReply = message;
			ReplyReady.Set();
			return true;
		}

		/// <summary>
		/// Called by python to get the next message, in JSON format
		/// </summary>
		/// <returns></returns>
		[DllExport("GetMessage", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.AnsiBStr)]
		private static string GetMessage() {
			Debug.WriteLine("Waiting Message...");
			MessageReady.WaitOne();
			string message = LastMessage;
			Debug.WriteLine(string.Format("Giving {0}", message));
			return message;
		}

		/// <summary>
		/// Signals python to close RPC. Called by python itself to terminate
		/// </summary>
		/// <returns></returns>
		[DllExport("StopRPC", CallingConvention = CallingConvention.Cdecl)]
		private static bool StopRPC() {
			CloseRPC();
			return true;
		}
	}
}
