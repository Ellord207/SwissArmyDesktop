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
        // Reference: http://www.pinvoke.net
        //   This site has almost to much information

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        internal const int SW_HIDE = 0;
        internal const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();  // Returns a hWand

        [DllImport("user32.dll")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd); // Returns a hWand

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);
        internal const int GW_HWNDFIRST = 0; // The highest window in the Z-order having the same parent as the given window.
        internal const int GW_HWNDLAST = 1; // The lowest window in the Z-order having the same parent as the given window.
        internal const int GW_HWNDNEXT = 2; // The window below the given window in the Z-order.
        internal const int GW_HWNDPREV = 3; // The window above the given window in the Z-order.
        internal const int GW_OWNER = 4; //The window that owns the given window(not to be confused with the parent window).
        internal const int GW_CHILD = 5; // The topmost of the given window's child windows. This has the same effect as using the GetTopWindow function.

        [DllImport("user32.dll")]
        internal static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        internal static extern Boolean BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        /// A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.
        /// <para>
        /// Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        internal struct WINDOWPLACEMENT
            {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
            }
        internal static WINDOWPLACEMENT GetPlacement(IntPtr hWnd)
            {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement;
            }

        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //internal static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
        //[StructLayout(LayoutKind.Sequential)]
        //internal struct RECT
        //    {
        //    public int Left;        // x position of upper-left corner
        //    public int Top;         // y position of upper-left corner
        //    public int Right;       // x position of lower-right corner
        //    public int Bottom;      // y position of lower-right corner
        //    }
        //internal static RECT GetRect(IntPtr hWnd)
        //    {
        //    RECT rect = new RECT();
        //    new HandleRef()
        //    GetWindowRect(hWnd, out rect);
        //    return rect;
        //    }



        /// <summary>
        ///     Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a
        ///     control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another
        ///     application.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633520%28v=vs.85%29.aspx  for more
        ///     information
        ///     </para>
        /// </summary>
        /// <param name="hWnd">
        ///     C++ ( hWnd [in]. Type: HWND )<br />A <see cref="IntPtr" /> handle to the window or control containing the text.
        /// </param>
        /// <param name="lpString">
        ///     C++ ( lpString [out]. Type: LPTSTR )<br />The <see cref="StringBuilder" /> buffer that will receive the text. If
        ///     the string is as long or longer than the buffer, the string is truncated and terminated with a null character.
        /// </param>
        /// <param name="nMaxCount">
        ///     C++ ( nMaxCount [in]. Type: int )<br /> Should be equivalent to
        ///     <see cref="StringBuilder.Length" /> after call returns. The <see cref="int" /> maximum number of characters to copy
        ///     to the buffer, including the null character. If the text exceeds this limit, it is truncated.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the length, in characters, of the copied string, not including
        ///     the terminating null character. If the window has no title bar or text, if the title bar is empty, or if the window
        ///     or control handle is invalid, the return value is zero. To get extended error information, call GetLastError.<br />
        ///     This function cannot retrieve the text of an edit control in another application.
        /// </returns>
        /// <remarks>
        ///     If the target window is owned by the current process, GetWindowText causes a WM_GETTEXT message to be sent to the
        ///     specified window or control. If the target window is owned by another process and has a caption, GetWindowText
        ///     retrieves the window caption text. If the window does not have a caption, the return value is a null string. This
        ///     behavior is by design. It allows applications to call GetWindowText without becoming unresponsive if the process
        ///     that owns the target window is not responding. However, if the target window is not responding and it belongs to
        ///     the calling application, GetWindowText will cause the calling application to become unresponsive. To retrieve the
        ///     text of a control in another process, send a WM_GETTEXT message directly instead of calling GetWindowText.<br />For
        ///     an example go to
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644928%28v=vs.85%29.aspx#sending">
        ///     Sending a
        ///     Message.
        ///     </see>
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        }
    }
