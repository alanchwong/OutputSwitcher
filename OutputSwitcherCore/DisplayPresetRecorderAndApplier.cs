using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcher.WinAPI;

namespace OutputSwitcher.Core
{
    /// <summary>
    /// Class responsible for recording the current display configuration as a preset, and
    /// applying a saved configuration.
    /// </summary>
    public class DisplayPresetRecorderAndApplier
    {
        /// <summary>
        /// Records the current display configuration as a display preset.
        /// </summary>
        /// <param name="presetName">Name of the preset to create</param>
        /// <returns>A DisplayPreset with the current configuration and supplied name.
        ///             Throws exception if failed to get display configuration paths and modes.</returns>
        static public DisplayPreset RecordCurrentConfiguration(string presetName)
        {
            DisplayPreset displayPreset = null;

            const CCD.QueryDisplayFlags OnlyActivePathsFlag = CCD.QueryDisplayFlags.OnlyActivePaths;

            int numPathArrayElements;
            int numModeInfoArrayElements;

            // Get the buffer sizes needed to hold the active paths and the source/target mode table.
            Win32Utilities.ThrowIfResultCodeNotSuccess(
                CCD.GetDisplayConfigBufferSizes(
                    OnlyActivePathsFlag,
                    out numPathArrayElements,
                    out numModeInfoArrayElements));

            CCD.DisplayConfigPathInfo[] pathInfoArray = new CCD.DisplayConfigPathInfo[numPathArrayElements];
            CCD.DisplayConfigModeInfo[] modeInfoArray = new CCD.DisplayConfigModeInfo[numModeInfoArrayElements];

            // Get the active paths and their associated source/target modes.
            Win32Utilities.ThrowIfResultCodeNotSuccess(
                CCD.QueryDisplayConfig(
                    OnlyActivePathsFlag,
                    ref numPathArrayElements,
                    pathInfoArray,
                    ref numModeInfoArrayElements,
                    modeInfoArray,
                    IntPtr.Zero));

            displayPreset = new DisplayPreset(presetName);

            displayPreset.PathInfoArray = pathInfoArray;
            displayPreset.ModeInfoArray = modeInfoArray;

            // Save the Target Device Name structs to log the monitor output devices. This isn't
            // actually used for anything but makes the XML output more readable.
            CCD.DisplayConfigTargetDeviceName[] targetDeviceNameArray = new CCD.DisplayConfigTargetDeviceName[pathInfoArray.Length];

            for (int i = 0; i < pathInfoArray.Length; i++)
            {
                targetDeviceNameArray[i] = GetTargetDeviceName(pathInfoArray[i].targetInfo.adapterId, pathInfoArray[i].targetInfo.id);
            }

            displayPreset.TargetDeviceNames = targetDeviceNameArray;

            // Save the Adapter Name structs. The adapter ID values may change on reboot, as they
            // appear to simply be a logical, run-time value rather than a persistent identifier.
            // So we save the Adapter Name structs to log a device name that the adapter ID maps to. 
            // We can use this to update the adapter IDs we save in our presets such that the presets
            // will still be usable after a reboot. Otherwise, when the machine is rebooted and the
            // user tries to apply a saved preset, the API call will fail because the adapter ID has
            // changed.
            Dictionary<CCD.LUID, CCD.DisplayConfigAdapterName> adapterIdToAdapterName = new Dictionary<CCD.LUID, CCD.DisplayConfigAdapterName>();

            // Find all the unique adapter IDs used in the active paths and capture the display adapter
            // device names that those IDs map to.
            foreach (CCD.DisplayConfigPathInfo pathInfo in pathInfoArray)
            {
                if (!adapterIdToAdapterName.ContainsKey(pathInfo.sourceInfo.adapterId))
                {
                    CCD.DisplayConfigAdapterName adapterName = new CCD.DisplayConfigAdapterName();
                    adapterName.header.adapterId = pathInfo.sourceInfo.adapterId;
                    adapterName.header.size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(adapterName);
                    adapterName.header.type = CCD.DisplayConfigDeviceInfoType.GetAdapterName;

                    Win32Utilities.ThrowIfResultCodeNotSuccess(CCD.DisplayConfigGetDeviceInfo(ref adapterName));

                    adapterIdToAdapterName.Add(adapterName.header.adapterId, adapterName);
                }
            }

            displayPreset.AdapterNames = adapterIdToAdapterName.Values.ToArray();

            return displayPreset;
        }

        /// <summary>
        /// Applies the supplied display preset to the system, and dynamically changes it.
        /// Throws if the settings failed to be applied.
        /// </summary>
        /// <param name="displayPreset">The display preset to apply.</param>
        static public void ApplyPreset(DisplayPreset displayPreset)
        {
            AdapterIdMapper.DisplayPresetAdapterIdValidation validationResult =
                AdapterIdMapper.GetAdapterIdMapper().ValidateDisplayPresetAdapterIds(displayPreset);

            CCD.DisplayConfigPathInfo[] pathInfoArrayToApply = displayPreset.PathInfoArray;
            CCD.DisplayConfigModeInfo[] modeInfoArrayToApply = displayPreset.ModeInfoArray;

            if (validationResult == AdapterIdMapper.DisplayPresetAdapterIdValidation.NeedAdapterIdRemap)
            {
                pathInfoArrayToApply = displayPreset.PathInfoArray;
                modeInfoArrayToApply = displayPreset.ModeInfoArray;

                // TODO: The remap methods return a boolean, though it sorta doesn't matter for us since
                // we've already done validation. It's the right design that those methods return a
                // boolean, so maybe we should put an assert here?
                AdapterIdMapper.GetAdapterIdMapper().RemapDisplayConfigPathInfoAdapterIds(displayPreset, ref pathInfoArrayToApply);
                AdapterIdMapper.GetAdapterIdMapper().RemapDisplayConfigModeInfoAdapterIds(displayPreset, ref modeInfoArrayToApply);
            }
            else if (validationResult == AdapterIdMapper.DisplayPresetAdapterIdValidation.MissingAdapter)
            {
                // TODO: Have better interface for handling this case, cuz it's a real case where someone
                // upgrades a video card for example.
                throw new Exception("Missing adapter! Can't apply preset.");
            }
            else if (validationResult == AdapterIdMapper.DisplayPresetAdapterIdValidation.DisplayPresetMissingAdapterInformation)
            {
                // TODO: Handle this better, basically case where schema change
                throw new Exception("Display Preset is missing adapter name information.");
            }

            // Third validation result case is that all the adapter IDs are still valid and map correctly, so no action required.

            Win32Utilities.ThrowIfResultCodeNotSuccess(
                CCD.SetDisplayConfig(
                    (uint)pathInfoArrayToApply.Length,
                    pathInfoArrayToApply,
                    (uint)modeInfoArrayToApply.Length,
                    modeInfoArrayToApply,
                    CCD.SdcFlags.Apply | CCD.SdcFlags.UseSuppliedDisplayConfig | CCD.SdcFlags.AllowChanges | CCD.SdcFlags.SaveToDatabase));
        }
        
        /// <summary>
        /// Applies the supplied display preset to the system and dynamically switches to it. Returns the
        /// display configuration in use before the new preset is applied as a DisplayPreset. This allows
        /// consumers to switch back to that last configuration if needed. Throws exception if failed to
        /// capture current configuration, or if failed to apply supplied preset configuration.
        /// </summary>
        /// <param name="displayPreset">The display configuration to switch to.</param>
        /// <returns>The display configuration in use before the supplied preset is applied. The name of
        /// the preset will be a GUID.</returns>
        static public DisplayPreset ReturnLastConfigAndApplyPreset(DisplayPreset displayPreset)
        {
            Guid guid = Guid.NewGuid();

            DisplayPreset currentConfig = RecordCurrentConfiguration(guid.ToString());

            ApplyPreset(displayPreset);

            return currentConfig;
        }

        /// <summary>
        /// Gets the display target name.
        /// </summary>
        /// <param name="targetAdapterId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        static private CCD.DisplayConfigTargetDeviceName GetTargetDeviceName(CCD.LUID targetAdapterId, uint targetId)
        {
            CCD.DisplayConfigTargetDeviceName targetDeviceName = new CCD.DisplayConfigTargetDeviceName();

            targetDeviceName.header.type = CCD.DisplayConfigDeviceInfoType.GetTargetName;
            targetDeviceName.header.size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(targetDeviceName);
            targetDeviceName.header.adapterId = targetAdapterId;
            targetDeviceName.header.id = targetId;

            Win32Utilities.ThrowIfResultCodeNotSuccess(CCD.DisplayConfigGetDeviceInfo(ref targetDeviceName));

            return targetDeviceName;
        }
    }
}
