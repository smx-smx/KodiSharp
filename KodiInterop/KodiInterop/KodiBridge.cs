using Newtonsoft.Json;
using System.Threading;
using System.Runtime.InteropServices;
using Smx.KodiInterop.Messages;
using RGiesecke.DllExport;
using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Smx.KodiInterop
{
	public static class KodiBridge {
		public static string LastMessage { get; private set; }
		public static string LastReply { get; private set;  }

		/// <summary>
		/// Whether to unload the DLL when the Plugin is closed
		/// </summary>
		public static bool UnloadDLL = false;

		private static AutoResetEvent MessageReady = new AutoResetEvent(false);
		private static AutoResetEvent ReplyReady = new AutoResetEvent(false);
		
		/// <summary>
		/// The currently running addon
		/// </summary>
		public static KodiAddon RunningAddon = null;

		public static void SaveException(Exception ex) {
			PyConsole.Write(ex.ToString());
		}

		/// <summary>
		/// Instructs python to abort message fetching
		/// </summary>
		private static void CloseRPC() {
			PythonExitMessage exitMessage = new PythonExitMessage();
			exitMessage.UnloadDLL = UnloadDLL;
			SendMessage(exitMessage);
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

		//http://stackoverflow.com/a/1373295
		private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args) {
			string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
			if (!File.Exists(assemblyPath)) return null;
			Assembly assembly = Assembly.LoadFrom(assemblyPath);
			return assembly;
		}

		/// <summary>
		/// Set an Assembly resolver to lookup in the plugin current directory
		/// </summary>
		private static void SetAssemblyResolver() {
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
		}

		/// <summary>
		/// Set the Culture to be as close to python as possible
		/// </summary>
		private static void SetPythonCulture() {
			// Clone the current Culture, and alter it as needed
			CultureInfo pythonCulture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
			pythonCulture.NumberFormat.NumberDecimalSeparator = ".";
			pythonCulture.NumberFormat.NaNSymbol = "nan";
			pythonCulture.NumberFormat.PositiveInfinitySymbol = "inf";
			pythonCulture.NumberFormat.NegativeInfinitySymbol = "-inf";

			// Change the current thread culture
			Thread.CurrentThread.CurrentCulture = pythonCulture;
			
			// Set the new culture for all new threads
			CultureInfo.DefaultThreadCurrentCulture = pythonCulture;
		}

		/// <summary>
		/// Called by Python to prepare the C# environment before running the plugin
		/// </summary>
		/// <returns></returns>
		[DllExport("Initialize", CallingConvention=CallingConvention.Cdecl)]
		private static bool Initialize() {
			SetAssemblyResolver();
			SetPythonCulture();
			return true;
		}

		/// <summary>
		/// Called by Python to send a message to C#
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[DllExport("PostMessage", CallingConvention=CallingConvention.Cdecl)]
		private static bool PostMessage([MarshalAs(UnmanagedType.AnsiBStr)] string message) {
			LastReply = message;
			ReplyReady.Set();
			return true;
		}

		[DllExport("PostEvent", CallingConvention=CallingConvention.Cdecl)]
		private static bool PostEvent([MarshalAs(UnmanagedType.AnsiBStr)] string eventMessage) {
			KodiEventMessage ev = JsonConvert.DeserializeObject<KodiEventMessage>(eventMessage);
			return Modules.Xbmc.Events.DispatchEvent(ev);

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
