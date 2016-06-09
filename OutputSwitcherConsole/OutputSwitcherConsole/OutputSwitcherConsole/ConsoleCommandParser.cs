using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcherConsole
{
    class ConsoleCommandParser
    {
        enum CommandAction : uint
        {
            Exit = 0,
            ShowDisplayDevicesToConsole = 1,
            ShowDisplayAdapterSettingsToConsole = 2,
            TestSwapPrimaryAndASecondaryDisplay = 3,
            TestCaptureCurrentConfigurationAndWriteToFile = 4,
            Unknown = 5,
        }

        static public void MainLoop()
        {
            CommandAction commandAction;

            do
            {
                Console.WriteLine("Select action:");
                Console.WriteLine("[1] Show all display devices.");
                Console.WriteLine("[2] Show all display adapters' settings.");
                Console.WriteLine("[3] (Test) Swap Primary and a Secondary display.");
                Console.WriteLine("[4] (Test) Capture current configuration and write to file.");
                Console.WriteLine("[0] Exit.");
                Console.Write("\nAction: ");

                commandAction = ParseUserActionInput(Console.ReadLine());

                switch (commandAction)
                {
                    case CommandAction.Exit:
                        break;
                    case CommandAction.ShowDisplayDevicesToConsole:
                        ConsoleCommands.ShowDisplayDevicesToConsole();
                        break;
                    case CommandAction.ShowDisplayAdapterSettingsToConsole:
                        ConsoleCommands.ShowDisplayAdapterDeviceSettings();
                        break;
                    case CommandAction.TestSwapPrimaryAndASecondaryDisplay:
                        ConsoleCommands.TestSwapPrimaryAndASecondaryDisplay();
                        break;
                    case CommandAction.TestCaptureCurrentConfigurationAndWriteToFile:
                        ConsoleCommands.TestCaptureCurrentConfigurationAndWriteToFile();
                        break;
                    default:
                        Console.WriteLine("Unknown action.");
                        break;
                }

            } while (commandAction != CommandAction.Exit);
        }


        static CommandAction ParseUserActionInput(string action)
        {
            if (action.Length == 1)
            {
                uint parseResult;
                if (UInt32.TryParse(action, out parseResult))
                {
                    if (parseResult < (uint)(CommandAction.Unknown))
                    {
                        return (CommandAction)parseResult;
                    }
                }
            }

            return CommandAction.Unknown;
        }
    }
}
