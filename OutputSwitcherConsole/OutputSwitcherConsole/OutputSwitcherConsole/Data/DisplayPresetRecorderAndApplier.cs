using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcherConsole.WinAPI;

namespace OutputSwitcherConsole.Data
{
    /// <summary>
    /// Class responsible for recording the current display configuration as a preset, and
    /// applying a saved configuration.
    /// </summary>
    class DisplayPresetRecorderAndApplier
    {
        /// <summary>
        /// Returns a fully populated DisplayDeviceSettings containing the settings for the display device specified
        /// by the supplied DISPLAY_DEVICE, as well as the DeviceID of the first display device attached to it.
        /// (e.g. the settings of \\.\DISPLAY1 and the device ID of \\.\DISPLAY1\Monitor0)
        /// It is expected that the supplied DISPLAY_DEVICE is a top-level logical display.
        /// </summary>
        /// <param name="displayDevice">The display device to get settings for.</param>
        /// <returns>A DisplayDeviceSettings object populated with the device settings of the supplied display device.</returns>
        static public DisplayDeviceSettings GetDisplayDeviceSettingsForDisplayDevice(ref DISPLAY_DEVICE displayDevice)
        {
            DEVMODE logicalDisplayAdapterDevMode = DisplaySettings.MakeNewDevmode();

            // This returns false if the adapter is disconnected in current settings, so
            // we don't check for display value.
            DisplaySettings.EnumDisplaySettingsEx(
                displayDevice.DeviceName,
                DisplaySettings.ENUM_CURRENT_SETTINGS,
                ref logicalDisplayAdapterDevMode,
                0);

            DISPLAY_DEVICE monitorDisplayDevice = DisplaySettings.MakeNewDisplayDevice();

            // Get the first display device, which will be a monitor, that is attached to the logical display
            // adapter. There won't be any meaningful settings here, but we need the DeviceID because we
            // care about settings as they relate to monitors, which can be attached to different logical
            // display adapters (e.g. the Dell 27" is attached to \\.\DISPLAY1 this time, and \\.\DISPLAY2 after
            // reboot) during different sessions.
            // TODO: What do we do for disconnected case?
            uint monitorDevNum = 0;

            string deviceID = String.Empty;

            while (DisplaySettings.EnumDisplayDevices(
                    displayDevice.DeviceName,
                    monitorDevNum,
                    ref monitorDisplayDevice,
                    0))
            {
                monitorDevNum++;

                if ((monitorDisplayDevice.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) > 0)
                { 
                   deviceID = monitorDisplayDevice.DeviceID;
                   break;
                }
            };

            DisplayDeviceSettings displayDeviceSettings = new DisplayDeviceSettings(
                displayDevice.DeviceName,
                deviceID,
                logicalDisplayAdapterDevMode);

            return displayDeviceSettings;
        }

        /// <summary>
        /// Records the current display configuration as a display preset.
        /// </summary>
        /// <param name="presetName">Name of the preset to create</param>
        /// <returns>A DisplayPreset with the current configuration and supplied name.
        ///             Throws exception if failed to get settings for a single device.</returns>
        static public DisplayPreset RecordCurrentConfiguration(string presetName)
        {
            DisplayPreset displayPreset = null;

            List<DisplayDeviceSettings> listOfDeviceSettings = new List<DisplayDeviceSettings>();

            DISPLAY_DEVICE displayDevice = DisplaySettings.MakeNewDisplayDevice();

            uint devNum = 0;

            // Grab all the settings for adapters. Ignore the monitors for now, assume one mon per adapter.
            while (DisplaySettings.EnumDisplayDevices(null, devNum, ref displayDevice, 0))
            {
                devNum++;

                DisplayDeviceSettings displayDeviceSettings = GetDisplayDeviceSettingsForDisplayDevice(ref displayDevice);
                listOfDeviceSettings.Add(displayDeviceSettings);
            }

            if (listOfDeviceSettings.Count > 0)
            {
                displayPreset = new DisplayPreset(presetName);
                displayPreset.DisplaySettings = listOfDeviceSettings;
            }

            return displayPreset;
        }

        /// <summary>
        /// Applies the supplied display preset to the system, and dynamically changes it.
        /// Throws if a device's settings failed to be changed, or if dynamically changing the
        /// displays to the new settings failed.
        /// </summary>
        /// <param name="displayPreset">The display preset to apply.</param>
        static public void ApplyPreset(DisplayPreset displayPreset)
        {
            // TODO: Verify that configuration is still valid in as much as that we have
            // the same number of display adapters available.

            List<DisplayDeviceSettings> displaySettingsList = displayPreset.DisplaySettings;

            foreach (DisplayDeviceSettings displaySettings in displaySettingsList)
            {
                DEVMODE devMode = displaySettings.ToDevModeForChangeDisplaySettings();

                ChangeDisplaySettingsFlags settingsFlags =
                    ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET;

                if (DisplaySettings.IsDisplayPrimary(devMode))
                {
                    settingsFlags = settingsFlags | ChangeDisplaySettingsFlags.CDS_SET_PRIMARY;
                }

                DISP_CHANGE changeDeviceSettingResult = DisplaySettings.ChangeDisplaySettingsEx(
                    displaySettings.DeviceName,
                    ref devMode,
                    IntPtr.Zero,
                    settingsFlags,
                    IntPtr.Zero);
                
                if (changeDeviceSettingResult != DISP_CHANGE.Successful)
                {
                    // If we fail to apply settings for this device, it's possible the
                    // device is disconnected. So we can check to see if it's disconnected
                    // by attempting to retrieve settings from it.
                    if (changeDeviceSettingResult == DISP_CHANGE.Failed)
                    {
                        DEVMODE retrievedSettingsDevMode = new DEVMODE();
                        retrievedSettingsDevMode.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(retrievedSettingsDevMode);
                        retrievedSettingsDevMode.dmDriverExtra = 0;

                        bool retrieveSettingsOnFailedDeviceResult =
                            DisplaySettings.EnumDisplaySettingsEx(
                                displaySettings.DeviceName,
                                DisplaySettings.ENUM_CURRENT_SETTINGS,
                                ref retrievedSettingsDevMode,
                                0);

                        if (retrieveSettingsOnFailedDeviceResult)
                        {
                            // We've successfully retrieved settings for the device, which means it isn't disconnected.
                            throw new Exception("Failed to update settings for verified attached device: " + displaySettings.DeviceName + ". Failure code: " + changeDeviceSettingResult);
                        }
                        // ELSE the device is disconnected and we should do nothing and continue.
                    }
                    else
                    {
                        throw new Exception("Failed to update setting for device: " + displaySettings.DeviceName + ". Failure code: " + changeDeviceSettingResult);
                    }
                }
            }

            // Apply all the changes.
            DISP_CHANGE applySettingsResult = DisplaySettings.ChangeDisplaySettingsEx(
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                IntPtr.Zero);
            
            if (applySettingsResult != DISP_CHANGE.Successful)
            {
                throw new Exception("Successfully updated all settings, failed to change settings dynamically. Failure code: " + applySettingsResult);
            }

            applySettingsResult = DisplaySettings.ChangeDisplaySettings(
                IntPtr.Zero,
                0);

            if (applySettingsResult != DISP_CHANGE.Successful)
            {
                throw new Exception("Successfully updated all settings, failed to change settings dynamically. Failure code: " + applySettingsResult);
            }

            // TODO: Should we be able to UNDO if we've messed up somewhere here? Probably.
        }
    }
}
