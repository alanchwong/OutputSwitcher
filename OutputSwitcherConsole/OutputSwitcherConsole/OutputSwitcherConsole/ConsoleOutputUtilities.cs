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
        static private bool IsDisplayDeviceStateFlagSet(DisplayDeviceStateFlags flagField, DisplayDeviceStateFlags flagToCheckForEnabled)
        {
            return (flagField & flagToCheckForEnabled) > 0;
        }

        static private string GetIsDisplayDeviceStateFlagSetYesNoString(DisplayDeviceStateFlags flagField, DisplayDeviceStateFlags flagToCheckForEnabled)
        {
            if (IsDisplayDeviceStateFlagSet(flagField, flagToCheckForEnabled))
                return "Yes";
            else
                return "No";
        }

        static public void WriteDisplayDeviceToConsole(DISPLAY_DEVICE displayDevice, uint devNum)
        {
            Console.WriteLine("Device #: " + devNum);
            Console.WriteLine("Device Name: " + displayDevice.DeviceName);
            Console.WriteLine("Device String: " + displayDevice.DeviceString);
            Console.WriteLine("Device ID: " + displayDevice.DeviceID);
            Console.WriteLine("Attached to Desktop? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.AttachedToDesktop));
            Console.WriteLine("MultiDriver? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.MultiDriver));
            Console.WriteLine("Primary Device? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.PrimaryDevice));
            Console.WriteLine("Mirroring Driver? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.MirroringDriver));
            Console.WriteLine("VGA Compatible? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.VGACompatible));
            Console.WriteLine("Removable? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.Removable));
            Console.WriteLine("ModesPruned? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.ModesPruned));
            Console.WriteLine("Remote? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.Remote));
            Console.WriteLine("Disconnect? " + GetIsDisplayDeviceStateFlagSetYesNoString(displayDevice.StateFlags, DisplayDeviceStateFlags.Disconnect));
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
            Console.WriteLine("Display Frequency: " + devMode.dmDisplayFrequency);
            Console.WriteLine("Is Primary: " + (DisplaySettings.IsDisplayPrimary(devMode) ? "Yes" : "No"));
            Console.WriteLine();
        }

        static public void WriteDisplayDeviceSettingsToConsole(DisplayDeviceSettings displaySettings)
        {
            Console.WriteLine("Device Name: " + displaySettings.DeviceName);
            Console.WriteLine("Device ID: " + displaySettings.DeviceID);
            Console.WriteLine("PelsWidth: " + displaySettings.PelsWidth);
            Console.WriteLine("PelsHeight: " + displaySettings.PelsHeight);
            Console.WriteLine("Position X: " + displaySettings.Position.x + ", Y: " + displaySettings.Position.y);
            Console.WriteLine("Orientation: " + displaySettings.Orientation);
            Console.WriteLine("Display Frequency: " + displaySettings.DisplayFrequency);
            Console.WriteLine();
        }

        static public void WriteDisplayPresetToConsole(DisplayPreset displayPreset)
        {
            Console.WriteLine("Preset Name: " + displayPreset.Name);

            foreach (DisplayDeviceSettings displayDeviceSettings in displayPreset.DisplaySettings)
            {
                WriteDisplayDeviceSettingsToConsole(displayDeviceSettings);
            }
        }
    }
}
