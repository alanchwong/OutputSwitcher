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
        static public int ENUM_CURRENT_SETTINGS = -1;
        static public int ENUM_REGISTRY_SETTINGS = 0;

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

        static public void WriteDisplayDeviceToConsole(DISPLAY_DEVICE displayDevice, uint devNum)
        {
            Console.WriteLine("Device #: " + devNum);
            Console.WriteLine("Device Name: " + displayDevice.DeviceName);
            Console.WriteLine("Device String: " + displayDevice.DeviceString);
            Console.WriteLine("Device ID: " + displayDevice.DeviceID);
            Console.WriteLine("Attached to Desktop? " + ((displayDevice.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) > 0 ? "Yes" : "No"));
            Console.WriteLine("Primary Device? " + ((displayDevice.StateFlags & DisplayDeviceStateFlags.PrimaryDevice) > 0 ? "Yes" : "No"));
            Console.WriteLine("Removable? " + ((displayDevice.StateFlags & DisplayDeviceStateFlags.Removable) > 0 ? "Yes" : "No"));
            Console.WriteLine();
        }

        static public void WriteDisplayDeviceSettingsToConsole(DEVMODE devMode, string deviceName)
        {
            Console.WriteLine("Device Name: " + deviceName);
            Console.WriteLine("Friendly Name: " + devMode.dmDeviceName);
            Console.WriteLine("PelsWidth: " + devMode.dmPelsWidth);
            Console.WriteLine("PelsHeight: " + devMode.dmPelsHeight);
            Console.WriteLine("Position X: " + devMode.dmPosition.x + ", Y: " + devMode.dmPosition.y);
            Console.WriteLine("Is Primary: " + ((devMode.dmPosition.x == 0 && devMode.dmPosition.y == 0) ? "Yes" : "No"));
            Console.WriteLine();
        }
    }
}
