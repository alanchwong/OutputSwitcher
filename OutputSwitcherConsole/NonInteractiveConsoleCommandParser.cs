using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcher.ConsoleApp
{
    class NonInteractiveConsoleCommandParser
    {
        enum CommandAction
        {
            Help,
            CaptureCurrentConfigurationAsPreset,
            ApplyPreset,
            UnknownCommand
        }

        struct CommandActionWithStringAndAction
        {
            public CommandActionWithStringAndAction(CommandAction commandAction, string commandLineArgument, Action<string[]> action)
            {
                this.commandAction = commandAction;
                this.commandLineArgument = commandLineArgument;
                this.action = action;
            }

            public CommandAction commandAction;
            public string commandLineArgument;
            public Action<string[]> action;
        }

        private static readonly CommandActionWithStringAndAction[] commands =
        {
            new CommandActionWithStringAndAction(CommandAction.Help, "Help", NonInteractiveConsoleCommands.DisplayHelp),
            new CommandActionWithStringAndAction(CommandAction.CaptureCurrentConfigurationAsPreset, "Capture", NonInteractiveConsoleCommands.CaptureCurrentConfigAndSaveAsPreset),
            new CommandActionWithStringAndAction(CommandAction.ApplyPreset, "Apply", NonInteractiveConsoleCommands.ApplyPreset),
        };

        public static void ParseCommandLineArgsAndExecute(string[] args)
        {
            string enteredCommand = args[0];
            bool validCommand = false;

            foreach (CommandActionWithStringAndAction command in commands)
            {
                if (enteredCommand.Equals(command.commandLineArgument, StringComparison.InvariantCultureIgnoreCase))
                {
                    validCommand = true;
                    command.action(args);
                    break;
                }
            }

            if (!validCommand)
            {
                NonInteractiveConsoleCommands.DisplayHelp(args);
            }
        }
    }
}
