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
			IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
			SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
			FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
			Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
			StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
			standardOutput.AutoFlush = true;
			Console.SetOut(standardOutput);
		}

		// P/Invoke required:
		private const UInt32 StdOutputHandle = 0xFFFFFFF5;
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetStdHandle(int nStdHandle);
		[DllImport("kernel32.dll")]
		private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
		[DllImport("kernel32")]
		static extern bool AllocConsole();
	}
}
