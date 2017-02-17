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

namespace OutputSwitcher.TrayApp
{
    public partial class EnterNewPresetNameForm : Form
    {
        public EnterNewPresetNameForm()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (TryCreateNewPresetWithEnteredName())
            {
                Close();
            }
        }

        private void TextboxEnterNewPresetName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (TryCreateNewPresetWithEnteredName())
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// Verifies that the input in the textbox is a unique name and is non-empty, calls in to
        /// OutputSwitcher.Core to create and save the new preset.
        /// </summary>
        /// <returns>True if preset created and saved. False if input is invalid or failed to save new preset.</returns>
        private bool TryCreateNewPresetWithEnteredName()
        {
            if (TextboxEnterNewPresetName.Text.Length < 1)
            {
                MessageBox.Show("Please enter at least one character.", "OutputSwitcher");
                return false;
            }

            string newPresetName = TextboxEnterNewPresetName.Text;

            DisplayPresetCollection displayPresetCollection = DisplayPresetCollection.GetDisplayPresetCollection();
            DisplayPreset existingPreset = displayPresetCollection.GetPreset(newPresetName);

            if (existingPreset != null)
            {
                MessageBox.Show("Name already exists, please enter a unique name.", "OutputSwitcher");
                return false;
            }

            DisplayPreset newPreset = DisplayPresetRecorderAndApplier.RecordCurrentConfiguration(newPresetName);

            if (!displayPresetCollection.TryAddDisplayPreset(newPreset))
            {
                MessageBox.Show("Failed to save new display preset configuration.", "OutputSwitcher");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
