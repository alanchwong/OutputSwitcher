using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcherConsole.WinAPI;

namespace OutputSwitcherConsole.Data
{
    [Serializable]
    public class DisplayPreset
    {
        /// <summary>
        /// Creates an empty DisplayPreset. Required for serialization.
        /// </summary>
        public DisplayPreset()
        {

        }

        public DisplayPreset(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Unique user-defined name to identify this set of display settings.
        /// </summary>
        public string Name { get; set; }

        public CCD.DisplayConfigPathInfo[] PathInfoArray
        {
            get; set;
        }
                 
        public CCD.DisplayConfigModeInfo[] ModeInfoArray
        {
            get; set;
        }
    }
}
