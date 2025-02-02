
namespace Smx.KodiInterop.Builtins
{
    public static class Dialog
    {
        public static void Close(string dialogName, bool force){
            PythonInterop.CallBuiltin("Dialog.Close", dialogName, force);
        }

        public static void CloseAll(bool force) => Close("all", force);
    }
}