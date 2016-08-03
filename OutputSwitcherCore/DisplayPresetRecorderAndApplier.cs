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

            CCD.DisplayConfigTargetDeviceName[] targetDeviceNameArray = new CCD.DisplayConfigTargetDeviceName[pathInfoArray.Length];

            for (int i = 0; i < pathInfoArray.Length; i++)
            {
                targetDeviceNameArray[i] = GetTargetDeviceName(pathInfoArray[i].targetInfo.adapterId, pathInfoArray[i].targetInfo.id);
            }

            displayPreset.TargetDeviceNames = targetDeviceNameArray;

            return displayPreset;
        }

        /// <summary>
        /// Applies the supplied display preset to the system, and dynamically changes it.
        /// Throws if the settings failed to be applied.
        /// </summary>
        /// <param name="displayPreset">The display preset to apply.</param>
        static public void ApplyPreset(DisplayPreset displayPreset)
        {
            Win32Utilities.ThrowIfResultCodeNotSuccess(
                CCD.SetDisplayConfig(
                    (uint)displayPreset.PathInfoArray.Length,
                    displayPreset.PathInfoArray,
                    (uint)displayPreset.ModeInfoArray.Length,
                    displayPreset.ModeInfoArray,
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
