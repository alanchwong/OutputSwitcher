using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OutputSwitcherConsole.WinAPI
{
    class DisplaySettings
    {
        static public readonly int ENUM_CURRENT_SETTINGS = -1;
        static public readonly int ENUM_REGISTRY_SETTINGS = 0;

        [DllImport("user32.dll")]
        static extern public bool EnumDisplayDevices(
            string lpDevice,
            uint iDevNum,
            ref DISPLAY_DEVICE lpDisplayDevice,
            uint dwFlags);

        [DllImport("user32.dll")]
        static extern public bool EnumDisplaySettingsEx(
            string lpszDeviceName,
            int iModeNum,
            ref DEVMODE lpDevMode,
            uint dwFlags);

        [DllImport("user32.dll")]
        public static extern DISP_CHANGE ChangeDisplaySettings(ref DEVMODE devMode, ChangeDisplaySettingsFlags flags);

        [DllImport("user32.dll")]
        public static extern DISP_CHANGE ChangeDisplaySettings(IntPtr devMode, ChangeDisplaySettingsFlags flags);

        [DllImport("user32.dll")]
        static extern public DISP_CHANGE ChangeDisplaySettingsEx(
            string lpszDeviceName,
            ref DEVMODE lpDevMode,
            IntPtr hwnd,
            ChangeDisplaySettingsFlags dwflags,
            IntPtr lParam);

        [DllImport("user32.dll")]
        static extern public DISP_CHANGE ChangeDisplaySettingsEx(
            IntPtr lpszDeviceName,
            IntPtr lpDevMode,
            IntPtr hwnd,
            ChangeDisplaySettingsFlags dwflags,
            IntPtr lParam);

        static public bool IsDisplayPrimary(DEVMODE devMode)
        {
            return (devMode.dmPosition.x == 0 &&
                    devMode.dmPosition.y == 0 &&
                    devMode.dmPelsHeight != 0 &&
                    devMode.dmPelsWidth != 0);
        }

        static public bool IsDisplayAttachedToDesktop(DEVMODE devMode)
        {
            return (devMode.dmPelsHeight != 0 && devMode.dmPelsWidth != 0);
        }

        /// <summary>
        /// Return an empty DEVMODE with dmSize set properly and dmDriverExtra set to 0.
        /// </summary>
        /// <returns>New empty DEVMODE ready to use.</returns>
        static public DEVMODE MakeNewDevmode()
        {
            DEVMODE devMode = new DEVMODE();
            devMode.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(devMode);
            devMode.dmDriverExtra = 0;
            return devMode;
        }

        static public DISPLAY_DEVICE MakeNewDisplayDevice()
        {
            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);
            return displayDevice;
        }
    }
}
