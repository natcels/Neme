using System.Runtime.InteropServices;


namespace Neme.Utils
{
    public class ToastSupport
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string appID);

        public static void RegisterAppForToasts(string appId = "NemeApp")
        {
            SetCurrentProcessExplicitAppUserModelID(appId);
        }
    }
}
