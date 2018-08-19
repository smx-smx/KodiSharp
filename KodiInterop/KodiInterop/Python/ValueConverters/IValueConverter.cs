using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python.ValueConverters
{
    public interface IValueConverter<TLocal>
    {
        TLocal FromPythonCode(string value);
        string ToPythonCode(TLocal value);
    }
}
