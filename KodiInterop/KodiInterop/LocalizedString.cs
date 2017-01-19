using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop
{
    class LocalizedString
    {
		private Dictionary<int, string> pair;
		public int Id { get; private set; }
		public string Value { get; private set; }

		public LocalizedString(int stringId, string defaultString = null) {
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
