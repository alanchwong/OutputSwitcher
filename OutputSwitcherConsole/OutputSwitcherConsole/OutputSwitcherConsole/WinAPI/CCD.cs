using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcherConsole.WinAPI
{
    public class CCD
    {
        public enum DisplayConfigVideoOutputTechnology : uint
        {
            Other = 4294967295, // -1
            Hd15 = 0,
            Svideo = 1,
            CompositeVideo = 2,
            ComponentVideo = 3,
            Dvi = 4,
            Hdmi = 5,
            Lvds = 6,
            DJpn = 8,
            Sdi = 9,
            DisplayportExternal = 10,
            DisplayportEmbedded = 11,
            UdiExternal = 12,
            UdiEmbedded = 13,
            Sdtvdongle = 14,
            Miracast = 15,
            Internal = 0x80000000,
            ForceUint32 = 0xFFFFFFFF
        }

        #region SdcFlags enum

        [Flags]
        public enum SdcFlags : uint
        {
            Zero = 0,

            TopologyInternal = 0x00000001,
            TopologyClone = 0x00000002,
            TopologyExtend = 0x00000004,
            TopologyExternal = 0x00000008,
            TopologySupplied = 0x00000010,

            UseSuppliedDisplayConfig = 0x00000020,
            Validate = 0x00000040,
            Apply = 0x00000080,
            NoOptimization = 0x00000100,
            SaveToDatabase = 0x00000200,
            AllowChanges = 0x00000400,
            PathPersistIfRequired = 0x00000800,
            ForceModeEnumeration = 0x00001000,
            AllowPathOrderChanges = 0x00002000,

            UseDatabaseCurrent = TopologyInternal | TopologyClone | TopologyExtend | TopologyExternal
        }

        [Flags]
        public enum DisplayConfigFlags : uint
        {
            /*
             * (from Win10 version of wingdi.h)
             * #define DISPLAYCONFIG_PATH_ACTIVE               0x00000001
             * #define DISPLAYCONFIG_PATH_PREFERRED_UNSCALED   0x00000004
             * #define DISPLAYCONFIG_PATH_SUPPORT_VIRTUAL_MODE 0x00000008
             * #define DISPLAYCONFIG_PATH_VALID_FLAGS          0x0000000D
             */

            Zero = 0x0,
            PathActive = 0x00000001,
            PathPreferredUnscaled = 0x00000004,
            PathSupportVirtualMode = 0x00000008,
            PathValidFlags = 0x0000000D,
        }

        [Flags]
        public enum DisplayConfigSourceStatus
        {
            Zero = 0x0,
            InUse = 0x00000001
        }

        [Flags]
        public enum DisplayConfigTargetStatus : uint
        {
            Zero = 0x0,

            InUse = 0x00000001,
            FORCIBLE = 0x00000002,
            ForcedAvailabilityBoot = 0x00000004,
            ForcedAvailabilityPath = 0x00000008,
            ForcedAvailabilitySystem = 0x00000010,
        }

        [Flags]
        public enum DisplayConfigRotation : uint
        {
            Zero = 0x0,

            Identity = 1,
            Rotate90 = 2,
            Rotate180 = 3,
            Rotate270 = 4,
            ForceUint32 = 0xFFFFFFFF
        }

        [Flags]
        public enum DisplayConfigPixelFormat : uint
        {
            Zero = 0x0,

            Pixelformat8Bpp = 1,
            Pixelformat16Bpp = 2,
            Pixelformat24Bpp = 3,
            Pixelformat32Bpp = 4,
            PixelformatNongdi = 5,
            PixelformatForceUint32 = 0xffffffff
        }

        [Flags]
        public enum DisplayConfigScaling : uint
        {
            Zero = 0x0,

            Identity = 1,
            Centered = 2,
            Stretched = 3,
            Aspectratiocenteredmax = 4,
            Custom = 5,
            Preferred = 128,
            ForceUint32 = 0xFFFFFFFF
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public UInt32 LowPart;
            public UInt32 HighPart;

            public override string ToString()
            {
                return "High: " + HighPart + ", Low: " + LowPart;
            }
        }

        [Flags]
        public enum DisplayConfigScanLineOrdering : uint
        {
            Unspecified = 0,
            Progressive = 1,
            Interlaced = 2,
            InterlacedUpperfieldfirst = Interlaced,
            InterlacedLowerfieldfirst = 3,
            ForceUint32 = 0xFFFFFFFF
        }

        public enum DisplayConfigModeInfoType : UInt32
        {
            Zero = 0,

            Source = 1,
            Target = 2,
            Desktop = 3,
            ForceUint32 = 0xFFFFFFFF
        }

        [Flags]
        public enum D3DmdtVideoSignalStandard : UInt16
        {
            Uninitialized = 0,
            VesaDmt = 1,
            VesaGtf = 2,
            VesaCvt = 3,
            Ibm = 4,
            Apple = 5,
            NtscM = 6,
            NtscJ = 7,
            Ntsc443 = 8,
            PalB = 9,
            PalB1 = 10,
            PalG = 11,
            PalH = 12,
            PalI = 13,
            PalD = 14,
            PalN = 15,
            PalNc = 16,
            SecamB = 17,
            SecamD = 18,
            SecamG = 19,
            SecamH = 20,
            SecamK = 21,
            SecamK1 = 22,
            SecamL = 23,
            SecamL1 = 24,
            Eia861 = 25,
            Eia861A = 26,
            Eia861B = 27,
            PalK = 28,
            PalK1 = 29,
            PalL = 30,
            PalM = 31,
            Other = 255
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigRational
        {
            public uint numerator;
            public uint denominator;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigPathInfo
        {
            public DisplayConfigPathSourceInfo sourceInfo;
            public DisplayConfigPathTargetInfo targetInfo;
            public uint flags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Explicit)]
        public struct DisplayConfigModeInfo
        {
            [FieldOffset(0)]
            public DisplayConfigModeInfoType infoType;

            [FieldOffset(4)]
            public UInt32 id;

            [FieldOffset(8)]
            public LUID adapterId;

            // Since the targetMode, sourceMode, and desktopImageInfo properties all occupy the same
            // field offset but are different in size and makeup, we need to tell the serializer when
            // they should be serialized, and when not to be. Apparently there is a pattern that the
            // XML serializer looks for, public bool ShouldSerialize<propertyname>, to determine
            // whether a given attribute should be serialized. Maybe this is how the "code behind" of
            // some serialization attribute works?

            [FieldOffset(16)]
            public DisplayConfigTargetMode targetMode;
            public bool ShouldSerializetargetMode() { return infoType == DisplayConfigModeInfoType.Target; }

            [FieldOffset(16)]
            public DisplayConfigSourceMode sourceMode;
            public bool ShouldSerializesourceMode() { return infoType == DisplayConfigModeInfoType.Source; }

            [FieldOffset(16)]
            public DisplayConfigDesktopImageInfo desktopImageInfo;
            public bool ShouldSerializedesktopImageInfo() { return infoType == DisplayConfigModeInfoType.Desktop; }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfig2DRegion
        {
            public uint cx;
            public uint cy;
        }

        [Serializable]
        [StructLayout(LayoutKind.Explicit)]
        public struct DisplayConfigVideoSignalInfo
        {
            [FieldOffset(0)]
            public UInt64 pixelRate;

            [FieldOffset(8)]
            public DisplayConfigRational hSyncFreq;

            [FieldOffset(16)]
            public DisplayConfigRational vSyncFreq;

            [FieldOffset(24)]
            public DisplayConfig2DRegion activeSize;

            [FieldOffset(32)]
            public DisplayConfig2DRegion totalSize;

            /* In Windows 8.1 and later, the next 32 bits are:
             *  - First 16 bits: D3DmdtVideoSignalStandard
             *  - Next 6 bits: unsigned int vSyncFreqDivider
             *  - Next 10 bits: reserved and unused
             * Otherwise, the next 32 bits are D3DmdtVideoSignalStandard.
             * See: https://msdn.microsoft.com/en-us/library/windows/hardware/ff554007(v=vs.85).aspx
             */

            [FieldOffset(40)]
            public D3DmdtVideoSignalStandard videoStandard;

            [FieldOffset(42)]
            public VSyncFreqDivider vSyncFreqDivider;

            [FieldOffset(44)]
            public DisplayConfigScanLineOrdering ScanLineOrdering;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct VSyncFreqDivider
        {
            // Win 8.1+ only, used in DisplayConfigVideoSignalInfo.
            // First 6 bits are the vSyncFreqDivider as an unsigned int.
            // The remaining 10 bits are reserved and unused.

            public UInt16 vSyncFreqDividerAndReserved;

            public UInt16 GetVSyncFreqDivider()
            {
                // Mask off the reserved bits -- no telling what they're set to.
                return (UInt16)(vSyncFreqDividerAndReserved & 0x003F);
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigTargetMode
        {
            public DisplayConfigVideoSignalInfo targetVideoSignalInfo;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigSourceMode
        {
            public uint width;
            public uint height;
            public DisplayConfigPixelFormat pixelFormat;
            public POINTL position;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigDesktopImageInfo
        {
            public POINTL pathSourceSize;
            public RECTL desktopImageRegion;
            public RECTL desktopImageClip;
        }

        [Serializable]
        [StructLayout(LayoutKind.Explicit)]
        public struct DisplayConfigPathSourceInfo
        {
            [FieldOffset(0)]
            public LUID adapterId;

            [FieldOffset(8)]
            public UInt32 id;

            /* In the Win10 C++ definition of this struct, there is a union
             * between modeInfoIdx and a struct containing cloneGroupId and
             * sourceModeInfoIdx. If the path source supports virtual mode
             * the struct is used, otherwise modeInfoIdx is used.
             * See: https://msdn.microsoft.com/en-us/library/windows/hardware/ff553951%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
             */

            [FieldOffset(12)]
            public UInt32 modeInfoIdx;

            [FieldOffset(12)]
            public UInt16 cloneGroupId;

            [FieldOffset(14)]
            public UInt16 sourceModeInfoIdx;

            [FieldOffset(16)]
            public DisplayConfigSourceStatus statusFlags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Explicit)]
        public struct DisplayConfigPathTargetInfo
        {
            [FieldOffset(0)]
            public LUID adapterId;

            [FieldOffset(8)]
            public UInt32 id;

            /* In the Win10 definition of this struct, there is a union between
             * modeInfoIdx and a struct containing two 16-bit fields, desktopModeInfoIdx
             * and targetModeInfoIdx. The latter two fields are used if the path
             * supports virtual mode.
             * See: https://msdn.microsoft.com/en-us/library/windows/hardware/ff553954(v=vs.85).aspx
             */

            [FieldOffset(12)]
            public UInt32 modeInfoIdx;

            [FieldOffset(12)]
            public UInt16 desktopModeInfoIdx;

            [FieldOffset(14)]
            public UInt16 targetModeInfoIdx;

            [FieldOffset(16)]
            public DisplayConfigVideoOutputTechnology outputTechnology;

            [FieldOffset(20)]
            public DisplayConfigRotation rotation;

            [FieldOffset(24)]
            public DisplayConfigScaling scaling;

            [FieldOffset(28)]
            public DisplayConfigRational refreshRate;

            [FieldOffset(36)]
            public DisplayConfigScanLineOrdering scanLineOrdering;

            [FieldOffset(40)]
            public bool targetAvailable;

            [FieldOffset(44)]
            public DisplayConfigTargetStatus statusFlags;
        }

        [Flags]
        public enum QueryDisplayFlags : uint
        {
            Zero = 0x0,

            AllPaths = 0x00000001,
            OnlyActivePaths = 0x00000002,
            DatabaseCurrent = 0x00000004
        }

        [Flags]
        public enum DisplayConfigTopologyId : uint
        {
            Zero = 0x0,

            Internal = 0x00000001,
            Clone = 0x00000002,
            Extend = 0x00000004,
            External = 0x00000008,
            ForceUint32 = 0xFFFFFFFF
        }

        public enum DisplayConfigDeviceInfoType : UInt32
        {
            GetSourceName = 1,
            GetTargetName = 2,
            GetTargetPreferredMode = 3,
            GetAdapterName = 4,
            SetTargetPersistence = 5,
            GetTargetBaseType = 6,
            GetSupportVirtualResolution = 7,
            SetSupportVirtualResolution = 8,
            ForceUInt32 = 0xFFFFFFFF,
        } 

        /// <summary>
        /// Empty interface to provide common root type for all structs used with DisplayConfigGetDeviceInfo.
        /// </summary>        
        public interface IDisplayConfigInfo {}

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigDeviceInfoHeader
        {
            public DisplayConfigDeviceInfoType type;
            public UInt32 size;
            public LUID adapterId;
            public UInt32 id;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DisplayConfigSourceDeviceName : IDisplayConfigInfo
        {
            public DisplayConfigDeviceInfoHeader header;

            // Underlying type is WCHAR array of size CCHDEVICENAME which is 32 (defined in wingdi.h)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string viewGdiDeviceName; 

        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigTargetDeviceNameFlags
        {
            /// <summary>
            /// This is a 32-bit value that actually contains 3 bit fields and 29 reserved bytes.
            /// </summary>
            public UInt32 value;

            public bool IsFriendlyNameFromEdid()
            {
                return (value & 0x00000001) > 0;
            }

            public bool IsFriendlyNameForced()
            {
                return (value & 0x00000002) > 0;
            }

            public bool AreEdidIdsValid()
            {
                return (value & 0x00000004) > 0;
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DisplayConfigTargetDeviceName : IDisplayConfigInfo
        {
            public DisplayConfigDeviceInfoHeader header;

            public DisplayConfigTargetDeviceNameFlags flags;

            public DisplayConfigVideoOutputTechnology outputTechnology;

            public UInt16 edidManufactureId;

            public UInt16 edidProductCodeId;

            public UInt32 connectorInstance;
            
            // Underlying type is fixed length WCHAR array
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string monitorFriendlyDeviceName;
            
            // Underlying type is fixed length WCHAR array
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string monitorDevicePath;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayConfigTargetPreferredMode : IDisplayConfigInfo
        {
            public DisplayConfigDeviceInfoHeader header;
            public UInt32 width;
            public UInt32 height;
            public DisplayConfigTargetMode targetMode;
        }

        #endregion

        [DllImport("User32.dll")]
        public static extern int SetDisplayConfig(
            uint numPathArrayElements,
            [In] DisplayConfigPathInfo[] pathArray,
            uint numModeInfoArrayElements,
            [In] DisplayConfigModeInfo[] modeInfoArray,
            SdcFlags flags);

        [DllImport("User32.dll")]
        public static extern int QueryDisplayConfig(
            QueryDisplayFlags flags, 
            ref int numPathArrayElements,
            [Out] DisplayConfigPathInfo[] pathInfoArray,
            ref int modeInfoArrayElements,
            [Out] DisplayConfigModeInfo[] modeInfoArray,
            IntPtr z);

        /// <summary>
        /// The GetDisplayConfigBufferSizes function retrieves the size of the buffers that are 
        /// required to call the QueryDisplayConfig function.
        /// </summary>
        /// <param name="flags">Type of information to retrieve.</param>
        /// <param name="numPathArrayElements">Receives the number of elements in the path information table. Used by subsequent call to QueryDisplayConfig.</param>
        /// <param name="numModeInfoArrayElements">Receives the number of elements in the mode information table. Used by subsequent call to QueryDisplayConfig.</param>
        /// <returns>System error code. Returns 0 (ERROR_SUCCESS if successful.)</returns>
        [DllImport("User32.dll")]
        public static extern int GetDisplayConfigBufferSizes(
            QueryDisplayFlags flags, 
            out int numPathArrayElements, 
            out int numModeInfoArrayElements);

        [DllImport("User32.dll")]
        private static extern int DisplayConfigGetDeviceInfo(IntPtr requestPacket);
        public static int DisplayConfigGetDeviceInfo<T>(ref T displayConfig) where T : IDisplayConfigInfo
        {
            // Allocate unmanaged memory in the size of the supplied IDisplayConfigInfo struct.
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(displayConfig));

            // Copy the struct's data to the unmanaged memory.
            Marshal.StructureToPtr(displayConfig, ptr, false);

            // This works because in all of the IDisplayConfigInfo structs, the first chunk
            // of memory is always the DisplayConfigDeviceInfoHeader. So passing a pointer
            // to any of the structs is equivalent to passing a pointer to a DisplayConfigDeviceInfoHeader.
            int resultCode = DisplayConfigGetDeviceInfo(ptr);

            // Re-copy the data in the unmanaged memory, which should have been modified by 
            // DisplayConfigGetDeviceInfo, to the reference originally supplied.
            displayConfig = Marshal.PtrToStructure<T>(ptr);

            // Copying is done, free the unmanaged memory!
            Marshal.FreeHGlobal(ptr);

            return resultCode;
        }
    }
}
