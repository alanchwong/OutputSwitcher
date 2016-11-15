using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OutputSwitcher.Core;
using OutputSwitcher.WinAPI;

namespace OutputSwitcher.TrayApp
{
    partial class PresetHotkeyForm : Form
    {
        /* NOTES
         *  There are a lot of crunchy synchronization issues this whole design introduces.
         *  By allowing for the hotkey mappings to live separately from the display presets
         *  collection, we add complexity in having to sync between those two collections.
         *  What do we do if there are presets added while this is active?
         */

        /// <summary>
        /// Structure to hold the last user-entered new hotkey combination.
        /// </summary>
        private struct NewHotkey
        {
            public Keys NewHotkeyModifiers;
            public Keys NewHotkeyKeyCode;
            public bool NewHotkeyIsValid;
        }

        public PresetHotkeyForm(DisplayPresetCollection displayPresetCollection, PresetToHotkeyMap presetToHotkeyMap)
        {
            InitializeComponent();

            mNewHotkey = new NewHotkey();

            mPresetToHotkeyMap = presetToHotkeyMap;
            mPresetToHotkeyDictionaryForComboBox = new Dictionary<string, VirtualHotkey>(mPresetToHotkeyMap.GetHotkeyMappings());
            mPresetToHotkeyMap.OnPresetToHotkeyMappingChanged += OnPresetToHotkeyMapChanged;

            presetsComboBox.Items.AddRange(displayPresetCollection.GetPresets().ToArray());
            presetsComboBox.SelectedIndexChanged += PresetsComboBox_SelectedIndexChanged;
            if (presetsComboBox.Items.Count > 0)
                presetsComboBox.SelectedIndex = 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            mPresetToHotkeyMap.OnPresetToHotkeyMappingChanged -= OnPresetToHotkeyMapChanged;

            base.OnClosed(e);
        }

        private void ClearNewShortcutKey()
        {
            mNewHotkey.NewHotkeyIsValid = false;
        }

        private void ShowSelectedPresetKeyCombination(string preset)
        {
            VirtualHotkey keycode;

            if (mPresetToHotkeyDictionaryForComboBox.TryGetValue(preset, out keycode))
            {
                // TODO: Do some magic to make it look okay
                StringBuilder pleasantHotkeyStringBuilder = new StringBuilder();

                if (VirtualKeyCodes.KeyCodeHasCtrl(keycode.ModifierKeyCode))
                    pleasantHotkeyStringBuilder.Append("CTRL + ");

                if (VirtualKeyCodes.KeyCodeHasAlt(keycode.ModifierKeyCode))
                    pleasantHotkeyStringBuilder.Append("ALT + ");

                if (VirtualKeyCodes.KeyCodeHasShift(keycode.ModifierKeyCode))
                    pleasantHotkeyStringBuilder.Append("SHIFT + ");

                pleasantHotkeyStringBuilder.Append(VirtualKeyCodes.GetFriendlyKeyCodeName(keycode.KeyCode));

                selectedPresetHotkeyTextbox.Text = pleasantHotkeyStringBuilder.ToString();
                newHotkeyTextBox.Text = "";
            }
            else
            {
                selectedPresetHotkeyTextbox.Text = "";
                newHotkeyTextBox.Text = "";
            }
        }

        private void OnPresetToHotkeyMapChanged(string presetName, VirtualHotkey keyCode, PresetToHotkeyMap.PresetToHotkeyMappingChangeType changeType)
        {
            // Dumbly update the entire working dictionary of preset to hotkey mappings.
            mPresetToHotkeyDictionaryForComboBox = mPresetToHotkeyMap.GetHotkeyMappings();
            ShowSelectedPresetKeyCombination(((DisplayPreset)presetsComboBox.SelectedItem).Name);
        }

        private void PresetsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedPresetKeyCombination(((DisplayPreset)presetsComboBox.SelectedItem).Name);
            ClearNewShortcutKey();
        }

        private void newHotkeyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            StringBuilder outputString = new StringBuilder();

            bool validShortcutKeyPressed = false;

            if ((e.Modifiers & Keys.Control) > 0)
                outputString.Append("CTRL + ");

            if ((e.Modifiers & Keys.Alt) > 0)
                outputString.Append("ALT + ");

            if ((e.Modifiers & Keys.Shift) > 0)
                outputString.Append("SHIFT + ");

            // The keycode property will always be the key pressed unless it's only a modifier key
            if (e.KeyCode != Keys.Alt &&
                e.KeyCode != Keys.Control &&
                e.KeyCode != Keys.Shift &&
                e.KeyCode != Keys.ControlKey &&
                e.KeyCode != Keys.RShiftKey &&
                e.KeyCode != Keys.LShiftKey &&
                e.KeyCode != Keys.ShiftKey &&
                e.KeyCode != Keys.Menu &&
                e.KeyCode != Keys.LMenu &&
                e.KeyCode != Keys.RMenu &&
                e.KeyCode != Keys.LWin &&
                e.KeyCode != Keys.RWin &&
                e.KeyCode != Keys.Return &&
                e.KeyCode != Keys.Capital &&
                e.KeyCode != Keys.CapsLock)
            {
                validShortcutKeyPressed = true;
                outputString.Append(e.KeyCode);
            }

            mNewHotkey.NewHotkeyKeyCode = e.KeyCode;
            mNewHotkey.NewHotkeyModifiers = e.Modifiers;
            mNewHotkey.NewHotkeyIsValid = validShortcutKeyPressed;

            newHotkeyTextBox.Text = outputString.ToString();

            e.Handled = true;
        }

        private void setHotkeyButton_Click(object sender, EventArgs e)
        {
            if (presetsComboBox.SelectedItem != null && mNewHotkey.NewHotkeyIsValid)
            {
                uint modifierKeyCode = 0;

                if ((mNewHotkey.NewHotkeyModifiers & Keys.Alt) > 0)
                    modifierKeyCode = modifierKeyCode | VirtualKeyCodes.MOD_ALT;

                if ((mNewHotkey.NewHotkeyModifiers & Keys.Control) > 0)
                    modifierKeyCode = modifierKeyCode | VirtualKeyCodes.MOD_CONTROL;

                if ((mNewHotkey.NewHotkeyModifiers & Keys.Shift) > 0)
                    modifierKeyCode = modifierKeyCode | VirtualKeyCodes.MOD_SHIFT;

                uint keypressKeyCode = (uint)mNewHotkey.NewHotkeyKeyCode;

                VirtualHotkey virtualKeyCode = new VirtualHotkey(modifierKeyCode, keypressKeyCode);
                mPresetToHotkeyMap.SetHotkey(((DisplayPreset)presetsComboBox.SelectedItem).Name, virtualKeyCode);
            }
        }

        private void clearHotkeyButton_Click(object sender, EventArgs e)
        {
            if (presetsComboBox.SelectedItem != null)
                mPresetToHotkeyMap.RemoveHotkey(((DisplayPreset)presetsComboBox.SelectedItem).Name);
        }

        private PresetToHotkeyMap mPresetToHotkeyMap;
        private Dictionary<string, VirtualHotkey> mPresetToHotkeyDictionaryForComboBox;
        private NewHotkey mNewHotkey;
    }
}
