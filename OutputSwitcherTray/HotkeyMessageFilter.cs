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

        public delegate void PresetSwitchHotkeyPressedEventHandler(ushort modifierKeyFlags, ushort keycode);
        public event PresetSwitchHotkeyPressedEventHandler PresetSwitchHotkeyPressed;

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                // The lParam of the message contains the pressed keys, which we need
                // to use to determine which preset we should switch to. The low-order
                // word of lParam contains the modifier bit flags, and the high-order 
                // word contains the primary key's keycode. A tiny wrinkle is that the
                // LPARAM is 32-bit in x86 settings, and 64-bit in x86-64 settings, but
                // words are 16-bits in either case.
                long lParam = IntPtr.Size == 8 ? m.LParam.ToInt64() : m.LParam.ToInt32();
                ushort modifierKeyFlags = unchecked((ushort)lParam);  // Cast to 16-bit to chop off high order word
                ushort keycode = unchecked((ushort)(lParam >> 16));   // Shift high order word over and chop off the rest.

                PresetSwitchHotkeyPressed?.Invoke(modifierKeyFlags, keycode);
            }

            return false;
        }

    }
}
