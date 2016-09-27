using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutputSwitcher.TrayApp
{
    class HotkeyMessageFilter : IMessageFilter
    {
        private const int WM_HOTKEY = 0x0312;     // ID of Windows Hotkey event.

        public delegate void PresetSwitchHotkeyPressed();
        public event PresetSwitchHotkeyPressed OnPresetSwitchHotkey;

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                OnPresetSwitchHotkey?.Invoke();
            }

            return false;
        }

    }
}
