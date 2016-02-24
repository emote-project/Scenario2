using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EmoteScenario2Gui
{
    class WindowsLayoutEr
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner  
            public int Top;         // y position of upper-left corner  
            public int Right;       // x position of lower-right corner  
            public int Bottom;      // y position of lower-right corner  
        }

        public class WindowSizeAndPosition
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Heigh { get; set; }
        }

        public static async void RepositionWindow(Process process, int x, int y, int width, int heigh)
        {
            while ((int)process.MainWindowHandle == 0)
            {
                await Task.Delay(100);
            }
            IntPtr hWnd = process.MainWindowHandle;
            while (!IsWindowVisible(hWnd))
            {
                await Task.Delay(100);
            }
            MoveWindow(hWnd, x, y, width, heigh, true);
        }

        public static WindowSizeAndPosition GetSizeAndPosition(Process process)
        {
            IntPtr hWnd = process.MainWindowHandle;
            RECT r;
            GetWindowRect(hWnd, out r);
            return new WindowSizeAndPosition() { X = r.Left, Y = r.Top, Width = r.Right - r.Left, Heigh = r.Bottom - r.Top };
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

    }
}
