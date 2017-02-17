using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutputSwitcher.Core;

namespace OutputSwitcher.TrayApp
{
    class PresetToHotkeyMap
    {
        public static PresetToHotkeyMap GetInstance()
        {
            if (mInstance == null)
                mInstance = new PresetToHotkeyMap();

            return mInstance;
        }

        public enum PresetToHotkeyMappingChangeType
        {
            HotkeySet,
            HotkeyRemoved
        }

        /// <summary>
        /// Event handler for a change in the preset to hotkey mappings.
        /// </summary>
        /// <param name="presetName">Name of the preset with a hotkey mapping.</param>
        /// <param name="virtualHotkey">The virtual key code of the hotkey with modifiers.</param>
        /// <param name="changeType">The type of change to the mapping.</param>
        public delegate void PresetToHotkeyMappingChangedHandler(string presetName, VirtualHotkey virtualHotkey, PresetToHotkeyMappingChangeType changeType);

        /// <summary>
        /// Event raised when there is a change in the preset to hotkey mappings.
        /// </summary>
        public event PresetToHotkeyMappingChangedHandler OnPresetToHotkeyMappingChanged;

        /// <summary>
        /// Set the global hotkey of a display preset.
        /// </summary>
        /// <param name="presetName">The display preset name.</param>
        /// <param name="virtualHotkey">The virtual key code of the global hotkey with modifiers.</param>
        public void SetHotkey(string presetName, VirtualHotkey virtualHotkey)
        {
            // Use the Item property to set the value because if it already exists
            // we want to overwrite it.
            mPresetToHotkeyDictionary[presetName] = virtualHotkey;
            SaveMapping();
            OnPresetToHotkeyMappingChanged?.Invoke(presetName, virtualHotkey, PresetToHotkeyMappingChangeType.HotkeySet);
        }

        /// <summary>
        /// Removes the global hotkey entry for a given display preset name.
        /// </summary>
        /// <param name="presetName">The preset name whose hotkey mapping to remove.</param>
        public void RemoveHotkey(string presetName)
        {
            mPresetToHotkeyDictionary.Remove(presetName);
            SaveMapping();
            OnPresetToHotkeyMappingChanged?.Invoke(presetName, new VirtualHotkey(0, 0), PresetToHotkeyMappingChangeType.HotkeyRemoved);
        }

        /// <summary>
        /// Returns a dictionary mapping DisplayPreset names to the hotkeys used to trigger them.
        /// </summary>
        /// <returns>Dictionary mapping DisplayPreset names to VirtualHotkey.</returns>
        public Dictionary<string, VirtualHotkey> GetHotkeyMappings()
        {
            // Return a copy.
            return new Dictionary<string, VirtualHotkey>(mPresetToHotkeyDictionary);
        }

        /// <summary>
        /// Persists the current mapping to disk.
        /// </summary>
        private void SaveMapping()
        {
            PresetToHotkeyMapPersistence.SavePresetToHotkeysMap(mPresetToHotkeyDictionary);
        }

        /// <summary>
        /// Handles the event when the display preset collection has changed. If a preset has been
        /// deleted, the corresponding hotkey (if existing) should be deleted as well.
        /// </summary>
        /// <param name="changeType">The type of collection change.</param>
        /// <param name="presetName">The name of the changed preset.</param>
        private void OnDisplayPresetCollectionChanged(DisplayPresetCollection.DisplayPresetCollectionChangeType changeType, string presetName)
        {
            // Really it seems a better idea to just unify all the persistence into one data structure and
            // collection rather than trying to keep two collections in sync.

            if (changeType == DisplayPresetCollection.DisplayPresetCollectionChangeType.PresetRemoved && 
                mPresetToHotkeyDictionary.ContainsKey(presetName))
            {
                RemoveHotkey(presetName);
            }
        }

        private PresetToHotkeyMap()
        {
            Dictionary<string, VirtualHotkey> dictionary = PresetToHotkeyMapPersistence.LoadPresetToHotkeysMap();

            mPresetToHotkeyDictionary = dictionary != null ? new Dictionary<string, VirtualHotkey>(dictionary) : new Dictionary<string, VirtualHotkey>();

            DisplayPresetCollection.GetDisplayPresetCollection().DisplayPresetCollectionChanged += OnDisplayPresetCollectionChanged;
        }

        private static PresetToHotkeyMap mInstance;

        private Dictionary<string, VirtualHotkey> mPresetToHotkeyDictionary;
    }
}
