using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.KodiInterop.Python.ValueConverters
{
    public interface PythonConvertible
    {
        string ToPythonCode();
    }
}
