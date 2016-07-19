using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // If no command arguments supplied, enter interactive mode.
            if (args.Length == 0)
            {
                ConsoleCommandParser.MainLoop();
            }
            else
            {
                NonInteractiveConsoleCommandParser.ParseCommandLineArgsAndExecute(args);
            }
        }
    }
}
