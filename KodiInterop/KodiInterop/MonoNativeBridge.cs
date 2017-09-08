using System;
using System.Runtime.CompilerServices;

namespace Smx.KodiInterop
{
	public static class MonoNativeBridge
	{
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern static string PySendString(string messageData);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern static void PyExit();
	}
}
