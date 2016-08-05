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
        }

        public static AdapterIdMapper GetAdapterIdMapper()
        {
            if (mInstance == null)
                mInstance = new AdapterIdMapper();

            return mInstance;
        }

        public bool GetCurrentAdapterIdForDevice(string displayAdapterDevicePath, out CCD.LUID adapterId)
        {
            return mCurrentRuntimeDeviceAdapterNamesToAdapterIds.TryGetValue(displayAdapterDevicePath, out adapterId);
        }

        public DisplayPresetAdapterIdValidation ValidateDisplayPresetAdapterIds(DisplayPreset displayPreset)
        {
            bool allAdapterNamesAccountedFor = true;
            bool allAdapterIdsValid = true;

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

            if (!allAdapterNamesAccountedFor)
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
            foreach (CCD.DisplayConfigPathInfo pathInfo in pathInfoArray)
            {
                if (!adapterIdToAdapterName.ContainsKey(pathInfo.sourceInfo.adapterId))
                {
                    CCD.DisplayConfigAdapterName adapterName = new CCD.DisplayConfigAdapterName();
                    adapterName.header.type = CCD.DisplayConfigDeviceInfoType.GetAdapterName;
                    adapterName.header.adapterId = pathInfo.sourceInfo.adapterId;
                    adapterName.header.size = (uint)Marshal.SizeOf(adapterName);

                    // TODO: Uhhh... silently ignore errors?
                    if (CCD.DisplayConfigGetDeviceInfo(ref adapterName) == Win32Constants.ERROR_SUCCESS)
                    {
                        adapterIdToAdapterName.Add(
                            adapterName.header.adapterId,
                            adapterName.adapterDevicePath);
                    }
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
