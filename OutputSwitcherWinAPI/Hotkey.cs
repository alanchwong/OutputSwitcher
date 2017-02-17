using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcher.WinAPI
{
    public class Hotkey
    {
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hwnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hwnd, int id);
    }
}
