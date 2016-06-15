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
        /// Terminates the application with a return code of 0.
        /// </summary>
        static public void Exit()
        {
            DisplayPresetCollection.GetDisplayPresetCollection().PersistDisplayPresets();   // Write to disk before exiting.

            Environment.Exit(0);
        }

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

                uint innerDevNum = 0;

                DISPLAY_DEVICE innerDisplayDevice = DisplaySettings.MakeNewDisplayDevice();

                while (DisplaySettings.EnumDisplayDevices(displayAdapterName, innerDevNum, ref innerDisplayDevice, 0))
                {
                    DEVMODE innerDevMode = DisplaySettings.MakeNewDevmode();

                    DisplaySettings.EnumDisplaySettingsEx(
                        innerDisplayDevice.DeviceName,
                        DisplaySettings.ENUM_CURRENT_SETTINGS,
                        ref innerDevMode,
                        0);

                    ConsoleOutputUtilities.WriteDisplayDeviceSettingsToConsole(innerDevMode, innerDisplayDevice.DeviceName);

                    innerDevNum++;
                }

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
                    DISPLAY_DEVICE innerDisplayDevice = DisplaySettings.MakeNewDisplayDevice();

                    DisplaySettings.EnumDisplayDevices(
                        displayDevice.DeviceName,
                        0,
                        ref innerDisplayDevice,
                        0);

                    if (DisplaySettings.IsDisplayPrimary(devMode))
                    {
                        primaryFound = true;
                        originalPrimaryDeviceSettings = new DisplayDeviceSettings(displayDevice.DeviceName, innerDisplayDevice.DeviceID, devMode);
                        originalPrimarySettings = devMode;
                    }
                    else if (!secondaryFound && DisplaySettings.IsDisplayAttachedToDesktop(devMode))
                    {
                        secondaryFound = true;
                        originalSecondaryDeviceSettings = new DisplayDeviceSettings(displayDevice.DeviceName, innerDisplayDevice.DeviceID, devMode);
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

            result = DisplaySettings.ChangeDisplaySettingsEx(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero);

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

            result = DisplaySettings.ChangeDisplaySettingsEx(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero);

            if (result != DISP_CHANGE.Successful)
            {
                Console.WriteLine("Failed to apply primary/secondary swap with value: " + result);
            }
        }

        static public void TestCaptureCurrentConfigurationAndWriteToFile()
        {
            DisplayPresetCollection presetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            DisplayPreset currentConfigurationAsPreset = 
                DisplayPresetRecorderAndApplier.RecordCurrentConfiguration("Alpha One");

            presetCollection.TryAddDisplayPreset(currentConfigurationAsPreset);
            presetCollection.PersistDisplayPresets();
        }

        static public void ListPresets()
        {
            DisplayPresetCollection presetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            foreach (DisplayPreset displayPreset in presetCollection.GetPresets())
            {
                ConsoleOutputUtilities.WriteDisplayPresetToConsole(displayPreset);
            }
        }

        static public void DeletePreset()
        {
            Console.Write("Enter name of preset to delete: ");
            string targetPresetName = Console.ReadLine();

            DisplayPresetCollection displayPresetCollection = DisplayPresetCollection.GetDisplayPresetCollection();
            bool presetDeleted = displayPresetCollection.TryRemoveDisplayPreset(targetPresetName);

            if (presetDeleted)
            {
                Console.WriteLine("Deleted preset '" + targetPresetName + "'.");
            }
            else
            {
                Console.WriteLine("Preset '" + targetPresetName + "' does not exist.");
            }
        }

        static public void CaptureCurrentConfigAndSaveAsPreset()
        {
            bool uniqueNameEntered = false;

            string presetName;

            DisplayPresetCollection displayPresetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            do
            {
                Console.Write("Enter name for new preset: ");
                presetName = Console.ReadLine();

                DisplayPreset existingDisplayPreset = displayPresetCollection.GetPreset(presetName);

                uniqueNameEntered = existingDisplayPreset == null;

                if (!uniqueNameEntered)
                {
                    Console.WriteLine("Preset with name '" + presetName + "' already exists. Please choose a different name.");
                }

            } while (!uniqueNameEntered);

            DisplayPreset newPreset = DisplayPresetRecorderAndApplier.RecordCurrentConfiguration(presetName);

            if (!displayPresetCollection.TryAddDisplayPreset(newPreset))
            {
                throw new Exception("Failed to add new preset to saved presets collection.");   // This is really unexpected because we've checked for existence already.
            }

            Console.WriteLine("Added new preset!");
            ConsoleOutputUtilities.WriteDisplayPresetToConsole(newPreset);
        }

        static public void ApplyPreset()
        {
            Console.Write("Enter name of preset to apply: ");
            string presetName = Console.ReadLine();

            DisplayPresetCollection displayPresetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            DisplayPreset targetPreset = displayPresetCollection.GetPreset(presetName);

            if (targetPreset == null)
            {
                Console.WriteLine("Preset with name '" + presetName + "' does not exist.");
                return;
            }

            Console.WriteLine("Applying preset '" + presetName + "'...");
            DisplayPresetRecorderAndApplier.ApplyPreset(targetPreset);
        }

        static public void TestAttachTV()
        {
            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);

            DisplaySettings.EnumDisplayDevices(
                null,
                2,  // TV is the third device
                ref displayDevice,
                0);

            DEVMODE devmode = DisplaySettings.MakeNewDevmode();
            devmode.dmDeviceName = displayDevice.DeviceName;
            devmode.dmPelsHeight = 1080;
            devmode.dmPelsWidth = 1920;
            devmode.dmDisplayOrientation = 0;
            devmode.dmDisplayFrequency = 60;
            devmode.dmPosition = new POINTL();
            devmode.dmPosition.x = -1050;
            devmode.dmPosition.y = -128;
            devmode.dmFields = DM.PelsHeight | DM.PelsWidth | DM.DisplayOrientation | DM.DisplayFrequency | DM.Position;

            ConsoleOutputUtilities.WriteDisplayDeviceSettingsToConsole(devmode, devmode.dmDeviceName);

            DISP_CHANGE result = DisplaySettings.ChangeDisplaySettingsEx(
                devmode.dmDeviceName,
                ref devmode,
                IntPtr.Zero,
                ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET,
                IntPtr.Zero);

            Console.WriteLine("Result code: " + result);

            result = DisplaySettings.ChangeDisplaySettingsEx(
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                IntPtr.Zero);

            Console.WriteLine("Result code: " + result);
        }
    }
}
