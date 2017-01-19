using System;

namespace Smx.KodiInterop.Builtins
{
	internal class KodiMinApiVersionAttribute : Attribute
	{
		private int apiVersion;

		public KodiMinApiVersionAttribute(int apiVersion) {
			this.apiVersion = apiVersion;
		}
	}

	internal class KodiMaxApiVersionAttribute : Attribute
	{
		private int apiVersion;

		public KodiMaxApiVersionAttribute(int apiVersion) {
			this.apiVersion = apiVersion;
		}
	}
}