using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcher.TrayApp
{
    [Serializable]
    public struct VirtualHotkey
    {
        public VirtualHotkey(uint modifierKeycode, uint keyCode)
        {
            ModifierKeyCode = modifierKeycode;
            KeyCode = keyCode;
        }

        public uint ModifierKeyCode;
        public uint KeyCode;
    }
}
