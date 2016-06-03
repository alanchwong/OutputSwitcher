using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcherConsole.Data;
using OutputSwitcherConsole.WinAPI;

namespace OutputSwitcherConsole
{
    class ConsoleCommands
    {
        /// <summary>
        /// Enumerates all display adapters and attached display devices for each adapter and outputs 
        /// to console.
        /// </summary>
        static public void ShowDisplayDevicesToConsole()
        {
            DISPLAY_DEVICE adapterDisplayDevice = new DISPLAY_DEVICE();
            adapterDisplayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(adapterDisplayDevice);

            uint adapterDevNum = 0;
            string displayAdapterName;

            while (DisplaySettings.EnumDisplayDevices(null, adapterDevNum, ref adapterDisplayDevice, 0))
            {
                displayAdapterName = adapterDisplayDevice.DeviceName;

                ConsoleOutputUtilities.WriteDisplayDeviceToConsole(adapterDisplayDevice, adapterDevNum);

                uint devNum = 0;

                DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
                displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);

                while (DisplaySettings.EnumDisplayDevices(displayAdapterName, devNum, ref displayDevice, 0))
                {
                    ConsoleOutputUtilities.WriteDisplayDeviceToConsole(displayDevice, devNum);
                    devNum++;
                }

                adapterDevNum++;
            }
        }

        /// <summary>
        /// Enumerates settings for all attached display adapters and outputs to console.
        /// </summary>
        static public void ShowDisplayAdapterDeviceSettings()
        {
            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);

            uint devNum = 0;

            while (DisplaySettings.EnumDisplayDevices(null, devNum, ref displayDevice, 0))
            {
                string displayAdapterName = displayDevice.DeviceName;

                DEVMODE devMode = new DEVMODE();
                devMode.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(devMode);
                devMode.dmDriverExtra = 0;

                DisplaySettings.EnumDisplaySettingsEx(
                    displayAdapterName,
                    DisplaySettings.ENUM_CURRENT_SETTINGS,
                    ref devMode,
                    0);

                ConsoleOutputUtilities.WriteDisplayDeviceSettingsToConsole(devMode, displayAdapterName);

                devNum++;
            }
        }

        static public void TestSwapPrimaryAndASecondaryDisplay()
        {
            bool primaryFound = false;
            bool secondaryFound = false;

            DisplayDeviceSettings originalPrimaryDeviceSettings = null;
            DisplayDeviceSettings originalSecondaryDeviceSettings = null;

            DEVMODE originalPrimarySettings = new DEVMODE();
            DEVMODE originalSecondarySettings = new DEVMODE();

            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);
           
            uint devNum = 0;

            // Loop until we find a primary and a secondary or until we run out of devices.
            while (!(primaryFound && secondaryFound) && DisplaySettings.EnumDisplayDevices(null, devNum, ref displayDevice, 0))
            {
                DEVMODE devMode = new DEVMODE();
                devMode.dmDriverExtra = 0;
                devMode.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(devMode);

                if (DisplaySettings.EnumDisplaySettingsEx(
                    displayDevice.DeviceName,
                    DisplaySettings.ENUM_CURRENT_SETTINGS,
                    ref devMode,
                    0))
                {
                    if (DisplaySettings.IsDisplayPrimary(devMode))
                    {
                        primaryFound = true;
                        originalPrimaryDeviceSettings = new DisplayDeviceSettings(displayDevice.DeviceName, devMode);
                        originalPrimarySettings = devMode;
                    }
                    else if (!secondaryFound && DisplaySettings.IsDisplayAttachedToDesktop(devMode))
                    {
                        secondaryFound = true;
                        originalSecondaryDeviceSettings = new DisplayDeviceSettings(displayDevice.DeviceName, devMode);
                        originalSecondarySettings = devMode;
                    }
                }

                devNum++;
            }

            if (!(primaryFound && secondaryFound))
            {
                Console.WriteLine("Failed to detect both primary and secondary display");
                return;
            }

            Console.WriteLine("Found primary device:");
            ConsoleOutputUtilities.WriteDisplayDeviceSettingsToConsole(originalPrimaryDeviceSettings);
            Console.WriteLine("Found secondary device:");
            ConsoleOutputUtilities.WriteDisplayDeviceSettingsToConsole(originalSecondaryDeviceSettings);

            // Create new settings
            DEVMODE newPrimarySettings = new DEVMODE();
            DEVMODE newSecondarySettings = new DEVMODE();

            newPrimarySettings.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(newPrimarySettings);
            newPrimarySettings.dmDriverExtra = 0;
            newPrimarySettings.dmPosition.x = 0;
            newPrimarySettings.dmPosition.y = 0;
            newPrimarySettings.dmPelsHeight = originalSecondaryDeviceSettings.PelsHeight;
            newPrimarySettings.dmPelsWidth = originalSecondaryDeviceSettings.PelsWidth;
            newPrimarySettings.dmDisplayOrientation = originalSecondaryDeviceSettings.Orientation;
            newPrimarySettings.dmFields = DM.Position | DM.PelsWidth | DM.PelsHeight | DM.DisplayOrientation;

            newSecondarySettings.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(newSecondarySettings);
            newSecondarySettings.dmDriverExtra = 0;
            newSecondarySettings.dmPosition.x = newPrimarySettings.dmPelsWidth; // Place it immediately to the right of new primary
            newSecondarySettings.dmPosition.y = 0;  // Place it at the same height as new primary
            newSecondarySettings.dmPelsHeight = originalPrimaryDeviceSettings.PelsHeight;
            newSecondarySettings.dmPelsWidth = originalPrimaryDeviceSettings.PelsWidth;
            newSecondarySettings.dmDisplayOrientation = originalPrimaryDeviceSettings.Orientation;
            newSecondarySettings.dmFields = DM.Position | DM.PelsWidth | DM.PelsHeight | DM.DisplayOrientation;

            // Output new settings
            Console.WriteLine("New primary device settings:");
            ConsoleOutputUtilities.WriteDisplayDeviceSettingsToConsole(newPrimarySettings, originalSecondaryDeviceSettings.DeviceName);
            Console.WriteLine("New secondary device settings:");
            ConsoleOutputUtilities.WriteDisplayDeviceSettingsToConsole(newSecondarySettings, originalPrimaryDeviceSettings.DeviceName);

            // Wait for user input before swapping...
            Console.WriteLine("Press any key to commence swap...");
            Console.ReadKey();

            DISP_CHANGE result = DisplaySettings.ChangeDisplaySettingsEx(
                originalSecondaryDeviceSettings.DeviceName,
                ref newPrimarySettings,
                IntPtr.Zero,
                ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET,
                IntPtr.Zero);

            if (result != DISP_CHANGE.Successful)
            {
                Console.WriteLine("Failed to change secondary to primary with value: " + result);
                return;
            }

            result = DisplaySettings.ChangeDisplaySettingsEx(
                originalPrimaryDeviceSettings.DeviceName,
                ref newSecondarySettings,
                IntPtr.Zero,
                ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET,
                IntPtr.Zero);

            if (result != DISP_CHANGE.Successful)
            {
                Console.WriteLine("Failed to change secondary to primary with value: " + result);
                return;
            }

            result = DisplaySettings.ChangeDisplaySettingsEx(
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                IntPtr.Zero);

            if (result != DISP_CHANGE.Successful)
            {
                Console.WriteLine("Failed to apply primary/secondary swap with value: " + result);
            }

            // Wait for user input before swapping back to original!
            Console.WriteLine("Press any key to swap back to original configuration.");
            Console.ReadKey();

            // Set the dmFields flag to indicate which parts of the original DEVMODE struct to be used
            // by ChangeDisplaySettingsEx.
            originalPrimarySettings.dmFields = DM.Position | DM.PelsWidth | DM.PelsHeight | DM.DisplayOrientation;
            originalSecondarySettings.dmFields = DM.Position | DM.PelsWidth | DM.PelsHeight | DM.DisplayOrientation;

            result = DisplaySettings.ChangeDisplaySettingsEx(
                originalPrimaryDeviceSettings.DeviceName,
                ref originalPrimarySettings,
                IntPtr.Zero,
                ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_NORESET,
                IntPtr.Zero);

            if (result != DISP_CHANGE.Successful)
            {
                Console.WriteLine("Failed to restore original primary with value: " + result);
                return;
            }

            result = DisplaySettings.ChangeDisplaySettingsEx(
                originalSecondaryDeviceSettings.DeviceName,
                ref originalSecondarySettings,
                IntPtr.Zero,
                ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET,
                IntPtr.Zero);

            if (result != DISP_CHANGE.Successful)
            {
                Console.WriteLine("Failed to restore original secondary with value: " + result);
                return;
            }

            result = DisplaySettings.ChangeDisplaySettingsEx(
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                IntPtr.Zero);

            if (result != DISP_CHANGE.Successful)
            {
                Console.WriteLine("Failed to apply primary/secondary swap with value: " + result);
            }
        }
    }
}
