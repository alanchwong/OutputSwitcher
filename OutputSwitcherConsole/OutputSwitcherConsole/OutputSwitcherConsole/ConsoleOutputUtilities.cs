using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcherConsole.WinAPI;
using OutputSwitcherConsole.Data;

namespace OutputSwitcherConsole
{
    class ConsoleOutputUtilities
    {
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
            Console.WriteLine("Orientation: " + devMode.dmDisplayOrientation);
            Console.WriteLine("Is Primary: " + (DisplaySettings.IsDisplayPrimary(devMode) ? "Yes" : "No"));
            Console.WriteLine();
        }

        static public void WriteDisplayDeviceSettingsToConsole(DisplayDeviceSettings displaySettings)
        {
            Console.WriteLine("Device Name: " + displaySettings.DeviceName);
            Console.WriteLine("PelsWidth: " + displaySettings.PelsWidth);
            Console.WriteLine("PelsHeight: " + displaySettings.PelsHeight);
            Console.WriteLine("Position X: " + displaySettings.Position.x + ", Y: " + displaySettings.Position.y);
            Console.WriteLine("Orientation: " + displaySettings.Orientation);
            Console.WriteLine();
        }
    }
}
