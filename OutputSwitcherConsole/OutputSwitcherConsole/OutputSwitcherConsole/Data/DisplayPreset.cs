using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// The set of DisplayDeviceSetting objects that describe a preset display configuration.
        /// </summary>
        public List<DisplayDeviceSettings> DisplaySettings
        {
            get { return mDisplaySettings; }
            set { mDisplaySettings = value; }
        }

        private List<DisplayDeviceSettings> mDisplaySettings = null;
    }
}
