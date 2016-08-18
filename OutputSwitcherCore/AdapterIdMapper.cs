using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using OutputSwitcher.WinAPI;

namespace OutputSwitcher.Core
{
    public class AdapterIdMapper
    {
        /// <summary>
        /// Specifies the validation result when checking a DisplayPreset's saved
        /// Adapter IDs to the Adapter IDs currently in use by Windows.
        /// </summary>
        public enum DisplayPresetAdapterIdValidation
        {
            /// <summary>
            /// DisplayPreset's adapter ID -> adapter name mapping matches
            /// what is currently in use by Windows. The DisplayPreset's data
            /// can be used with the CCD API to change the display configuration
            /// with no modification.
            /// </summary>
            AllValid,

            /// <summary>
            /// All of the adapters used in the DisplayPreset are accounted for,
            /// but the runtime Adapter IDs currently in use by Windows are different
            /// for those adapters.
            /// </summary>
            NeedAdapterIdRemap,

            /// <summary>
            /// One or more of the display adapters used by the DisplayPreset
            /// is no longer available in Windows (i.e. hardware change), rendering
            /// the DisplayPreset unusable.
            /// </summary>
            MissingAdapter,

            /// <summary>
            /// DisplayPreset does not have valid adapter name information saved that
            /// maps its adapter IDs to device names.
            /// </summary>
            DisplayPresetMissingAdapterInformation,
        }

        /// <summary>
        /// Retrieves the singleton instance of AdapterIdMapper.
        /// </summary>
        /// <returns>The singleton instance of AdapterIdMapper.</returns>
        public static AdapterIdMapper GetAdapterIdMapper()
        {
            if (mInstance == null)
                mInstance = new AdapterIdMapper();

            return mInstance;
        }

        /// <summary>
        /// Retrieves the Adapter ID currently in use by Windows to identify the supplied display
        /// adapter device path name.
        /// </summary>
        /// <param name="displayAdapterDevicePath">The display adapter device path name, as returned from 
        ///     from a DisplayConfigGetDeviceInfo API call with a DisplayConfigAdapterName struct.</param>
        /// <param name="adapterId">The current LUID adapter ID identifying that device.</param>
        /// <returns>True if the supplied adapter device name is available and functional (and thus has
        ///     an adapter ID). False otherwise.</returns>
        public bool GetCurrentAdapterIdForDevice(string displayAdapterDevicePath, out CCD.LUID adapterId)
        {
            return mCurrentRuntimeDeviceAdapterNamesToAdapterIds.TryGetValue(displayAdapterDevicePath, out adapterId);
        }

        /// <summary>
        /// Checks the adapter IDs in use by a DisplayPreset and validates whether those IDs need to be
        /// remapped to new values before being applied, and whether all the display adapters the preset
        /// uses are still installed and available on the system.
        /// </summary>
        /// <param name="displayPreset">The preset to validate.</param>
        /// <returns>A DisplayPresetAdapterIdValidation value indicating the validation result.</returns>
        public DisplayPresetAdapterIdValidation ValidateDisplayPresetAdapterIds(DisplayPreset displayPreset)
        {
            bool allAdapterNamesAccountedFor = true;
            bool allAdapterIdsValid = true;

            if (displayPreset.AdapterNames != null)
            {
                foreach (CCD.DisplayConfigAdapterName adapterName in displayPreset.AdapterNames)
                {
                    if (!mCurrentRuntimeDeviceAdapterNamesToAdapterIds.ContainsKey(adapterName.adapterDevicePath))
                    {
                        allAdapterNamesAccountedFor = false;
                        break;
                    }

                    if (!mCurrentRuntimeDeviceAdapterNamesToAdapterIds[adapterName.adapterDevicePath].Equals(adapterName.header.adapterId))
                    {
                        allAdapterIdsValid = false;
                    }
                }
            }

            if (displayPreset.AdapterNames == null)
            {
                return DisplayPresetAdapterIdValidation.DisplayPresetMissingAdapterInformation;
            }
            else if (!allAdapterNamesAccountedFor)
            {
                return DisplayPresetAdapterIdValidation.MissingAdapter;
            }
            else if (!allAdapterIdsValid)
            {
                return DisplayPresetAdapterIdValidation.NeedAdapterIdRemap;
            }
            else
            {
                return DisplayPresetAdapterIdValidation.AllValid;
            }
        }

        /// <summary>
        /// Remaps an array of DisplayConfigPathInfo source and target entries' adapter IDs to use the
        /// new adapter IDs 
        /// </summary>
        /// <param name="displayPreset">The DisplayPreset containing old adapter ID information to remap.</param>
        /// <param name="pathInfoArray">The array of paths to modify with new adapter IDs.</param>
        /// <returns></returns>
        public bool RemapDisplayConfigPathInfoAdapterIds(DisplayPreset displayPreset, ref CCD.DisplayConfigPathInfo[] pathInfoArray)
        {
            bool allAdapterIdsRemappable = true;

            Dictionary<CCD.LUID, string> displayPresetAdapterIdToAdapterNameMap = new Dictionary<CCD.LUID, string>();

            foreach (CCD.DisplayConfigAdapterName adapterName in displayPreset.AdapterNames)
            {
                displayPresetAdapterIdToAdapterNameMap.Add(adapterName.header.adapterId, adapterName.adapterDevicePath);
            }

            for (int i = 0; i < pathInfoArray.Length; i++)
            {
                CCD.LUID currentAdapterId;

                if (GetCurrentAdapterIdForDevice(displayPresetAdapterIdToAdapterNameMap[pathInfoArray[i].sourceInfo.adapterId], out currentAdapterId))
                {
                    pathInfoArray[i].sourceInfo.adapterId = currentAdapterId;
                }
                else
                {
                    allAdapterIdsRemappable = false;
                    break;
                }

                if (GetCurrentAdapterIdForDevice(displayPresetAdapterIdToAdapterNameMap[pathInfoArray[i].targetInfo.adapterId], out currentAdapterId))
                {
                    pathInfoArray[i].targetInfo.adapterId = currentAdapterId;
                }
                else
                {
                    allAdapterIdsRemappable = false;
                    break;
                }
            }

            return allAdapterIdsRemappable;
        }

        /// <summary>
        /// Remaps an array of DisplayConfigModeInfo entries' adapter IDs from a previous session to
        /// the current Windows session's adapter IDs for a given display adapter.
        /// </summary>
        /// <param name="displayPreset">The Display preset containing old adapter ID information to remap</param>
        /// <param name="modeInfoArray">The array of mode info entries to modify with new adapter IDs.</param>
        /// <returns></returns>
        public bool RemapDisplayConfigModeInfoAdapterIds(DisplayPreset displayPreset, ref CCD.DisplayConfigModeInfo[] modeInfoArray)
        {
            bool allAdapterIdsRemappable = true;

            Dictionary<CCD.LUID, string> displayPresetAdapterIdToAdapterNameMap = new Dictionary<CCD.LUID, string>();

            foreach (CCD.DisplayConfigAdapterName adapterName in displayPreset.AdapterNames)
            {
                displayPresetAdapterIdToAdapterNameMap.Add(adapterName.header.adapterId, adapterName.adapterDevicePath);
            }

            for (int i = 0; i < modeInfoArray.Length; i++)
            {
                CCD.LUID currentAdapterId;

                if (GetCurrentAdapterIdForDevice(displayPresetAdapterIdToAdapterNameMap[modeInfoArray[i].adapterId], out currentAdapterId))
                {
                    modeInfoArray[i].adapterId = currentAdapterId;
                }
                else
                {
                    allAdapterIdsRemappable = false;
                    break;
                }
            }

            return allAdapterIdsRemappable;
        }

        private AdapterIdMapper()
        {
            InitializeRuntimeDeviceAdapterNameToAdapterIdMapping();
        }

        private void InitializeRuntimeDeviceAdapterNameToAdapterIdMapping()
        {
            mCurrentRuntimeDeviceAdapterNamesToAdapterIds = new Dictionary<string, CCD.LUID>();

            // Initialize the mapping of device names to adapter IDs.

            // Start with querying all display paths.
            int numPathArrayElements;
            int numModeInfoArrayElements;

            Win32Utilities.ThrowIfResultCodeNotSuccess(
                CCD.GetDisplayConfigBufferSizes(
                    CCD.QueryDisplayFlags.AllPaths,
                    out numPathArrayElements,
                    out numModeInfoArrayElements));

            CCD.DisplayConfigPathInfo[] pathInfoArray = new CCD.DisplayConfigPathInfo[numPathArrayElements];
            CCD.DisplayConfigModeInfo[] modeInfoArray = new CCD.DisplayConfigModeInfo[numModeInfoArrayElements];

            Win32Utilities.ThrowIfResultCodeNotSuccess(
                CCD.QueryDisplayConfig(
                    CCD.QueryDisplayFlags.AllPaths,
                    ref numPathArrayElements,
                    pathInfoArray,
                    ref numModeInfoArrayElements,
                    modeInfoArray,
                    IntPtr.Zero));

            Dictionary<CCD.LUID, string> adapterIdToAdapterName = new Dictionary<CCD.LUID, string>();

            // Find the mapping for each unique LUID -> device adapter name.
            // Don't use a foreach here because there are some cases where QueryDisplayConfig changes
            // the length of pathInfoArray to have more elements than numPathArrayElements would indicate,
            // with the extra elements invalidly initialized to 0s in all of its members.
            for (int i = 0; i < numPathArrayElements; i++)
            {
                CCD.DisplayConfigPathInfo pathInfo = pathInfoArray[i];

                if (!adapterIdToAdapterName.ContainsKey(pathInfo.sourceInfo.adapterId))
                {
                    CCD.DisplayConfigAdapterName adapterName = new CCD.DisplayConfigAdapterName();
                    adapterName.header.type = CCD.DisplayConfigDeviceInfoType.GetAdapterName;
                    adapterName.header.adapterId = pathInfo.sourceInfo.adapterId;
                    adapterName.header.size = (uint)Marshal.SizeOf(adapterName);

                    Win32Utilities.ThrowIfResultCodeNotSuccess(CCD.DisplayConfigGetDeviceInfo(ref adapterName));

                    adapterIdToAdapterName.Add(
                        adapterName.header.adapterId,
                        adapterName.adapterDevicePath);
                }
            }

            // Flip the mapping to adapter name to LUID for usage by DisplayPresets.
            foreach (KeyValuePair<CCD.LUID, string> luidStringPair in adapterIdToAdapterName)
            {
                mCurrentRuntimeDeviceAdapterNamesToAdapterIds.Add(
                    luidStringPair.Value, luidStringPair.Key);
            }
        }

        private static AdapterIdMapper mInstance;

        private Dictionary<string, CCD.LUID> mCurrentRuntimeDeviceAdapterNamesToAdapterIds;
    }
}
