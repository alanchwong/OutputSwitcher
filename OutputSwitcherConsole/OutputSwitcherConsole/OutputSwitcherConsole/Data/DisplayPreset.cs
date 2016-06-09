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
        public List<DisplayDeviceSettings> DisplayDeviceSettings
        {
            get
            {
                if (mDisplaySettings != null)
                {
                    // Return a copy of the list so as not to change the stored settings in this instance.
                    // NOTE: Not a fan of this, maybe accept a list or array as input, and return
                    // a copied array. If someone just wanted to use this as a property then we keep
                    // making a ton of copies every time its referenced even when it appears that it's using
                    // the same reference.
                    return new List<DisplayDeviceSettings>(mDisplaySettings);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                // Create a copy of the supplied list to store in this instance.
                if (value != null)
                {
                    mDisplaySettings = new List<DisplayDeviceSettings>(value);
                }
                else
                {
                    mDisplaySettings = null;
                }
            }
        }

        private IList<DisplayDeviceSettings> mDisplaySettings = null;
    }
}
