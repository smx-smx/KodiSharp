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
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop
{
	/// <summary>
	/// Class to handle message passing from C# and Python
	/// </summary>
	public static class KodiBridge {
		public static string LastMessage { get; private set; }
		public static string LastReply { get; private set;  }

		/// <summary>
		/// Whether to unload the DLL when the Plugin is closed
		/// </summary>
		public static bool UnloadDLL = false;

		private static AutoResetEvent MessageReady = new AutoResetEvent(false);
		private static AutoResetEvent ReplyReady = new AutoResetEvent(false);

		public static readonly Dictionary<Type, List<object>> EventClasses = new Dictionary<Type, List<object>>();

		public static void RegisterEventClass(Type classType, object classInstance) {
			if (!EventClasses.ContainsKey(classType))
				EventClasses.Add(classType, new List<object>());
			EventClasses[classType].Add(classInstance);
		}

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

		public static string EncodeNonAsciiCharacters(string value) {
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
		/// <param name="message">message object to send</param>
		/// <returns></returns>
		public static string SendMessage(RPCMessage message) {
			string messageString = EncodeNonAsciiCharacters( JsonConvert.SerializeObject(message) );
			// Check if we have a callback we can call
			if (_pySendString != null) {
				return PySendMessage(messageString);
			}

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

		static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) {
			Console.WriteLine(" ---- UNHANDLED EXCEPTION ---- ");
			Console.WriteLine(e.ExceptionObject.ToString());
			Console.ReadLine();
			Environment.Exit(1);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr PySendStringDelegate([MarshalAs(UnmanagedType.AnsiBStr)] string messageData);

		private static PySendStringDelegate _pySendString;

		/// <summary>
		/// </summary>
		/// <param name="messageData"></param>
		/// <returns></returns>
		// Wrapper to PySendMessageDelegate that does *not* free the string, like in MarshalAs (causing a python crash when it tries to free the string again)
		public static string PySendMessage(string messageData) {
			IntPtr pyStr = _pySendString(messageData);
			if (pyStr == IntPtr.Zero)
				return "";
			return Marshal.PtrToStringAnsi(pyStr);
		}

		/// <summary>
		/// Called by Python to prepare the C# environment before running the plugin
		/// </summary>
		/// <returns></returns>
		[DllExport("Initialize", CallingConvention=CallingConvention.Cdecl)]
		private static bool Initialize([MarshalAs(UnmanagedType.FunctionPtr)] PySendStringDelegate sendMessageCallback, bool enableDebug = false){
#if DEBUG
			ConsoleHelper.CreateConsole();
			if (!Debugger.IsAttached && enableDebug) {
				Debugger.Launch();
			}
#endif
			_pySendString = sendMessageCallback;
			Console.WriteLine(string.Format("Function Pointer: 0x{0:X}", Marshal.GetFunctionPointerForDelegate(sendMessageCallback)));

			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
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
			// Indicate that a reply is available for consumption
			ReplyReady.Set();
			return true;
		}

		[DllExport("PostEvent", CallingConvention=CallingConvention.Cdecl)]
		private static bool PostEvent([MarshalAs(UnmanagedType.AnsiBStr)] string eventMessage) {
			KodiEventMessage ev = JsonConvert.DeserializeObject<KodiEventMessage>(eventMessage);
			Type classType = Type.GetType(ev.Source);
			if (classType != null && EventClasses.ContainsKey(classType)) {
				List<object> instances = EventClasses[classType];
				foreach (object instance in instances) {
					var method = classType.GetMethod("onEvent", BindingFlags.NonPublic | BindingFlags.Instance);
					bool result = (bool)method.Invoke(instance, new object[] { ev });
				}
				return true;
			} else {
				return Modules.Xbmc.GlobalEvents.DispatchEvent(ev);
			}
		}

		/// <summary>
		/// Called by python to get the next message, in JSON format
		/// </summary>
		/// <returns></returns>
		[DllExport("GetMessage", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.AnsiBStr)]
		private static string GetMessage() {
			Debug.WriteLine("Waiting Message...");
			// Wait for the message to be populated
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
