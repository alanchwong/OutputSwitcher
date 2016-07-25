using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OutputSwitcher.Core;

namespace OutputSwitcher.Tray
{
    public class OutputSwitcherApplicationContext : ApplicationContext
    {
        public OutputSwitcherApplicationContext()
        {
            mComponents = new System.ComponentModel.Container();
            mNotifyIcon = new NotifyIcon(mComponents)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.OutputSwitcherTrayIcon,
                Text = "OutputSwitcher",
                Visible = true,
            };

            mNotifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            mNotifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private void InitializePresetsToolStripItemCollection()
        {
            List<DisplayPreset> displayPresets = DisplayPresetCollection.GetDisplayPresetCollection().GetPresets();

            List<ToolStripItem> applyPresetDropDownItems = new List<ToolStripItem>();
            List<ToolStripItem> removePresetDropDownItems = new List<ToolStripItem>();

            foreach (DisplayPreset preset in displayPresets)
            {
                applyPresetDropDownItems.Add(new ToolStripButton(preset.Name, null, ApplyPresetDropDown_ItemClicked));
                removePresetDropDownItems.Add(new ToolStripButton(preset.Name, null, RemovePresetDropDown_ItemClicked));
            }

            ToolStripDropDownButton applyPresetDropDownButton = new ToolStripDropDownButton("Apply Preset...", null, applyPresetDropDownItems.ToArray());
            ToolStripDropDownButton removePresetDropDownButton = new ToolStripDropDownButton("Remove Preset...", null, removePresetDropDownItems.ToArray());
            
            mAddRemovePresetsToolStripItems = new ToolStripItem[2];
            mAddRemovePresetsToolStripItems[0] = applyPresetDropDownButton;
            mAddRemovePresetsToolStripItems[1] = removePresetDropDownButton;
        }

        private void InitializeAfterPresetsToolStripItemCollection()
        {
            mAfterPresetsToolStripItems = new ToolStripItem[2];
            mAfterPresetsToolStripItems[0] = new ToolStripButton("Capture Current As Preset", null);
            mAfterPresetsToolStripItems[1] = new ToolStripButton("Exit", null, ContextMenuStrip_Exit);
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InitializePresetsToolStripItemCollection();

            if (mAfterPresetsToolStripItems == null)
            {
                InitializeAfterPresetsToolStripItemCollection();
            }

            mNotifyIcon.ContextMenuStrip.Items.Clear();
            mNotifyIcon.ContextMenuStrip.Items.AddRange(mAddRemovePresetsToolStripItems);
            mNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            mNotifyIcon.ContextMenuStrip.Items.AddRange(mAfterPresetsToolStripItems);

            mNotifyIcon.ContextMenuStrip.PerformLayout();

            e.Cancel = false;
        }

        private void ContextMenuStrip_Exit(object sender, EventArgs e)
        {
            ExitThread();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
        }

        private void ApplyPresetDropDown_ItemClicked(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;

            // The ToolStripButton's text is the name of the preset -- TODO: should actually encapsulate this in a subclass.
            if (button != null)
            {
                DisplayPresetRecorderAndApplier.ApplyPreset(
                    DisplayPresetCollection.GetDisplayPresetCollection().GetPreset(button.Text));
            }
        }

        private void RemovePresetDropDown_ItemClicked(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;

            // The ToolStripButton's text is the name of the preset -- TODO: should actually encapsulate this in a subclass.
            if (button != null)
            {
                DisplayPresetCollection.GetDisplayPresetCollection().TryRemoveDisplayPreset(button.Text);
            }
        }
        protected override void Dispose(bool disposing)
        {
            // Dispose of any resources that we're holding on to here.
            if (disposing && mComponents != null)
            {
                mComponents.Dispose();
            }
        }

        protected override void ExitThreadCore()
        {
            // Do clean up before shutting down.
            mNotifyIcon.Visible = false;

            base.ExitThreadCore();
        }

        private System.ComponentModel.Container mComponents;
        private NotifyIcon mNotifyIcon;

        private ToolStripItem[] mAddRemovePresetsToolStripItems;
        private ToolStripItem[] mAfterPresetsToolStripItems;
    }
}
