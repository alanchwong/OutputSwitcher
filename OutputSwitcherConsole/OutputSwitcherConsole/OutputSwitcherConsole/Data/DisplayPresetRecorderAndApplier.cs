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
        /// Records the current display configuration as a display preset.
        /// </summary>
        /// <param name="presetName">Name of the preset to create</param>
        /// <returns>A DisplayPreset with the current configuration and supplied name.
        ///             Throws exception if failed to get settings for a single device.</returns>
        static public DisplayPreset RecordCurrentConfiguration(string presetName)
        {
            DisplayPreset displayPreset = null;

            List<DisplayDeviceSettings> listOfDeviceSettings = new List<DisplayDeviceSettings>();

            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);

            uint devNum = 0;

            // Grab all the settings for adapters. Ignore the monitors for now, assume one mon per adapter.
            while (DisplaySettings.EnumDisplayDevices(null, devNum, ref displayDevice, 0))
            {
                devNum++;

                DEVMODE devMode = new DEVMODE();
                devMode.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(devMode);
                devMode.dmDriverExtra = 0;

                // This also returns false if the adapter is disconnected in current settings, so
                // we don't check for display value.
                DisplaySettings.EnumDisplaySettingsEx(
                    displayDevice.DeviceName,
                    DisplaySettings.ENUM_CURRENT_SETTINGS,
                    ref devMode,
                    0);

                DisplayDeviceSettings displayDeviceSettings = new DisplayDeviceSettings(
                        displayDevice.DeviceName,
                        devMode);

                listOfDeviceSettings.Add(displayDeviceSettings);
            }

            if (listOfDeviceSettings.Count > 0)
            {
                displayPreset = new DisplayPreset(presetName);
                displayPreset.DisplayDeviceSettings = listOfDeviceSettings;
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

            IList<DisplayDeviceSettings> displaySettingsList = displayPreset.DisplayDeviceSettings;

            foreach (DisplayDeviceSettings displaySettings in displaySettingsList)
            {
                DEVMODE devMode = displaySettings.ToDevModeForChangeDisplaySettings();

                ChangeDisplaySettingsFlags settingsFlags =
                    ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET;

                if (DisplaySettings.IsDisplayPrimary(devMode))
                {
                    settingsFlags = settingsFlags | ChangeDisplaySettingsFlags.CDS_SET_PRIMARY;
                }

                if (DisplaySettings.ChangeDisplaySettingsEx(
                    displaySettings.DeviceName,
                    ref devMode,
                    IntPtr.Zero,
                    settingsFlags,
                    IntPtr.Zero) != DISP_CHANGE.Successful)
                {
                    throw new Exception("Failed to update setting for device: " + displaySettings.DeviceName);
                }
            }

            // Apply all the changes.
            if (DisplaySettings.ChangeDisplaySettingsEx(
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                IntPtr.Zero) != DISP_CHANGE.Successful)
            {
                throw new Exception("Successfully updated all settings, failed to change settings dynamically.");
            }

            // TODO: Should we be able to UNDO if we've messed up somewhere here? Probably.
        }
    }
}
