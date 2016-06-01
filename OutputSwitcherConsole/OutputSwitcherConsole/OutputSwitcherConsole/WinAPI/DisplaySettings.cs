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
            Console.WriteLine("Attached to Desktop? " + ((displayDevice.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) > 0 ? "Yes" : "No"));
            Console.WriteLine("Primary Device? " + ((displayDevice.StateFlags & DisplayDeviceStateFlags.PrimaryDevice) > 0 ? "Yes" : "No"));
            Console.WriteLine("Removable? " + ((displayDevice.StateFlags & DisplayDeviceStateFlags.Removable) > 0 ? "Yes" : "No"));
            Console.WriteLine();
        }
    }
}
