using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcherConsole.Data;

namespace OutputSwitcherConsole
{
    public class NonInteractiveConsoleCommands
    {
        public static void DisplayHelp(string[] args)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("For non-interactive mode: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName + " <command> <command arguments>");
            Console.WriteLine("For interactive mode: omit <command> and <command arguments>");
            Console.WriteLine();
            Console.WriteLine("Available non-interactive mode commands:");
            Console.WriteLine("Help\t\tDisplays this help info.");
            Console.WriteLine("Capture\t\tCaptures current display configuration as a new preset with the supplied preset name.");
            Console.WriteLine("Apply\t\tApplies a previously saved preset display configuration with the supplied preset name.");
        }

        public static void CaptureCurrentConfigAndSaveAsPreset(string[] args)
        {
            // Trim quotes in case the preset name has spaces
            string presetName = args[1].Trim(new char[] { '"', '\'' });

            DisplayPreset displayPreset = DisplayPresetRecorderAndApplier.RecordCurrentConfiguration(presetName);

            DisplayPresetCollection displayPresetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            if (displayPresetCollection.TryAddDisplayPreset(displayPreset))
            {
                Console.WriteLine(String.Format("New display preset '{0}' saved!", presetName));
            }
            else
            {
                Console.WriteLine("Failed to save preset.");
            }
        }

        public static void ApplyPreset(string[] args)
        {

            // Trim quotes in case the preset name has spaces
            string presetName = args[1].Trim(new char[] { '"', '\'' });

            DisplayPresetCollection displayPresetCollection = DisplayPresetCollection.GetDisplayPresetCollection();

            DisplayPreset displayPreset = displayPresetCollection.GetPreset(presetName);

            if (displayPreset == null)
            {
                Console.WriteLine(String.Format("Preset '{0}' does not exist.", presetName));
                return;
            }

            DisplayPresetRecorderAndApplier.ApplyPreset(displayPreset);
        }
    }
}
