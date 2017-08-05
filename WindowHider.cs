using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowHider
{
    class WindowHider
    {
        public class Window
        {
            public string Title;
            public IntPtr hwnd;

            public Window(string _title, IntPtr _hwnd)
            {
                Title = _title;
                hwnd = _hwnd;
            }
        }

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);



        public static Window[] GetAllWindows()
        {
            List<Window> windows = new List<Window>();
            GCHandle handle = GCHandle.Alloc(windows);
            try
            {
                Win32Callback callback = new Win32Callback(InsertWindowToList);
                EnumChildWindows(IntPtr.Zero, callback, GCHandle.ToIntPtr(handle));
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }

            return windows.ToArray();
        }

        public static bool InsertWindowToList(IntPtr hwnd, IntPtr lparam)
        {
            List<Window> windows = GCHandle.FromIntPtr(lparam).Target as List<Window>;

            StringBuilder builder = new StringBuilder(500);
            GetWindowText(hwnd, builder, builder.Capacity);
            string title = builder.ToString();
            windows.Add(new Window(title, hwnd));

            return true;
        }

        public static void HideWindows(string namepart)
        {
            Window[] windows = WindowHider.GetAllWindows();

            foreach (Window w in windows)
            {
                if(w.Title.Contains(namepart))
                {
                    HideWindow(w);
                }
            }
        }

        public static void HideWindow(Window window)
        {
            int SW_HIDE = 0;
            int SW_SHOW = 5;

            ShowWindow(window.hwnd, SW_HIDE);
        }
    }
}
