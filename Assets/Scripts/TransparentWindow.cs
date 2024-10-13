/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text;
using Interop.UIAutomationClient;

public class TransparentWindow : MonoBehaviour
{
    
    // P/Invoke for GetClassName
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


    // Import necessary WinAPI functions
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
    int nMaxCount);


    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    const int TB_BUTTONCOUNT = 0x0418; // Gets the number of buttons
    const int TB_GETBUTTON = 0x0417;   // Gets the button information


    [StructLayout(LayoutKind.Sequential)]
    public struct TBBUTTON
    {
        public int iBitmap;
        public int idCommand;
        public byte fsState;
        public byte fsStyle;
        public byte bReserved0;
        public byte bReserved1;
        public IntPtr dwData;
        public IntPtr iString;
    }

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    const int GWL_EXSTYLE = -20;

    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    const uint LWA_COLORKEY = 0x00000001;

    private IntPtr hWnd;

    public IntPtr TaskbarHandle;


    private void Awake()
    {
        //MessageBox(new IntPtr(0), "Hello World!", "Hello Dialog", 0);

#if !UNITY_EDITOR
        hWnd = GetActiveWindow();
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        //SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
        SetClickthrough(false);
        Application.runInBackground = true;
        TaskbarHandle = GetTaskbarHandle(hWnd);
    }

    public void SetClickthrough(bool clickthrough)
    {
#if !UNITY_EDITOR

        if (clickthrough)
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        else
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }
#endif
    }

    public static IntPtr GetTaskbarHandle()
    {
        return FindWindow("Shell_TrayWnd", null);
    }

    public static IntPtr GetTaskbarHandle(IntPtr hwnd)
    {
        IntPtr taskbarHwnd = GetTaskbarHandle();
        if (taskbarHwnd == IntPtr.Zero)
        {
            Console.WriteLine("Taskbar not found.");
            return IntPtr.Zero;
        }

        // Iterate over child windows of taskbar to find the button
        var str = new StringBuilder(256);
        GetWindowText(hwnd, str, 256);


        var hWndRebar = FindWindowEx(taskbarHwnd, IntPtr.Zero, "ReBarWindow32", null);
        var hWndMSTaskSwWClass = FindWindowEx(hWndRebar, IntPtr.Zero, "MSTaskSwWClass", null);
        var hWndMSTaskListWClass = FindWindowEx(hWndMSTaskSwWClass, IntPtr.Zero, "MSTaskListWClass", null);
        return hWndMSTaskListWClass;
    }

    public IUIAutomationElementArray GetTaskbarElementArray()
    {
        IntPtr hToolbar = FindWindowEx(TaskbarHandle, IntPtr.Zero, "ToolbarWindow32", null);

        IUIAutomation pUIAutomation = new CUIAutomation();

        // Taskbar
        IUIAutomationElement windowElement = pUIAutomation.ElementFromHandle(TaskbarHandle);
        IUIAutomationElementArray elementArray = null;
        if (windowElement != null)
        {
           
            IUIAutomationCondition condition = pUIAutomation.CreateTrueCondition();
            elementArray = windowElement.FindAll(TreeScope.TreeScope_Descendants | TreeScope.TreeScope_Children, condition);
        }
        return elementArray;
    }
}
