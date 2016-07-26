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

        /// <summary>
        /// Populates the apply and remove preset drop down menus with the latest set of
        /// presets available in the DisplayPresetCollection.
        /// </summary>
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

            ToolStripDropDownButton applyPresetDropDownButton = new ToolStripDropDownButton("Apply display preset...", null, applyPresetDropDownItems.ToArray());
            ToolStripDropDownButton removePresetDropDownButton = new ToolStripDropDownButton("Remove display preset...", null, removePresetDropDownItems.ToArray());

            mAddRemovePresetsToolStripItems = new ToolStripItem[2];
            mAddRemovePresetsToolStripItems[0] = applyPresetDropDownButton;
            mAddRemovePresetsToolStripItems[1] = removePresetDropDownButton;
        }

        /// <summary>
        /// Creates the set of context menu items that come after the apply/remove preset drop downs.
        /// </summary>
        private void InitializeAfterPresetsToolStripItemCollection()
        {
            mAfterPresetsToolStripItems = new ToolStripItem[2];
            mAfterPresetsToolStripItems[0] = new ToolStripButton("Capture current display configuration as preset", null, CaptureNewPreset_ItemClicked);
            mAfterPresetsToolStripItems[1] = new ToolStripButton("Exit", null, ContextMenuStrip_Exit);
        }

        /// <summary>
        /// Creates the set of context emnu items that come before the apple/remove preset drop downs.
        /// </summary>
        private void InitializeBeforePresetsToolStripItems()
        {
            mBeforePresetsToolStripItems = new ToolStripItem[1];
            mBeforePresetsToolStripItems[0] = new ToolStripLabel("OutputSwitcher");
            mBeforePresetsToolStripItems[0].Font = new System.Drawing.Font(mBeforePresetsToolStripItems[0].Font, System.Drawing.FontStyle.Bold);
        }

        /// <summary>
        /// Event handler for the NotifyIcon's context menu strip opening. Populates the context menu with 
        /// the menu items as it opens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /* This is not the most efficient way to do this. We shouldn't throw out menu items every
             * time even if they haven't changed. A quick test shows that until the garbage collector
             * runs the app starts eating up more memory each time the context menu is opened. Sure,
             * it's a small amount but that's still no good. Should have a way to detect changes in
             * the DisplayPresetCollection such that we only regenerate items when that collection
             * changes.
             */

            InitializePresetsToolStripItemCollection();

            if (mAfterPresetsToolStripItems == null)
            {
                InitializeAfterPresetsToolStripItemCollection();
            }

            if (mBeforePresetsToolStripItems == null)
            {
                InitializeBeforePresetsToolStripItems();
            }

            mNotifyIcon.ContextMenuStrip.Items.Clear();
            mNotifyIcon.ContextMenuStrip.Items.AddRange(mBeforePresetsToolStripItems);
            mNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            mNotifyIcon.ContextMenuStrip.Items.AddRange(mAddRemovePresetsToolStripItems);
            mNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            mNotifyIcon.ContextMenuStrip.Items.AddRange(mAfterPresetsToolStripItems);

            mNotifyIcon.ContextMenuStrip.PerformLayout();

            e.Cancel = false;
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
                DisplayPresetRecorderAndApplier.ApplyPreset(
                    DisplayPresetCollection.GetDisplayPresetCollection().GetPreset(button.Text));
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

        private ToolStripItem[] mBeforePresetsToolStripItems;
        private ToolStripItem[] mAddRemovePresetsToolStripItems;
        private ToolStripItem[] mAfterPresetsToolStripItems;
    }
}
