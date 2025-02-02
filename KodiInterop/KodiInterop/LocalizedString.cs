using System.Collections.Generic;

namespace Smx.KodiInterop
{
	class LocalizedString
    {
		public int Id { get; private set; }
		public string? Value { get; private set; }

		public LocalizedString(int stringId, string? defaultString = null) {
			this.Id = stringId;
			this.Value = defaultString;
		}

		public override string ToString() {
			string strVal = this.Id.ToString();
			if(this.Value != null) {
				strVal += this.Value;
			}
			return strVal;
		}
	}
}
