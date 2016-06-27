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
                mIsDirty = true;
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
                mIsDirty = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes current collection of display presets to disk if changes have been made.
        /// </summary>
        public void PersistDisplayPresetsIfDirty()
        {
            if (mIsDirty)
            {
                DisplayPersistence.WriteSettings(this.GetPresets());
            }
        }

        /// <summary>
        /// Retrieves a collection containing all the display presets.
        /// </summary>
        /// <returns>A list of DisplayPreset objects.</returns>
        public List<DisplayPreset> GetPresets()
        {
            List<DisplayPreset> listOfPresets = new List<DisplayPreset>(mDisplayPresetDictionary.Values);
            return listOfPresets;
        }

        /// <summary>
        /// Retrieves the DisplayPreset with a specific name.
        /// </summary>
        /// <param name="name">Name of the preset.</param>
        /// <returns>The DisplayPreset with the supplied name, null otherwise.</returns>
        public DisplayPreset GetPreset(string name)
        {
            DisplayPreset requestedPreset = null;

            if (mDisplayPresetDictionary.TryGetValue(name, out requestedPreset))
                return requestedPreset;
            else
                return null;
        }

        /// <summary>
        /// Constructor for the DisplayPresetCollection singleton. Initializes the
        /// collection of presets with presets saved in the configuration file.
        /// </summary>
        private DisplayPresetCollection()
        {
            mDisplayPresetDictionary = new Dictionary<string, DisplayPreset>(StringComparer.CurrentCultureIgnoreCase);

            // RAII baby, load it up
            List<DisplayPreset> displayPresets = DisplayPersistence.LoadSettings();

            if (displayPresets != null)
            {
                foreach (DisplayPreset preset in displayPresets)
                {
                    mDisplayPresetDictionary.Add(preset.Name, preset);
                }
            }
        }

        private static DisplayPresetCollection mInstance = null;

        // TODO: This might actually be overkill for our purposes.
        // Honestly, how many presets will a user have? A handful.
        private IDictionary<string, DisplayPreset> mDisplayPresetDictionary;

        /// <summary>
        /// Indicates whether something has changed in the collection that would neccessitate re-writing
        /// the presets to disk.
        /// </summary>
        private bool mIsDirty = false;
    }
}
