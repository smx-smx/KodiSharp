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
using Smx.KodiInterop.Modules.Xbmc;

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

		private static readonly AutoResetEvent addonFinished = new AutoResetEvent(true);

		private static readonly Dictionary<string, KodiAddon> addonRefs = new Dictionary<string, KodiAddon>();
		private static WeakReference<KodiAddon> runningAddon = new WeakReference<KodiAddon>(null);

		public static KodiAddon GetPersistentAddon(string addonUrl) {
			if (addonRefs.TryGetValue(addonUrl, out KodiAddon instance))
				return instance;
			return null;
		}
		
		public static void RegisterPersistentAddon(string addonUrl, KodiAddon instance) {
			addonRefs[addonUrl] = instance;
		}

		public static void ScheduleAddonTermination(string addonUrl) {
			if (addonRefs.ContainsKey(addonUrl)) {
				// Remove the strong reference, allowing the addon to be Garbage collected
				addonRefs.Remove(addonUrl);
				GC.Collect();
			}
		}

		public static void SetRunningAddon(KodiAddon addonInstance) {
			if(addonInstance == null) {
				// Addon finished running, allow another addon to start
				addonFinished.Set();
			} else {
				// Wait for the previous addon to finish
				Console.WriteLine("!!! Waiting for previous addon to finish");
				addonFinished.WaitOne();
				Console.WriteLine("!!! Previous addon finished, starting up");
			}
			runningAddon.SetTarget(addonInstance);
		}

		/// <summary>
		/// The currently running addon
		/// </summary>
		public static KodiAddon RunningAddon {
			get {
				if (runningAddon.TryGetTarget(out KodiAddon target))
					return target;
				return null;
			}
		}

		public static void SaveException(Exception ex){
			PyConsole.Write(ex.ToString());
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

		public static KodiBridgeInstance GlobalStaticBridge { get; private set; }

		/// <summary>
		/// Wrapper to PySendMessageDelegate that does *not* free the string, like in MarshalAs (causing a python crash when it tries to free the string again)
		/// </summary>
		/// <param name="messageData"></param>
		/// <returns></returns>
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

			GlobalStaticBridge = CreateBridgeInstance();
			return true;
		}

#if UNIX
		private static bool Initialize(
			IntPtr sendMessageCallback,
			IntPtr exitCallback,
			bool enableDebug = false
		){
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

		public static KodiBridgeInstance CreateBridgeInstance() {
			return new KodiBridgeInstance(_pySendString, _pyExit);
		}

#if !UNIX
		[DllExport("PostEvent", CallingConvention=CallingConvention.Cdecl)]
#endif
		private static bool PostEvent([MarshalAs(UnmanagedType.AnsiBStr)] string eventMessage){
			KodiEventMessage ev = JsonConvert.DeserializeObject<KodiEventMessage>(eventMessage);
			Type classType;

			switch (ev.Sender)
			{
				case "Monitor":
					classType = typeof(XbmcMonitor);
					break;
				case "Player":
					classType = typeof(XbmcPlayer);
					break;
				default:
					return false;
			}

			KodiBridgeInstance bridge = RunningAddon.Bridge;
			if (!bridge.EventClasses.ContainsKey(classType))
			{
				return false;
			}

			List<IKodiEventConsumer> instances = bridge.EventClasses[classType];
			foreach (IKodiEventConsumer instance in instances) {
				instance.TriggerEvent(ev);
			}
			return true;
		}
	}
}
