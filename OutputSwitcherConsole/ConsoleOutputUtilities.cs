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
        static private bool IsFlagSet(uint flagfield, uint desiredFlag)
        {
            return (flagfield & desiredFlag) > 0;
        }

        static private string GetIsFlagSetYesNoString(uint flagField, uint desiredFlag)
        {
            if ((flagField & desiredFlag) > 0)
                return "Yes";
            else
                return "No";
        }

        static private string RECTLToString(RECTL rectL)
        {
            return "L: " + rectL.left + " T: " + rectL.top + " R: " + rectL.right + " B: " + rectL.bottom;
        }

        static public void WriteDisplayPresetToConsole(DisplayPreset displayPreset)
        {
            Console.WriteLine("Preset Name: " + displayPreset.Name);

            foreach (CCD.DisplayConfigPathInfo dcPathInfo in displayPreset.PathInfoArray)
            {
                WriteDisplayConfigPathInfoToConsole(dcPathInfo);
                Console.WriteLine();
            }

            foreach (CCD.DisplayConfigModeInfo dcModeInfo in displayPreset.ModeInfoArray)
            {
                WriteDisplayConfigModeInfoToConsole(dcModeInfo);
                Console.WriteLine();
            }

            foreach (CCD.DisplayConfigTargetDeviceName dcTargetDeviceName in displayPreset.TargetDeviceNames)
            {
                WriteDisplayConfigTargetDeviceNameToConsole(dcTargetDeviceName);
                Console.WriteLine();
            }
        }

        static public void WriteDisplayConfigPathInfoToConsole(CCD.DisplayConfigPathInfo displayConfigPathInfo)
        {
            WriteDisplayConfigPathSourceInfoToConsole(displayConfigPathInfo.sourceInfo, IsFlagSet(displayConfigPathInfo.flags, (uint)CCD.DisplayConfigFlags.PathSupportVirtualMode));
            WriteDisplayConfigPathTargetInfoToConsole(displayConfigPathInfo.targetInfo, IsFlagSet(displayConfigPathInfo.flags, (uint)CCD.DisplayConfigFlags.PathSupportVirtualMode));

            Console.WriteLine("Path info flags:");
            Console.WriteLine("Active: " + GetIsFlagSetYesNoString(displayConfigPathInfo.flags, (uint)CCD.DisplayConfigFlags.PathActive) +
                                ", Preferred Unscaled: " + GetIsFlagSetYesNoString(displayConfigPathInfo.flags, (uint)CCD.DisplayConfigFlags.PathPreferredUnscaled) +
                                ", Support Virtual Mode: " + GetIsFlagSetYesNoString(displayConfigPathInfo.flags, (uint)CCD.DisplayConfigFlags.PathSupportVirtualMode));
        }

        static public void WriteDisplayConfigPathSourceInfoToConsole(CCD.DisplayConfigPathSourceInfo displayConfigPathSourceInfo, bool isVirtualModeSupported = false)
        {
            Console.WriteLine("Path Source Info:");
            Console.WriteLine("\tAdapter Id: " + displayConfigPathSourceInfo.adapterId);
            Console.WriteLine("\tId: " + displayConfigPathSourceInfo.id);

            if (isVirtualModeSupported)
            {
                Console.WriteLine("\tClone Group Id: " + displayConfigPathSourceInfo.cloneGroupId);
                Console.WriteLine("\tSource Mode Info Index: " + displayConfigPathSourceInfo.sourceModeInfoIdx);
            }
            else
            {
                Console.WriteLine("\tMode Info Index: " + displayConfigPathSourceInfo.modeInfoIdx);
            }

            Console.WriteLine("\tSource in use? " + GetIsFlagSetYesNoString((uint)displayConfigPathSourceInfo.statusFlags, (uint)CCD.DisplayConfigSourceStatus.InUse));
        }

        static public void WriteDisplayConfigPathTargetInfoToConsole(CCD.DisplayConfigPathTargetInfo displayConfigPathTargetInfo, bool isVirtualModeSupported = false)
        {
            Console.WriteLine("Path Target Info:");
            Console.WriteLine("\tAdapter Id: " + displayConfigPathTargetInfo.adapterId);
            Console.WriteLine("\tId: " + displayConfigPathTargetInfo.id);

            if (isVirtualModeSupported)
            {
                Console.WriteLine("\tDesktop Mode Info Index: " + displayConfigPathTargetInfo.desktopModeInfoIdx);
                Console.WriteLine("\tTarget Mode Info Idx: " + displayConfigPathTargetInfo.targetModeInfoIdx);
            }
            else
            {
                Console.WriteLine("\tMode Info Index: " + displayConfigPathTargetInfo.modeInfoIdx);
            }

            Console.WriteLine("\tVideo Output Technology: " + displayConfigPathTargetInfo.outputTechnology);
            Console.WriteLine("\tConfig Rotation: " + displayConfigPathTargetInfo.rotation);
            Console.WriteLine("\tConfig Scaling: " + displayConfigPathTargetInfo.scaling);
            Console.WriteLine("\tRefresh Rate: " + displayConfigPathTargetInfo.refreshRate.numerator / (double)displayConfigPathTargetInfo.refreshRate.denominator);
            Console.WriteLine("\tScanline Ordering: " + displayConfigPathTargetInfo.scanLineOrdering);
            Console.WriteLine("\tAvailable? " + displayConfigPathTargetInfo.targetAvailable);
            Console.WriteLine("\tIn Use? " + GetIsFlagSetYesNoString((uint)displayConfigPathTargetInfo.statusFlags, (uint)CCD.DisplayConfigTargetStatus.InUse));
            Console.WriteLine("\tForcible? " + GetIsFlagSetYesNoString((uint)displayConfigPathTargetInfo.statusFlags, (uint)CCD.DisplayConfigTargetStatus.FORCIBLE));
            Console.WriteLine("\tForced Availability Boot? " + GetIsFlagSetYesNoString((uint)displayConfigPathTargetInfo.statusFlags, (uint)CCD.DisplayConfigTargetStatus.ForcedAvailabilityBoot));
            Console.WriteLine("\tForced Availability Path? " + GetIsFlagSetYesNoString((uint)displayConfigPathTargetInfo.statusFlags, (uint)CCD.DisplayConfigTargetStatus.ForcedAvailabilityPath));
            Console.WriteLine("\tForced Availability System? " + GetIsFlagSetYesNoString((uint)displayConfigPathTargetInfo.statusFlags, (uint)CCD.DisplayConfigTargetStatus.ForcedAvailabilitySystem));
        }

        static public void WriteDisplayConfigModeInfoToConsole(CCD.DisplayConfigModeInfo modeInfo)
        {
            Console.WriteLine("Mode Info Type: " + modeInfo.infoType);
            Console.WriteLine("Adapter Id: " + modeInfo.adapterId);
            Console.WriteLine("Id: " + modeInfo.id);
            
            switch (modeInfo.infoType)
            {
                case CCD.DisplayConfigModeInfoType.Target:
                    WriteDisplayConfigTargetModeToConsole(modeInfo.targetMode);
                    break;
                case CCD.DisplayConfigModeInfoType.Source:
                    WriteDisplayConfigSourceModeToConsole(modeInfo.sourceMode);
                    break;
                case CCD.DisplayConfigModeInfoType.Desktop:
                    WriteDisplayConfigDesktopImageInfoToConsole(modeInfo.desktopImageInfo);
                    break;
                default:
                    break;                    
            }
        }

        static public void WriteDisplayConfigTargetModeToConsole(CCD.DisplayConfigTargetMode targetMode)
        {
            CCD.DisplayConfigVideoSignalInfo signalInfo = targetMode.targetVideoSignalInfo;

            Console.WriteLine("Target Mode:");
            Console.WriteLine("\tPixel Rate: " + signalInfo.pixelRate);
            Console.WriteLine("\tHSync Freq: " + signalInfo.hSyncFreq.numerator / signalInfo.hSyncFreq.denominator);
            Console.WriteLine("\tVSync Freq: " + signalInfo.vSyncFreq.numerator / signalInfo.vSyncFreq.denominator);
            Console.WriteLine("\tActive Size: x:" + signalInfo.activeSize.cx + " y: " + signalInfo.activeSize.cy);
            Console.WriteLine("\tVideo Standard: " + signalInfo.videoStandard);
            Console.WriteLine("\tVSync Freq Divider: " + signalInfo.vSyncFreqDivider.GetVSyncFreqDivider());
            Console.WriteLine("\tScanline Ordering: " + signalInfo.ScanLineOrdering);
        }

        static public void WriteDisplayConfigSourceModeToConsole(CCD.DisplayConfigSourceMode sourceMode)
        {
            Console.WriteLine("Source Mode:");
            Console.WriteLine("\tWidth: " + sourceMode.width + " Height: " + sourceMode.height);
            Console.WriteLine("\tPixel Format: " + sourceMode.pixelFormat);
            Console.WriteLine("\tPosition: x: " + sourceMode.position.x + " y: " + sourceMode.position.y);
        }

        static public void WriteDisplayConfigDesktopImageInfoToConsole(CCD.DisplayConfigDesktopImageInfo desktopImageInfo)
        {
            Console.WriteLine("Desktop Image Info: ");
            Console.WriteLine("\tPath Source Size (the size of source surface displayed on the monitor): " + desktopImageInfo.pathSourceSize);
            Console.WriteLine("\tDesktop Image Region: " + RECTLToString(desktopImageInfo.desktopImageRegion));
            Console.WriteLine("\tDesktop Image Clip: " + RECTLToString(desktopImageInfo.desktopImageClip));
        }

        static public void WriteDisplayConfigDeviceInfoHeader(CCD.DisplayConfigDeviceInfoHeader header)
        {
            Console.WriteLine("DisplayConfigDeviceInfoHeader: ");
            Console.WriteLine("Type: " + header.type);
            Console.WriteLine("Adapter Id: " + header.adapterId);
            Console.WriteLine("Id: " + header.id);
        }

        static public void WriteDisplayConfigTargetDeviceNameToConsole(CCD.DisplayConfigTargetDeviceName targetDeviceName)
        {
            WriteDisplayConfigDeviceInfoHeader(targetDeviceName.header);

            Console.WriteLine("IsFriendlyNameFromEdid: " + targetDeviceName.flags.IsFriendlyNameFromEdid() +
                                ", IsFriendlyNameForced: " + targetDeviceName.flags.IsFriendlyNameForced() +
                                ", AreEdidIdsValid: " + targetDeviceName.flags.AreEdidIdsValid());
            Console.WriteLine("Output Technology: " + targetDeviceName.outputTechnology);
            Console.WriteLine("Edid Manufacture Id: " + targetDeviceName.edidManufactureId +
                                ", Edid Product Code Id: " + targetDeviceName.edidProductCodeId +
                                ", Connector INstance: " + targetDeviceName.connectorInstance);
            Console.WriteLine("Monitor Friendly Name: " + targetDeviceName.monitorFriendlyDeviceName);
            Console.WriteLine("Monitor Device Path: " + targetDeviceName.monitorDevicePath);
        }
    }
}
