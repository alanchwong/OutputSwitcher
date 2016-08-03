using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OutputSwitcher.Core;

namespace OutputSwitcher.TrayApp
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

            mNotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            InitializeContextMenu();
        }

        /// <summary>
        /// Creates the app's context menu items.
        /// </summary>
        private void InitializeContextMenu()
        {
            ToolStripItem[] afterPresetsToolStripItems = new ToolStripItem[2];
            afterPresetsToolStripItems[0] = new ToolStripButton("Capture current display configuration as preset", null, CaptureNewPreset_ItemClicked);
            afterPresetsToolStripItems[1] = new ToolStripButton("Exit", null, ContextMenuStrip_Exit);

            List<DisplayPreset> displayPresets = DisplayPresetCollection.GetDisplayPresetCollection().GetPresets();

            List<PresetContextMenuItem> applyPresetDropDownItems = new List<PresetContextMenuItem>();
            List<PresetContextMenuItem> removePresetDropDownItems = new List<PresetContextMenuItem>();

            foreach (DisplayPreset preset in displayPresets)
            {
                applyPresetDropDownItems.Add(new PresetContextMenuItem(preset.Name, ApplyPresetDropDown_ItemClicked));
                removePresetDropDownItems.Add(new PresetContextMenuItem(preset.Name, RemovePresetDropDown_ItemClicked));
            }

            applyPresetDropDownButton = new ToolStripDropDownButton("Apply display preset...", null, applyPresetDropDownItems.ToArray());
            removePresetDropDownButton = new ToolStripDropDownButton("Remove display preset...", null, removePresetDropDownItems.ToArray());

            ToolStripItem[] mAddRemovePresetsToolStripItems = new ToolStripItem[2];
            mAddRemovePresetsToolStripItems[0] = applyPresetDropDownButton;
            mAddRemovePresetsToolStripItems[1] = removePresetDropDownButton;

            ToolStripItem[] beforePresetsToolStripItems = new ToolStripItem[1];
            beforePresetsToolStripItems[0] = new ToolStripLabel("OutputSwitcher");
            beforePresetsToolStripItems[0].Font = new System.Drawing.Font(beforePresetsToolStripItems[0].Font, System.Drawing.FontStyle.Bold);

            mNotifyIcon.ContextMenuStrip.Items.Clear();
            mNotifyIcon.ContextMenuStrip.Items.AddRange(beforePresetsToolStripItems);
            mNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            mNotifyIcon.ContextMenuStrip.Items.AddRange(mAddRemovePresetsToolStripItems);
            mNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            mNotifyIcon.ContextMenuStrip.Items.AddRange(afterPresetsToolStripItems);

            mNotifyIcon.ContextMenuStrip.PerformLayout();

            DisplayPresetCollection.GetDisplayPresetCollection().DisplayPresetCollectionChanged += OnDisplayPresetCollectionChanged;

        }

        /// <summary>
        /// Event handler for the Exit menu item in the context menu. Terminates this application context's thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Exit(object sender, EventArgs e)
        {
            ExitThread();
        }

        /// <summary>
        /// Event handler for a double-click action on the task tray icon. Displays the context menu.
        /// The MouseDoubleClick event only ever sends location (0,0) so there's no difference in handling
        /// that event over this one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            mNotifyIcon.ContextMenuStrip.Visible = true;
        }

        /// <summary>
        /// Event handler for when an item in the Apply Preset drop down is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyPresetDropDown_ItemClicked(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;

            // The ToolStripButton's text is the name of the preset -- TODO: should actually encapsulate this in a subclass.
            if (button != null)
            {
                DisplayPreset lastConfig = 
                    DisplayPresetRecorderAndApplier.ReturnLastConfigAndApplyPreset(
                        DisplayPresetCollection.GetDisplayPresetCollection().GetPreset(button.Text));

                // Pop up a dialog to give user the option to keep the configuration or else
                // automatically revert to the last configuration.
                // TODO: This seems weird to pass control away like this.
                UseAppliedPresetCountdownForm revertPresetCountdownForm = new UseAppliedPresetCountdownForm(lastConfig);
                revertPresetCountdownForm.Show();
            }
        }

        /// <summary>
        /// Event handler for when an item in the Remove Preset drop down is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePresetDropDown_ItemClicked(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;

            // The ToolStripButton's text is the name of the preset -- TODO: should actually encapsulate this in a subclass.
            if (button != null)
            {
                DisplayPresetCollection.GetDisplayPresetCollection().TryRemoveDisplayPreset(button.Text);
            }
        }

        /// <summary>
        /// Event handler for when the capture new preset menu item is clicked. Launches a form
        /// to allow the user to enter a new preset name for the current configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureNewPreset_ItemClicked(object sender, EventArgs e)
        {
            if (mEnterNewPresetNameForm == null)
            {
                mEnterNewPresetNameForm = new EnterNewPresetNameForm();
            }

            if (!mEnterNewPresetNameForm.Visible)
            {
                mEnterNewPresetNameForm.ShowDialog();
            }
            else
            {
                mEnterNewPresetNameForm.Activate();
            }
        }

        /// <summary>
        /// Event handler for when display preset collection changes and we need to update the context menu
        /// items containing the presets that are available for apply/remove.
        /// </summary>
        /// <param name="changeType">The type of collection change.</param>
        /// <param name="presetName">The name of the preset added or removed.</param>
        private void OnDisplayPresetCollectionChanged(DisplayPresetCollection.DisplayPresetCollectionChangeType changeType, string presetName)
        {
            if (changeType == DisplayPresetCollection.DisplayPresetCollectionChangeType.PresetAdded)
            {
                applyPresetDropDownButton.DropDown.Items.Add(new PresetContextMenuItem(presetName, ApplyPresetDropDown_ItemClicked));
                removePresetDropDownButton.DropDown.Items.Add(new PresetContextMenuItem(presetName, RemovePresetDropDown_ItemClicked));
            }
            else
            {
                int applyIndex = 0;
                int removeIndex = 0;

                foreach (ToolStripItem menuItem in applyPresetDropDownButton.DropDown.Items)
                {
                    PresetContextMenuItem presetItem = menuItem as PresetContextMenuItem;

                    if (presetItem != null && presetItem.PresetName.Equals(presetName))
                    {
                        applyPresetDropDownButton.DropDown.Items.RemoveAt(applyIndex);
                        break;
                    }

                    applyIndex++;
                }

                foreach (PresetContextMenuItem menuItem in removePresetDropDownButton.DropDown.Items)
                {
                    PresetContextMenuItem presetItem = menuItem as PresetContextMenuItem;

                    if (presetItem != null && presetItem.PresetName.Equals(presetName))
                    {
                        removePresetDropDownButton.DropDown.Items.RemoveAt(removeIndex);
                        break;
                    }

                    removeIndex++;
                }
            }

            mNotifyIcon.ContextMenuStrip.PerformLayout();   // Refresh the layout
        }

        /// <summary>
        /// Called by the public Dispose() method as part of terminating the thread. Disposes
        /// of resources held by this instance.
        /// </summary>
        /// <param name="disposing">True if should dispose managed resources in addition to unmanaged.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose of any resources that we're holding on to here.
            if (disposing && mComponents != null)
            {
                mComponents.Dispose();
            }
        }

        /// <summary>
        /// Called when the public ExitThread() is called.
        /// </summary>
        protected override void ExitThreadCore()
        {
            // Do clean up before shutting down.
            mNotifyIcon.Visible = false;

            base.ExitThreadCore();
        }

        private System.ComponentModel.Container mComponents;
        private NotifyIcon mNotifyIcon;

        private EnterNewPresetNameForm mEnterNewPresetNameForm;

        private ToolStripDropDownButton applyPresetDropDownButton;
        private ToolStripDropDownButton removePresetDropDownButton;
    }
}
