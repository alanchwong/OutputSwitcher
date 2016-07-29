using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace OutputSwitcher.Tray
{
    internal class PresetContextMenuItem : ToolStripButton, IComparable<PresetContextMenuItem>
    {
        public PresetContextMenuItem(string presetName, EventHandler eventHandler, bool usePresetNameAsText = true) : base("", null, eventHandler)
        {
            UsePresetNameAsText = usePresetNameAsText;
            PresetName = presetName;

            if (UsePresetNameAsText)
            {
                Text = PresetName;
            }
        }

        public string PresetName {
            get { return mPresetName;  }

            set
            {
                mPresetName = value;

                if (UsePresetNameAsText)
                    Text = mPresetName;
            }
        }


        public bool UsePresetNameAsText { get; set; }

        public int CompareTo(PresetContextMenuItem other)
        {
            return PresetName.CompareTo(other.PresetName);
        }

        private string mPresetName;
    }
}
