using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SwissArmyDesktop
    {
    public class ExternalAPIs
        {

        #region external Windows API
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetNextWindow(IntPtr hWnd, uint wCmd);

        [DllImport("user32.dll")]
        public static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern Boolean BringWindowToTop(IntPtr hWnd);
        #endregion
        }
    }
