using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop
{
	public class KodiWindowsBridge : KodiAbstractBridge
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public new delegate IntPtr PySendStringDelegate([MarshalAs(UnmanagedType.AnsiBStr)] string messageData);

		private readonly PySendStringDelegate _delegate;

		protected override Delegate PySendString {
			get => _delegate;
		}

		public KodiWindowsBridge(IntPtr pySendStringPtr) {
			_delegate = Marshal.GetDelegateForFunctionPointer<PySendStringDelegate>(pySendStringPtr);
		}
	}
}
