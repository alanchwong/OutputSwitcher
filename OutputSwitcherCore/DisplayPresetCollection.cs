using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcher.Core
{
    public class DisplayPresetCollection
    {
        // Responsibilities
        // 1. Load settings from disk into memory
        // 2. Save settings from memory to disk
        // 3. Add new presets
        // 4. Remove existing presets
        // 5. Provide access to the presets in a form that can be consumed by UI

        /// <summary>
        /// Enumeration indicating the type of change made to the display preset collection.
        /// </summary>
        public enum DisplayPresetCollectionChangeType
        {
            PresetAdded,
            PresetRemoved,
        }

        /// <summary>
        /// Event handler delegate for the DisplayPresetCollectionChanged event.
        /// </summary>
        /// <param name="changeType">The type of change in the collection.</param>
        /// <param name="presetName">The name of the preset participating in the change.</param>
        public delegate void DisplayPresetCollectionChangedEventHandler(DisplayPresetCollectionChangeType changeType, string presetName);

        /// <summary>
        /// Event fired when display preset collection changed.
        /// </summary>
        public event DisplayPresetCollectionChangedEventHandler DisplayPresetCollectionChanged;

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


                DisplayPresetCollectionChanged(DisplayPresetCollectionChangeType.PresetAdded, preset.Name);

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

                DisplayPresetCollectionChanged(DisplayPresetCollectionChangeType.PresetRemoved, presetName);

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
                DisplayPersistence.WritePresets(this.GetPresets());
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
            List<DisplayPreset> displayPresets = DisplayPersistence.LoadPresets();

            if (displayPresets != null)
            {
                foreach (DisplayPreset preset in displayPresets)
                {
                    mDisplayPresetDictionary.Add(preset.Name, preset);
                }
            }

            DisplayPresetCollectionChanged += OnDisplayPresetCollectionChanged;
        }

        /// <summary>
        /// Persists the collection if necessary when the singleton instance is marked for destruction.
        /// </summary>
        ~DisplayPresetCollection()
        {
            PersistDisplayPresetsIfDirty();
        }

        /// <summary>
        /// Internal event handler for handling display preset changes in the in-memory collection. Persists
        /// changes to disk.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="pn"></param>
        private void OnDisplayPresetCollectionChanged(DisplayPresetCollectionChangeType c, string pn)
        {
            PersistDisplayPresetsIfDirty();
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
