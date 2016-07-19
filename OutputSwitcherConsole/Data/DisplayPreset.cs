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
        public DisplayPreset() {}

        public DisplayPreset(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Unique user-defined name to identify this set of display settings.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The array of active paths for this preset display configuration.
        /// </summary>
        public CCD.DisplayConfigPathInfo[] PathInfoArray
        {
            get; set;
        }

        /// <summary>
        /// The array of source and target modes for the active paths.
        /// </summary>
        public CCD.DisplayConfigModeInfo[] ModeInfoArray
        {
            get; set;
        }

        /// <summary>
        /// The device names of the display targets in this preset's active paths.
        /// This is not used for applying presets, but to give extra context to the
        /// persisted preset info for human readability.
        /// </summary>
        public CCD.DisplayConfigTargetDeviceName[] TargetDeviceNames
        {
            get; set;
        }
    }
}
