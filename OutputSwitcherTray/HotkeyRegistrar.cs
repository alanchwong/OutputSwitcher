using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcher.WinAPI;

namespace OutputSwitcher.TrayApp
{
    /// <summary>
    /// Class to register and deregister hotkeys based on the preset to hotkey map.
    /// Responsibilities
    /// 1. Listens to changes in the preset hotkey mapping to register and deregister
    ///     global hotkeys.
    /// 2. Does initial registration of global hotkeys when initialized.
    /// </summary>
    internal class HotkeyRegistrar
    {
        /// <summary>
        /// Gets the singleton instance of the HotkeyRegistrar. InitializeHotkeyRegistrar
        /// must have been called for this to not return null.
        /// </summary>
        /// <returns>Returns the singleton instance of the HotkeyRegistrar.</returns>
        public static HotkeyRegistrar GetInstance()
        {
            return mInstance;
        }

        /// <summary>
        /// Initializes the singleton instance of HotkeyRegistrar.
        /// </summary>
        /// <param name="presetToHotkeyMap">The mapping of presets to hotkeys.</param>
        /// <param name="messageFilter"></param>
        public static void Initialize(PresetToHotkeyMap presetToHotkeyMap)
        {
            if (mInstance == null)
            {
                mInstance = new HotkeyRegistrar(presetToHotkeyMap);
            }
            else
            {
                throw new Exception("Unexpected reinitialization of HotkeyRegistrar.");
            }
        }

        /// <summary>
        /// Event handler for when a display preset has a hotkey set or cleared. Registers
        /// new hotkey and deregisters old hotkeys as appropriate.
        /// </summary>
        /// <param name="presetName">The name of the preset changed.</param>
        /// <param name="virtualHotkey">The new hotkey if applicable.</param>
        /// <param name="changeType">The type of change, either a hotkey has been set or removed.</param>
        private void OnPresetHotkeyMapChanged(string presetName, VirtualHotkey virtualHotkey, PresetToHotkeyMap.PresetToHotkeyMappingChangeType changeType)
        {
            if (changeType == PresetToHotkeyMap.PresetToHotkeyMappingChangeType.HotkeyRemoved)
            {
                int existingHotkeyId;
                if (mPresetToHotkeyId.TryGetValue(presetName, out existingHotkeyId))
                    DeregisterHotkeyAndUpdateIdMapping(presetName, existingHotkeyId);
                else
                    throw new Exception("Tried to deregister hotkey for unknown preset: " + presetName);
            }
            else if (changeType == PresetToHotkeyMap.PresetToHotkeyMappingChangeType.HotkeySet)
            {
                // If a hotkey already exists for this, we need to deregister the old one
                // and register the new one.
                int existingHotkeyId;
                if (mPresetToHotkeyId.TryGetValue(presetName, out existingHotkeyId))
                    DeregisterHotkeyAndUpdateIdMapping(presetName, existingHotkeyId);

                RegisterHotkeyAndUpdateIdMapping(presetName, virtualHotkey);
            }
        }

        /// <summary>
        /// Private constructor. Initializes the HotkeyRegistrar with the supplied preset to hotkey map
        /// instance. Performs initial registration of hotkeys.
        /// </summary>
        /// <param name="presetToHotkeyMap"></param>
        private HotkeyRegistrar(PresetToHotkeyMap presetToHotkeyMap)
        {
            // Register all existing hotkeys
            mPresetToHotkeyMap = presetToHotkeyMap;
            mPresetToHotkeyId = new Dictionary<string, int>();
            mPresetToHotkeyMap.OnPresetToHotkeyMappingChanged += OnPresetHotkeyMapChanged;
            foreach (KeyValuePair<string, VirtualHotkey> presetAndHotkey in mPresetToHotkeyMap.GetHotkeyMappings())
            {
                RegisterHotkeyAndUpdateIdMapping(presetAndHotkey.Key, presetAndHotkey.Value);
            }
        }

        /// <summary>
        /// Destructor. Unregisters all currently registered hotkeys.
        /// </summary>
        ~HotkeyRegistrar()
        {
            foreach(int hotkeyId in mPresetToHotkeyId.Values)
            {
                Hotkey.UnregisterHotKey(IntPtr.Zero, hotkeyId);
            }
        }

        /// <summary>
        /// Registers a global hotkey through Windows API and associates the ID of the hotkey with the
        /// name of the display preset it is intended to trigger.
        /// </summary>
        /// <param name="presetName">The name of the preset to register a hotkey for.</param>
        /// <param name="virtualHotkey">The keycode and modifier flags of the hotkey.</param>
        private void RegisterHotkeyAndUpdateIdMapping(string presetName, VirtualHotkey virtualHotkey)
        {
            int newHotkeyId = (int)DateTime.Now.ToFileTimeUtc();

            bool result = Hotkey.RegisterHotKey(
                IntPtr.Zero,
                newHotkeyId,
                virtualHotkey.ModifierKeyCode | VirtualKeyCodes.MOD_NOREPEAT,
                virtualHotkey.KeyCode);

            if (!result)
                throw new Exception("Failed to register new hotkey for preset " + presetName +
                                    " with ID " + newHotkeyId +
                                    " with modifiers keycode " + virtualHotkey.ModifierKeyCode +
                                    " with keycode " + virtualHotkey.KeyCode);

            mPresetToHotkeyId.Add(presetName, newHotkeyId);
        }

        /// <summary>
        /// Deregisters a global hotkey through the Windows API using an existing hotkey ID. Deassociates
        /// that hotkey ID with the preset it is intended to trigger.
        /// </summary>
        /// <param name="presetName">The name of the preset associated with the hotkey to be removed.</param>
        /// <param name="existingHotkeyId">The ID of the hotkey to deregister.</param>
        private void DeregisterHotkeyAndUpdateIdMapping(string presetName, int existingHotkeyId)
        {
            bool result = Hotkey.UnregisterHotKey(IntPtr.Zero, existingHotkeyId);

            if (!result)
                throw new Exception("Failed to deregister hotkey for preset " + presetName + ", with ID: " + existingHotkeyId);

            mPresetToHotkeyId.Remove(presetName);
        }

        private static HotkeyRegistrar mInstance;

        private Dictionary<string, int> mPresetToHotkeyId;
        private PresetToHotkeyMap mPresetToHotkeyMap;
    }
}