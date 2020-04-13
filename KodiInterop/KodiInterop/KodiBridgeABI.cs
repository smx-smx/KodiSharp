using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop
{
	/// <summary>
	/// This class is invoked from native code
	/// </summary>
	public class KodiBridgeABI : KodiAbstractBridge
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public new delegate IntPtr PySendStringDelegate([MarshalAs(UnmanagedType.LPStr)] string messageData);

		private readonly PySendStringDelegate _delegate;

		protected override Delegate PySendString {
			get => _delegate;
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int PluginMainDelegate();

		public static MethodInfo FindPluginMain() {
			Assembly thisAsm = Assembly.GetExecutingAssembly();

			var assemblies = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(asm => asm != thisAsm);

			MethodInfo pluginEntry = null;
			foreach (Assembly asm in assemblies) {
				pluginEntry = asm.GetTypes()
					.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
					.Where(m => m.GetCustomAttribute<PluginEntryAttribute>() != null)
					.FirstOrDefault();
				if (pluginEntry != null)
					break;
			}

			return pluginEntry;
		}

		public static IntPtr GetPluginMainFunc(
			IntPtr sendMessageCallbackPtr,
			IntPtr exitCallbackPtr,
			bool enableDebug = false
		) {
			KodiBridge.Initialize(sendMessageCallbackPtr, exitCallbackPtr, enableDebug);

			MethodInfo pluginEntry = FindPluginMain();
			return Marshal.GetFunctionPointerForDelegate(pluginEntry.CreateDelegate(typeof(PluginMainDelegate)));
		}
	}
}
