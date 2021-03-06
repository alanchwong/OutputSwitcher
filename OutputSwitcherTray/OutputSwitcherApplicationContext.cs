﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OutputSwitcher.Core;
using OutputSwitcher.WinAPI;

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
            InitializeHotkey();
        }

        /// <summary>
        /// Initializes global hotkey infrastructure.
        /// </summary>
        private void InitializeHotkey()
        {
            mMessageFilter = new HotkeyMessageFilter();

            HotkeyRegistrar.Initialize(PresetToHotkeyMap.GetInstance());
            HotkeyHandler.Initialize(PresetToHotkeyMap.GetInstance(), mMessageFilter);

            // Technically this can degrade performance, but we're not doing a ton of work in there.
            // Just have to make sure it's non-blocking.
            Application.AddMessageFilter(mMessageFilter);
        }

        /// <summary>
        /// Creates the app's context menu items.
        /// </summary>
        private void InitializeContextMenu()
        {
            ToolStripItem[] afterPresetsToolStripItems = new ToolStripItem[3];
            afterPresetsToolStripItems[0] = new ToolStripButton("Capture current display configuration as preset", null, CaptureNewPreset_ItemClicked);
            afterPresetsToolStripItems[1] = new ToolStripButton("Edit global hotkeys", null, EditGlobalHotkeys_ItemClicked);
            afterPresetsToolStripItems[2] = new ToolStripButton("Exit", null, ContextMenuStrip_Exit);

            List<DisplayPreset> displayPresets = DisplayPresetCollection.GetDisplayPresetCollection().GetPresets();

            List<ToolStripItem> applyPresetDropDownItems = new List<ToolStripItem>();
            List<ToolStripItem> removePresetDropDownItems = new List<ToolStripItem>();

            if (displayPresets.Count > 0)
            {
                foreach (DisplayPreset preset in displayPresets)
                {
                    applyPresetDropDownItems.Add(new PresetContextMenuItem(preset.Name, ApplyPresetDropDown_ItemClicked));
                    removePresetDropDownItems.Add(new PresetContextMenuItem(preset.Name, RemovePresetDropDown_ItemClicked));
                }
            }
            else
            {
                applyPresetDropDownItems.Add(new NoPresetPresetContextMenuItem());
                removePresetDropDownItems.Add(new NoPresetPresetContextMenuItem());
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
            mNotifyIcon.ContextMenuStrip.Show();
            mNotifyIcon.ContextMenuStrip.Focus();
        }

        /// <summary>
        /// Event handler for when an item in the Apply Preset drop down is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyPresetDropDown_ItemClicked(object sender, EventArgs e)
        {
            PresetContextMenuItem presetContextMenuItem = sender as PresetContextMenuItem;

            if (presetContextMenuItem != null)
            {
                SafePresetApplier.ApplyPresetWithRevertCountdown(
                    DisplayPresetCollection.GetDisplayPresetCollection().GetPreset(presetContextMenuItem.PresetName));
            }
        }

        /// <summary>
        /// Event handler for when an item in the Remove Preset drop down is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePresetDropDown_ItemClicked(object sender, EventArgs e)
        {
            PresetContextMenuItem presetContextMenuItem = sender as PresetContextMenuItem;

            if (presetContextMenuItem != null)
            {
                DisplayPresetCollection.GetDisplayPresetCollection().TryRemoveDisplayPreset(presetContextMenuItem.PresetName);
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
            ShowEnterNewPresetNameForm();
        }

        /// <summary>
        /// Event handler for when "Edit global hotkeys" context menu item is clicked. Launches
        /// a form to allow the user to edit global hotkeys for the display presets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditGlobalHotkeys_ItemClicked(object sender, EventArgs e)
        {
            if (mPresetHotkeyForm == null)
            {
                mPresetHotkeyForm = new PresetHotkeyForm(DisplayPresetCollection.GetDisplayPresetCollection(), PresetToHotkeyMap.GetInstance());
                mPresetHotkeyForm.FormClosed += MPresetHotkeyForm_FormClosed;
            }
            else if (!mPresetHotkeyForm.Visible)
                mPresetHotkeyForm.ShowDialog();
            else
                mPresetHotkeyForm.Activate();
        }

        private void MPresetHotkeyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mPresetHotkeyForm = null;
        }

        /// <summary>
        /// Show the new preset name form if it isn't already visible.
        /// </summary>
        private void ShowEnterNewPresetNameForm()
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
                // If this is the first new preset added, remove the "None" disabled items.
                if (applyPresetDropDownButton.DropDown.Items.Count == 1 &&
                    applyPresetDropDownButton.DropDown.Items[0] is NoPresetPresetContextMenuItem)
                {
                    applyPresetDropDownButton.DropDown.Items.RemoveAt(0);
                }

                if (removePresetDropDownButton.DropDown.Items.Count == 1 &&
                    removePresetDropDownButton.DropDown.Items[0] is NoPresetPresetContextMenuItem)
                {
                    removePresetDropDownButton.DropDown.Items.RemoveAt(0);
                }

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

                // If all presets have been removed, place the "None" disabled menu item back into the drop down.
                if (applyPresetDropDownButton.DropDown.Items.Count == 0)
                {
                    applyPresetDropDownButton.DropDown.Items.Add(new NoPresetPresetContextMenuItem());
                }

                if (removePresetDropDownButton.DropDown.Items.Count == 0)
                {
                    removePresetDropDownButton.DropDown.Items.Add(new NoPresetPresetContextMenuItem());
                }
            }

            applyPresetDropDownButton.DropDown.PerformLayout();
            removePresetDropDownButton.DropDown.PerformLayout();
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
        private PresetHotkeyForm mPresetHotkeyForm;

        private ToolStripDropDownButton applyPresetDropDownButton;
        private ToolStripDropDownButton removePresetDropDownButton;

        private HotkeyMessageFilter mMessageFilter;
    }
}
