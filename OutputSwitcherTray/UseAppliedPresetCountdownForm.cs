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
    public partial class UseAppliedPresetCountdownForm : Form
    {
        public UseAppliedPresetCountdownForm(DisplayPreset revertPreset)
        {
            InitializeComponent();

            mRevertPreset = revertPreset;
            mCountdown = COUNTDOWN_TIME_MAX;

            UpdateLabelCountdownText();

            mTimer = new Timer();
            mTimer.Interval = 1000; // Tick every 1 second.
            mTimer.Tick += Timer_Tick;
            mTimer.Start();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            CloseAndRevertPreset(false);
        }

        private void ButtonRevert_Click(object sender, EventArgs e)
        {
            // This is interesting, we now have a number of places in
            // the TrayApp namespace that all core into Core to perform
            // app tasks. Would this be better if we had a centralized
            // controller class in the TrayApp namespace that handled
            // talking to Core?
            CloseAndRevertPreset(true);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mCountdown--;

            UpdateLabelCountdownText();

            if (mCountdown == 0)
            {
                CloseAndRevertPreset(true);
            }
        }

        private void UpdateLabelCountdownText()
        {
            LabelCountdownText.Text = String.Format(Properties.Resources.KeepPresetCountdownString, mCountdown);
        }

        private void CloseAndRevertPreset(bool revertPreset)
        {
            mTimer.Stop();

            if (revertPreset)
            {
                DisplayPresetRecorderAndApplier.ApplyPreset(mRevertPreset);
            }

            Close();
        }

        private DisplayPreset mRevertPreset;
        private Timer mTimer;
        private uint mCountdown;

        private static readonly uint COUNTDOWN_TIME_MAX = 15;
    }
}
