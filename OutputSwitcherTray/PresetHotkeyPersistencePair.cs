using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcher.TrayApp
{
    [Serializable]
    public struct PresetHotkeyPersistencePair
    {
        public PresetHotkeyPersistencePair(string presetName, VirtualHotkey virtualHotkey)
        {
            PresetName = presetName;
            HotkeyKeycode = virtualHotkey;
        }

        public string PresetName;
        public VirtualHotkey HotkeyKeycode;
    }
}
