using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutputSwitcher.TrayApp
{
    internal class NoPresetPresetContextMenuItem : ToolStripButton
    {
        public NoPresetPresetContextMenuItem()
        {
            Text = "None";
            Enabled = false;
        }
    }
}
