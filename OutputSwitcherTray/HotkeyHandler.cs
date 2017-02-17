using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcher.Core;
using OutputSwitcher.WinAPI;

namespace OutputSwitcher.TrayApp
{
    /// <summary>
    /// Handles global hotkey events that are intended for OutputSwitcher. Finds
    /// the correct display preset to apply for a given global hotkey and applies it.
    /// </summary>
    internal class HotkeyHandler
    {
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        /// <returns>Null if HotkeyHandler.Initialize has not been called, the singleton instance otherwise.</returns>
        public static HotkeyHandler GetInstance()
        {
            return mInstance;
        }

        /// <summary>
        /// Initializes the singleton instance of HotkeyHandler with the supplied
        /// preset to hotkey mappings, and the message filter that receives global
        /// hotkey events.
        /// </summary>
        /// <param name="presetToHotkeyMap">Mapping of presets to hotkeys.</param>
        /// <param name="hotkeyMessageFilter">Message filter that notifies subscribers when a global hotkey for this app has occurred.</param>
        public static void Initialize(PresetToHotkeyMap presetToHotkeyMap, HotkeyMessageFilter hotkeyMessageFilter)
        {
            if (mInstance == null)
                mInstance = new HotkeyHandler(presetToHotkeyMap, hotkeyMessageFilter);
            else
                throw new Exception("Unexpected reiniitalization of Hotkey Handler");
        }

        /// <summary>
        /// Private constructor. Initializes the HotkeyHandler with the supplied preset to
        /// hotkey mapping and message filter.
        /// </summary>
        /// <param name="presetToHotkeyMap">Mapping of presets to hotkeys.</param>
        /// <param name="hotkeyMessageFilter">Message filter that notifies subscribers when a global hotkey for this app has occurred.</param>
        private HotkeyHandler(PresetToHotkeyMap presetToHotkeyMap, HotkeyMessageFilter hotkeyMessageFilter)
        {
            mPresetToHotkeyMap = presetToHotkeyMap;

            mHotkeyMessageFilter = hotkeyMessageFilter;
            mHotkeyMessageFilter.PresetSwitchHotkeyPressed += OnPresetSwitchHotkeyPressed;
        }

        /// <summary>
        /// Event handler when a global hotkey event for this app has been raised. Finds the
        /// preset that corresponds to the hotkey pressed and applies it.
        /// </summary>
        /// <param name="modifierKeyFlags">Modifier keys for the hotkey.</param>
        /// <param name="keycode">Key code for the hotkey.</param>
        private void OnPresetSwitchHotkeyPressed(ushort modifierKeyFlags, ushort keycode)
        {
            // TODO: This kinda sucks because it sorta relies on abusing the interface of PresetToHotkeyMap.
            foreach (KeyValuePair<string, VirtualHotkey> presetHotkeyPair in mPresetToHotkeyMap.GetHotkeyMappings().ToArray())
            {
                // Chop off the MOD_NOREPEAT that is added by HotkeyRegistrar.
                ushort presetModifierKeyFlags = 
                    unchecked((ushort)(presetHotkeyPair.Value.ModifierKeyCode &
                                        (VirtualKeyCodes.MOD_ALT | 
                                            VirtualKeyCodes.MOD_CONTROL | 
                                            VirtualKeyCodes.MOD_SHIFT | 
                                            VirtualKeyCodes.MOD_WIN)));

                ushort presetKeyCode = (unchecked((ushort)(presetHotkeyPair.Value.KeyCode)));

                if (presetModifierKeyFlags == modifierKeyFlags && presetKeyCode == keycode)
                {
                    DisplayPreset invokedPreset = DisplayPresetCollection.GetDisplayPresetCollection().GetPreset(presetHotkeyPair.Key);

                    if (invokedPreset != null)
                        SafePresetApplier.ApplyPresetWithRevertCountdown(invokedPreset);
                    else
                        throw new Exception("Hotkey pressed for unknown preset.");

                    break;
                }
            }

        }

        private static HotkeyHandler mInstance;

        private PresetToHotkeyMap mPresetToHotkeyMap;
        private HotkeyMessageFilter mHotkeyMessageFilter;
    }
}
