using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using Smx.KodiInterop.Messages;
using RGiesecke.DllExport;
using System;
using System.Diagnostics;

namespace Smx.KodiInterop {
	public class KodiBridge {
		public static string LastMessage = null;
		public static string LastReply = null;

		private static AutoResetEvent MessageReady = new AutoResetEvent(false);
		private static AutoResetEvent ReplyReady = new AutoResetEvent(false);


		/// <summary>
		/// Instructs python to abort message fetching
		/// </summary>
		private static void CloseRPC() {
			RPCMessage amsg = new RPCMessage { MessageType = "exit" }; //quick hack
			SendMessage(amsg);
		}

		/// <summary>
		/// Sends an RPC message to python
		/// </summary>
		/// <param name="message">message object to send</param>
		/// <returns></returns>
		public static string SendMessage(RPCMessage message) {
			string reply = null;

			string messageString = JsonConvert.SerializeObject(message);
			Debug.WriteLine(messageString);

			LastMessage = messageString;
			MessageReady.Set();

			Debug.WriteLine("Waiting Reply...");
			ReplyReady.WaitOne();

			Debug.WriteLine(reply);
			return reply;
		}

		/// <summary>
		/// Called by python to send a message to C#
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[DllExport("PythonMessage", CallingConvention=CallingConvention.Cdecl)]
		private static bool PythonMessage([MarshalAs(UnmanagedType.AnsiBStr)] string message) {
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
