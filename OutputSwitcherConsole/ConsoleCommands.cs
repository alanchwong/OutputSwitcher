using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using OutputSwitcher.Core;
using OutputSwitcher.WinAPI;

namespace OutputSwitcherConsole
{
    class ConsoleCommands
    {
        /// <summary>
        /// Terminates the application with a return code of 0.
        /// </summary>
        static public void Exit()
        {
            DisplayPresetCollection.GetDisplayPresetCollection().PersistDisplayPresetsIfDirty();   // Write to disk before exiting.

            Environment.Exit(0);
        }

        static public void ListPresets()
        {
            DisplayPresetCollection presetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            int index = 1;

            foreach (DisplayPreset displayPreset in presetCollection.GetPresets())
            {
                Console.WriteLine(String.Format("{0}. {1}", index, displayPreset.Name));
                index++;
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

        public static void TestCCDExampleCodeAll()
        {
            Console.WriteLine("** ALL PATHS **");
            TestCCDExampleCode(CCD.QueryDisplayFlags.AllPaths);
        }

        public static void TestCCDExampleCodeOnlyActive()
        {
            Console.WriteLine("** ONLY ACTIVE PATHS **");
            TestCCDExampleCode(CCD.QueryDisplayFlags.OnlyActivePaths);
        }

        public static void TestCCDExampleCode(CCD.QueryDisplayFlags queryDisplayFlags)
        {
            int resultCode;

            int numPathArrayElements;
            int numModeInfoArrayElements;

            // Get buffer size required to enumerate all valid paths.
            resultCode = CCD.GetDisplayConfigBufferSizes(queryDisplayFlags, out numPathArrayElements, out numModeInfoArrayElements);

            Win32Utilities.ThrowIfResultCodeNotSuccess(resultCode);

            CCD.DisplayConfigPathInfo[] pathInfoArray = new CCD.DisplayConfigPathInfo[numPathArrayElements];
            CCD.DisplayConfigModeInfo[] modeInfoArray = new CCD.DisplayConfigModeInfo[numModeInfoArrayElements];

            resultCode = CCD.QueryDisplayConfig(
                queryDisplayFlags,
                ref numPathArrayElements,
                pathInfoArray,
                ref numModeInfoArrayElements,
                modeInfoArray,
                IntPtr.Zero);

            Win32Utilities.ThrowIfResultCodeNotSuccess(resultCode);

            foreach (CCD.DisplayConfigPathInfo configPathInfo in pathInfoArray)
            {
                ConsoleOutputUtilities.WriteDisplayConfigPathInfoToConsole(configPathInfo);
                Console.WriteLine();
            }

            foreach (CCD.DisplayConfigModeInfo configModeInfo in modeInfoArray)
            {
                ConsoleOutputUtilities.WriteDisplayConfigModeInfoToConsole(configModeInfo);
                Console.WriteLine();
            }

            // Find and store the primary path based by looking for an active path that is located at desktop position
            // 0,0
            CCD.DisplayConfigPathInfo primaryPath = new CCD.DisplayConfigPathInfo();
            bool primaryPathFound = false;

            foreach (CCD.DisplayConfigPathInfo configPathInfo in pathInfoArray)
            {
                if ((configPathInfo.flags & (uint)CCD.DisplayConfigFlags.PathActive) > 0)
                {
                    if (configPathInfo.sourceInfo.modeInfoIdx > modeInfoArray.Length - 1)
                        throw new Exception("Config Path Info Source Mode Info Index is out of range.");

                    CCD.DisplayConfigModeInfo modeInfo = modeInfoArray[configPathInfo.sourceInfo.modeInfoIdx];

                    if (modeInfo.infoType == CCD.DisplayConfigModeInfoType.Source &&
                        modeInfo.sourceMode.position.x == 0 &&
                        modeInfo.sourceMode.position.y == 0)
                    {
                        // Bingo
                        primaryPath = configPathInfo;
                        primaryPathFound = true;
                        break;
                    }
                }
            }

            if (!primaryPathFound)
                throw new Exception("Failed to find primary display path!");

            CCD.DisplayConfigTargetDeviceName dcTargetDeviceName = new CCD.DisplayConfigTargetDeviceName();
            dcTargetDeviceName.header.type = CCD.DisplayConfigDeviceInfoType.GetTargetName;
            dcTargetDeviceName.header.size = (uint)Marshal.SizeOf(dcTargetDeviceName);
            dcTargetDeviceName.header.adapterId = primaryPath.targetInfo.adapterId;
            dcTargetDeviceName.header.id = primaryPath.targetInfo.id;

            resultCode = CCD.DisplayConfigGetDeviceInfo(ref dcTargetDeviceName);

            Win32Utilities.ThrowIfResultCodeNotSuccess(resultCode);

            ConsoleOutputUtilities.WriteDisplayConfigTargetDeviceNameToConsole(dcTargetDeviceName);
            Console.WriteLine();

            CCD.DisplayConfigSourceDeviceName dcSourceDeviceName = new CCD.DisplayConfigSourceDeviceName();
            dcSourceDeviceName.header.type = CCD.DisplayConfigDeviceInfoType.GetSourceName;
            dcSourceDeviceName.header.size = (uint)Marshal.SizeOf(dcSourceDeviceName);
            dcSourceDeviceName.header.adapterId = primaryPath.sourceInfo.adapterId;
            dcSourceDeviceName.header.id = primaryPath.sourceInfo.id;

            resultCode = CCD.DisplayConfigGetDeviceInfo(ref dcSourceDeviceName);

            Win32Utilities.ThrowIfResultCodeNotSuccess(resultCode);

            CCD.DisplayConfigTargetPreferredMode dcTargetPreferredMode = new CCD.DisplayConfigTargetPreferredMode();
            dcTargetPreferredMode.header.type = CCD.DisplayConfigDeviceInfoType.GetTargetPreferredMode;
            dcTargetPreferredMode.header.size = (uint)Marshal.SizeOf(dcTargetPreferredMode);
            dcTargetPreferredMode.header.adapterId = primaryPath.targetInfo.adapterId;
            dcTargetPreferredMode.header.id = primaryPath.targetInfo.id;

            Win32Utilities.ThrowIfResultCodeNotSuccess(CCD.DisplayConfigGetDeviceInfo(ref dcTargetPreferredMode));
        }

        public static void ListPresetDetail()
        {
            DisplayPresetCollection displayPresetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            List<DisplayPreset> displayPresets = displayPresetCollection.GetPresets();

            Console.WriteLine("Available presets: ");

            IEnumerator<DisplayPreset> displayPresetsEnumerator = displayPresets.GetEnumerator();

            for (int i = 0; i < displayPresets.Count; i++)
            {
                displayPresetsEnumerator.MoveNext();
                Console.WriteLine(String.Format("[{0}] {1}", i, displayPresetsEnumerator.Current.Name));
            }

            Console.Write("Select preset: ");
            string selection = Console.ReadLine();

            int selectedPresetIndex = -1;
            string selectedPresetName = String.Empty;

            if (!Int32.TryParse(selection, out selectedPresetIndex))
            {
                // If it's not a number, assume the user typed in a name
                selectedPresetName = selection;
            }
            else if (selectedPresetIndex >= displayPresets.Count)
            {
                Console.WriteLine("Invalid selection!");
                return;
            }
            else
            {
                selectedPresetName = displayPresets[selectedPresetIndex].Name;
            }

            DisplayPreset selectedPreset = displayPresetCollection.GetPreset(selectedPresetName);

            if (selectedPreset == null)
            {
                Console.WriteLine("Invalid preset name: " + selectedPresetName);
                return;
            }

            ConsoleOutputUtilities.WriteDisplayPresetToConsole(selectedPreset);
        }

        static public void DisplayHelp()
        {
            NonInteractiveConsoleCommands.DisplayHelp(null);
        }
    }
}
