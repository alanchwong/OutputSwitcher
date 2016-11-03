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
            mPresetToHotkeyDictionaryForComboBox = new Dictionary<string, uint>(mPresetToHotkeyMap.GetHotkeyMappings());
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
            uint keycode;

            if (mPresetToHotkeyDictionaryForComboBox.TryGetValue(preset, out keycode))
            {
                // TODO: Do some magic to make it look okay
                selectedPresetHotkeyTextbox.Text = keycode.ToString();
                newHotkeyTextBox.Text = "";
            }
            else
            {
                selectedPresetHotkeyTextbox.Text = "";
                newHotkeyTextBox.Text = "";
            }
        }

        private void OnPresetToHotkeyMapChanged(string presetName, uint keyCode, PresetToHotkeyMap.PresetToHotkeyMappingChangeType changeType)
        {
            if (changeType == PresetToHotkeyMap.PresetToHotkeyMappingChangeType.HotkeySet)
            {
                mPresetToHotkeyDictionaryForComboBox[presetName] = keyCode;
            }
            else
            {
                mPresetToHotkeyDictionaryForComboBox[presetName] = 0;
            }
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
            if (mNewHotkey.NewHotkeyIsValid)
            {
                uint virtualKeyCode = 0;

                if ((mNewHotkey.NewHotkeyModifiers & Keys.Alt) > 0)
                    virtualKeyCode = virtualKeyCode | VirtualKeyCodes.MOD_ALT;

                if ((mNewHotkey.NewHotkeyModifiers & Keys.Control) > 0)
                    virtualKeyCode = virtualKeyCode | VirtualKeyCodes.MOD_CONTROL;

                if ((mNewHotkey.NewHotkeyModifiers & Keys.Shift) > 0)
                    virtualKeyCode = virtualKeyCode | VirtualKeyCodes.MOD_SHIFT;

                virtualKeyCode = virtualKeyCode | (uint)mNewHotkey.NewHotkeyKeyCode;

                mPresetToHotkeyMap.SetHotkey(((DisplayPreset)presetsComboBox.SelectedItem).Name, virtualKeyCode);
            }
        }

        private PresetToHotkeyMap mPresetToHotkeyMap;
        private Dictionary<string, uint> mPresetToHotkeyDictionaryForComboBox;
        private NewHotkey mNewHotkey;
    }
}
