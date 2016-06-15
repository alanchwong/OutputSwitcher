using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcherConsole
{
    class ConsoleCommandParser
    {
        /// <summary>
        /// Probably unnecessary enum for commands?
        /// </summary>
        private enum CommandAction : uint
        {
            Exit,
            ShowDisplayDevicesToConsole,
            ShowDisplayAdapterSettingsToConsole,
            TestSwapPrimaryAndASecondaryDisplay,
            TestCaptureCurrentConfigurationAndWriteToFile,
            ListPresets,
            DeletePreset,
            CaptureCurrentConfigAndSaveAsPreset,
            ApplyPreset,
            TestAttachTV,
            Unknown,
        }

        /// <summary>
        /// Internal struct used to mate a command action enum value with a string of descriptive text and a function that executes that action.
        /// </summary>
        private struct CommandActionWithBlurbAndAction
        {
            public CommandActionWithBlurbAndAction(CommandAction commandAction, string blurb, Action func)
            {
                this.CommandAction = commandAction;
                this.Blurb = blurb;
                this.CommandExecutor = func;
            }

            public CommandAction CommandAction;
            public string Blurb;
            public Action CommandExecutor;
        }

        /// <summary>
        /// List of commands available through the console
        /// </summary>
        private static readonly CommandActionWithBlurbAndAction[] CommandList = {
            new CommandActionWithBlurbAndAction(CommandAction.Exit, "Exit.", ConsoleCommands.Exit),
            new CommandActionWithBlurbAndAction(CommandAction.ShowDisplayDevicesToConsole, "Enumerate all display devices.", ConsoleCommands.ShowDisplayDevicesToConsole),
            new CommandActionWithBlurbAndAction(CommandAction.ShowDisplayAdapterSettingsToConsole, "Enumerate display adapters' settings.", ConsoleCommands.ShowDisplayAdapterDeviceSettings),
            new CommandActionWithBlurbAndAction(CommandAction.TestSwapPrimaryAndASecondaryDisplay, "(Test) Swap Primary and a Secondary Display", ConsoleCommands.TestSwapPrimaryAndASecondaryDisplay),
            new CommandActionWithBlurbAndAction(CommandAction.TestCaptureCurrentConfigurationAndWriteToFile, "(Test) Capture current configuration and write to file.", ConsoleCommands.TestCaptureCurrentConfigurationAndWriteToFile),
            new CommandActionWithBlurbAndAction(CommandAction.ListPresets, "List all saved presets.", ConsoleCommands.ListPresets),
            new CommandActionWithBlurbAndAction(CommandAction.DeletePreset, "Delete a saved preset.", ConsoleCommands.DeletePreset),
            new CommandActionWithBlurbAndAction(CommandAction.CaptureCurrentConfigAndSaveAsPreset, "Capture current display configuration and save as preset.", ConsoleCommands.CaptureCurrentConfigAndSaveAsPreset),
            new CommandActionWithBlurbAndAction(CommandAction.ApplyPreset, "Apply a saved preset.", ConsoleCommands.ApplyPreset),
            new CommandActionWithBlurbAndAction(CommandAction.TestAttachTV, "(Test) Attach TV.", ConsoleCommands.TestAttachTV),
        };

        /// <summary>
        /// Main program loop to be called from Program.Main. Prompts user for command input until Exit command is issued.
        /// </summary>
        static public void MainLoop()
        {
            do
            {
                Console.WriteLine("Select action:");

                for (int i = 1; i < CommandList.Length; i++)
                {
                    Console.WriteLine("[" + i + "] " + CommandList[i].Blurb);
                }

                Console.WriteLine("[0] " + CommandList[0].Blurb);
                Console.Write("\nAction: ");

                uint commandIndex;
                if ((UInt32.TryParse(Console.ReadLine(), out commandIndex)) && (commandIndex < CommandList.Length))
                {
                    CommandList[commandIndex].CommandExecutor();
                }
                else
                {
                    Console.WriteLine("Unknown action.");
                }

            } while (true); // Need to invoke the Exit command to call ConsoleCommands.Exit() to terminate program.
        }

    }
}
