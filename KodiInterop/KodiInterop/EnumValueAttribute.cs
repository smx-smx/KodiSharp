using System;

namespace Smx.KodiInterop
{
	internal class StringValueAttribute : Attribute
	{
		public string Value { get; private set; }

		public StringValueAttribute(string value) {
			this.Value = value;
		}
	}
}