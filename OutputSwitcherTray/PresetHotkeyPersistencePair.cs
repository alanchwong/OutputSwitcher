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
        public PresetHotkeyPersistencePair(string presetName, uint keyCode)
        {
            PresetName = presetName;
            KeyCode = keyCode;
        }

        public string PresetName;
        public uint KeyCode;
    }
}
