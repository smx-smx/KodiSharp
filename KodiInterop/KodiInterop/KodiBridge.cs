using Newtonsoft.Json;
using System.Threading;
using System.Runtime.InteropServices;
using Smx.KodiInterop.Messages;
#if !UNIX
using RGiesecke.DllExport;
#endif
using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;

#if UNIX
using System.Configuration;
using System.Web.Configuration;
using System.Runtime.CompilerServices;
#endif

namespace Smx.KodiInterop
{
	/// <summary>
	/// Class to handle message passing from C# and Python
	/// </summary>
	public static class KodiBridge
	{
		/// <summary>
		/// Whether to unload the DLL when the Plugin is closed
		/// </summary>
		public static bool UnloadDLL = false;

		/// <summary>
		/// Cancellation Token for the periodic message queue flusher
		/// </summary>
		private static readonly CancellationTokenSource taskCts = new CancellationTokenSource();

		private static object MessageLock = new object();
		private static readonly ManualResetEvent RPCInitialized = new ManualResetEvent(false);

		private static readonly BlockingCollection<RPCRequest> MessageQueue = new BlockingCollection<RPCRequest>();
		private static readonly Task asyncMessageConsumer = new Task(new Action(_messageConsumer), taskCts.Token);

		public static readonly Dictionary<Type, List<object>> EventClasses = new Dictionary<Type, List<object>>();

		static KodiBridge()
		{
			asyncMessageConsumer.Start();
		}

		private static void _messageConsumer()
		{
			while (!taskCts.IsCancellationRequested) {
				try {
					RPCRequest req = MessageQueue.Take(taskCts.Token);
					//send the message, populate the reply and notify the listeners
					req.Send();
				} catch (OperationCanceledException) {
					break;
				}
			}
		}

		public static void RegisterEventClass(Type classType, object classInstance)
		{
			if (!EventClasses.ContainsKey(classType))
				EventClasses.Add(classType, new List<object>());
			EventClasses[classType].Add(classInstance);
		}

		/// <summary>
		/// The currently running addon
		/// </summary>
		public static KodiAddon RunningAddon = null;

		public static void SaveException(Exception ex){
			PyConsole.Write(ex.ToString());
		}

		/// <summary>
		/// Instructs python to abort message fetching
		/// </summary>
		private static void CloseRPC(){
			PythonExitMessage exitMessage = new PythonExitMessage();
			exitMessage.UnloadDLL = UnloadDLL;
			SendMessage(exitMessage);
		}

		public static string EncodeNonAsciiCharacters(string value){
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
		public static string SendMessage(RPCMessage message){
			RPCInitialized.WaitOne(); //Make sure the RPC is initialized before trying to use it

			string reply = null;
			lock (MessageLock) {
				string messageString = EncodeNonAsciiCharacters(JsonConvert.SerializeObject(message));
				reply = PySendMessage(messageString);
			}
			return reply;
		}

		public static void SendMessageAsync(RPCMessage message){
			MessageQueue.Add(new RPCRequest(message));
		}

		//http://stackoverflow.com/a/1373295
		private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args){
			string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
			if (!File.Exists(assemblyPath)) return null;
			Assembly assembly = Assembly.LoadFrom(assemblyPath);
			return assembly;
		}

		/// <summary>
		/// Set an Assembly resolver to lookup in the plugin current directory
		/// </summary>
		private static void SetAssemblyResolver(){
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
		}

		/// <summary>
		/// Set the Culture to be as close to python as possible
		/// </summary>
		private static void SetPythonCulture()
		{
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

		static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e){
			Console.WriteLine(" ---- UNHANDLED EXCEPTION ---- ");
			Console.WriteLine(e.ExceptionObject.ToString());
			Console.ReadLine();
			Environment.Exit(1);
		}

#if UNIX
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr PySendStringDelegate([MarshalAs(UnmanagedType.LPStr)] string messageData);
#else
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr PySendStringDelegate([MarshalAs(UnmanagedType.AnsiBStr)] string messageData);
#endif

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PyExitDelegate();

		private static PySendStringDelegate _pySendString;
		private static PyExitDelegate _pyExit;

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

		private static bool CommonInit() {
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
			SetAssemblyResolver();
			SetPythonCulture();
			RPCInitialized.Set();
			return true;
		}

#if UNIX
		private static bool Initialize(
			IntPtr sendMessageCallback,
			IntPtr exitCallback,
			bool enableDebug = false
		){
			if (RPCInitialized.WaitOne(0))
				return true; //already initialized

			Console.WriteLine("Ptr1: {0}", sendMessageCallback.ToString("x"));
			Console.WriteLine("Ptr2: {0}", exitCallback.ToString("x"));

			_pySendString = Marshal.GetDelegateForFunctionPointer(sendMessageCallback, typeof(PySendStringDelegate)) as PySendStringDelegate;
			_pyExit = Marshal.GetDelegateForFunctionPointer(exitCallback, typeof(PyExitDelegate)) as PyExitDelegate;
			return CommonInit();
		}
#else

			/// <summary>
			/// Called by Python to prepare the C# environment before running the plugin
			/// </summary>
			/// <returns></returns>
		[DllExport("Initialize", CallingConvention=CallingConvention.Cdecl)]
private static bool Initialize(
			[MarshalAs(UnmanagedType.FunctionPtr)] PySendStringDelegate sendMessageCallback,
			[MarshalAs(UnmanagedType.FunctionPtr)] PyExitDelegate exitCallback,
			bool enableDebug = false
		)
		{
#if DEBUG
			ConsoleHelper.CreateConsole();
			if (!Debugger.IsAttached && enableDebug) {
				Debugger.Launch();
			}
#endif

			//Console.WriteLine(string.Format("Function Pointer: 0x{0:X}", Marshal.GetFunctionPointerForDelegate(sendMessageCallback)));
			//Console.WriteLine(string.Format("Function Pointer: 0x{0:X}", Marshal.GetFunctionPointerForDelegate(exitCallback)));
			_pySendString = sendMessageCallback;
			_pyExit = exitCallback;

			return CommonInit();
		}
#endif

#if !UNIX
		[DllExport("PostEvent", CallingConvention=CallingConvention.Cdecl)]
#endif
		private static bool PostEvent([MarshalAs(UnmanagedType.AnsiBStr)] string eventMessage){
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
		/// Signals python to close RPC
		/// </summary>
		/// <returns></returns>
#if !UNIX
		[DllExport("StopRPC", CallingConvention = CallingConvention.Cdecl)]
#endif
		private static bool _StopRPC() {
			/* 
			 * this alias is needed to avoid "Method not Found" exception
			 * in PluginMain's "finally" block
			 * */
			return StopRPC();
		}

		public static bool StopRPC() {
			Console.WriteLine("Shutting Down...");
			taskCts.Cancel();
			asyncMessageConsumer.Wait();

			CloseRPC();
			RPCInitialized.Reset();
			return true;
		}
	}
}
