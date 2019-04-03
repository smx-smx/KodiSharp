using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop
{
	public interface IKodiBridge
	{
		string PySendMessage(string messageData);
	}
}
