using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Smx.KodiInterop
{
	/// <summary>
	/// This class is invoked from native code
	/// </summary>
	public class KodiBridgeABI : IKodiBridge
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public delegate IntPtr PySendStringDelegate(
			[MarshalAs(UnmanagedType.LPStr)] string messageData);

		private readonly PySendStringDelegate _delegate;


		/// <summary>
		/// Wrapper to PySendMessageDelegate that does *not* free the string, like in MarshalAs (causing a python crash when it tries to free the string again)
		/// </summary>
		/// <param name="messageData"></param>
		/// <returns></returns>
		public string PySendMessage(string messageData, bool replyExpected = true) {
			IntPtr pyStr = _delegate(messageData);
			if (pyStr == IntPtr.Zero) return "";
			if(replyExpected){
				return Marshal.PtrToStringAnsi(pyStr);
			}
			return string.Empty;
		}
			
		public KodiBridgeABI(IntPtr pySendStringFuncPtr) {
			_delegate = Marshal.GetDelegateForFunctionPointer<PySendStringDelegate>(pySendStringFuncPtr);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool InitializeDelegate(IntPtr sendMessageFuncPtr, IntPtr exitFuncPtr, bool enableDebug);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool PostEventDelegate([MarshalAs(UnmanagedType.LPStr)] string message);

		public static IntPtr GetInitializeFunc() => Marshal.GetFunctionPointerForDelegate(
			new InitializeDelegate(KodiBridge.Initialize)
		);

		public static IntPtr GetPostEventFunc() => Marshal.GetFunctionPointerForDelegate(
			new PostEventDelegate(KodiBridge.PostEvent)
		);

		public delegate int PluginMainDelegate();

		public static PluginMainDelegate? FindPluginMain(Assembly asm) {
			var pluginEntry = asm.GetTypes()
					.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
					.Where(m => m.GetCustomAttribute<PluginEntryAttribute>() != null)
					.FirstOrDefault();
			if (pluginEntry == null)
			{
				return null;
			}
			return (PluginMainDelegate?)Delegate.CreateDelegate(typeof(PluginMainDelegate), null, pluginEntry);
		}

		private static string[] ReadArgv(IntPtr args, int sizeBytes) {
			int nargs = sizeBytes / IntPtr.Size;
			string[] argv = new string[nargs];

			for (int i = 0; i < nargs; i++, args += IntPtr.Size) {
				IntPtr charPtr = Marshal.ReadIntPtr(args);
				argv[i] = Marshal.PtrToStringAnsi(charPtr);
			}
			return argv;
		}

		private static nint ParsePointer(string str) {
    		return (nint)Convert.ToUInt64(str, 16);
		}

		public static int Entry(IntPtr args, int sizeBytes) {
			string[] argv = ReadArgv(args, sizeBytes);
			if (argv.Length < 4) {
				return 1;
			}

			var assemblyPath = argv[0];
			var onMessageDelegate = ParsePointer(argv[1]);
			var onExitDelegate = ParsePointer(argv[2]);
			var postEventFptr = ParsePointer(argv[3]);
			var enableDebug = argv.Length > 4 && argv[4] == "1";
			if (!KodiBridge.Initialize(onMessageDelegate, onExitDelegate, enableDebug))
			{
				return 1;
			}
			Marshal.WriteIntPtr(
				postEventFptr,
				Marshal.GetFunctionPointerForDelegate(new PostEventDelegate(KodiBridge.PostEvent))
			);

			var asm = Assembly.LoadFrom(assemblyPath);
			var pluginMain = FindPluginMain(asm);
			if (pluginMain == null)
			{
				Console.Error.WriteLine("PluginMain not found");
				return 1;
			}
			return pluginMain();
		}
		
		public static void Main(string[] args){
			Console.Error.WriteLine("This Addon can only be ran inside Kodi");
			Environment.Exit(1);
		}
	}
}
