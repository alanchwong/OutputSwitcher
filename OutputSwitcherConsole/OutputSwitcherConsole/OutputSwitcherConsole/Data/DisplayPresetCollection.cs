using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcherConsole.Data
{
    class DisplayPresetCollection
    {
        // Responsibilities
        // 1. Load settings from disk into memory
        // 2. Save settings from memory to disk
        // 3. Add new presets
        // 4. Remove existing presets
        // 5. Provide access to the presets in a form that can be consumed by UI

        public static DisplayPresetCollection GetDisplayPresetCollection()
        {
            if (mInstance == null)
            {
                mInstance = new DisplayPresetCollection();
            }

            return mInstance;
        }

        /// <summary>
        /// Adds a new DisplayPreset to the collection of display configuration presets.
        /// </summary>
        /// <param name="preset">The new DisplayPreset to add.</param>
        /// <returns>True if the DisplayPreset.Name is unique in the collection of presets. False otherwise.</returns>
        public bool TryAddDisplayPreset(DisplayPreset preset)
        {
            if (!mDisplayPresetDictionary.ContainsKey(preset.Name))
            {
                mDisplayPresetDictionary.Add(preset.Name, preset);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the DisplayPreset from the collection with the given name.
        /// </summary>
        /// <param name="presetName">Name of the preset to remove.</param>
        /// <returns>True if preset found and removed, false otherwise.</returns>
        public bool TryRemoveDisplayPreset(string presetName)
        {
            if (mDisplayPresetDictionary.ContainsKey(presetName))
            {
                mDisplayPresetDictionary.Remove(presetName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes current collection of display presets to disk.
        /// </summary>
        public void PersistDisplayPresets()
        {
            DisplayDeviceSettingsPersistence.WriteSettings(this.GetPresets());
        }

        /// <summary>
        /// Retrieves an array containing all the display presets.
        /// </summary>
        /// <returns>An array of DisplayPreset objects.</returns>
        public List<DisplayPreset> GetPresets()
        {
            List<DisplayPreset> listOfPresets = new List<DisplayPreset>(mDisplayPresetDictionary.Values);
            return listOfPresets;
        }

        private DisplayPresetCollection()
        {
            // RAII baby, load it up
            DisplayDeviceSettingsPersistence.LoadSettings();
            mDisplayPresetDictionary = new Dictionary<string, DisplayPreset>(); // TODO: Place loaded items into this. Can we just write the dictionary as is?
        }

        private static DisplayPresetCollection mInstance = null;

        // TODO: This might actually be overkill for our purposes.
        // Honestly, how many presets will a user have? A handful.
        private IDictionary<string, DisplayPreset> mDisplayPresetDictionary;
    }
}
