using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop
{
	public class KodiMonoBridge : KodiAbstractBridge
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public new delegate IntPtr PySendStringDelegate([MarshalAs(UnmanagedType.LPStr)] string messageData);

		private readonly PySendStringDelegate _delegate;

		protected override Delegate PySendString {
			get => _delegate;
		}

		public KodiMonoBridge(IntPtr pySendStringFuncPtr) {
			_delegate = Marshal.GetDelegateForFunctionPointer<PySendStringDelegate>(pySendStringFuncPtr);
		}
	}
}
