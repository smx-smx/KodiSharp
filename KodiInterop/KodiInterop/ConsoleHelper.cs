using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace Smx.KodiInterop
{
	public static class ConsoleHelper
	{
		private const int STD_OUTPUT_HANDLE = -11;
		//private const int MY_CODE_PAGE = Encoding.Get;
		private static readonly int MY_CODE_PAGE = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;

		public static void CreateConsole() {
			AllocConsole();
			SetConsoleCtrlHandler(null, true);

			IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
			SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
			FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
			Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
			StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
			standardOutput.AutoFlush = true;
			Console.SetOut(standardOutput);
		}

		delegate bool HandlerRoutine(uint dwControlType);
		private const UInt32 StdOutputHandle = 0xFFFFFFF5;

		// P/Invoke required:
		[DllImport("kernel32")]
		static extern bool AllocConsole();
		[DllImport("kernel32")]
		public static extern bool FreeConsole();
		[DllImport("kernel32")]
		private static extern IntPtr GetStdHandle(int nStdHandle);
		[DllImport("kernel32")]
		private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
		[DllImport("kernel32")]
		static extern bool SetConsoleCtrlHandler(HandlerRoutine HandlerRoutine, bool Add);
	}
}
