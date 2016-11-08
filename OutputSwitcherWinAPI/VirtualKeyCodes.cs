using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcher.WinAPI
{
    public class VirtualKeyCodes
    {
        public const uint NO_MOD = 0;             // No modifier keys pressed.
        public const uint MOD_ALT = 0x0001;       // Either ALT key held down.
        public const uint MOD_CONTROL = 0x0002;   // Either CTRL key held down.
        public const uint MOD_SHIFT = 0x0004;     // Either SHIFT key held down.
        public const uint MOD_WIN = 0x0008;       // Either WIN key held down.

        public const uint VK_LBUTTON = 0x01;
        public const uint VK_RBUTTON = 0x02;
        public const uint VK_CANCEL = 0x03;
        public const uint VK_MBUTTON = 0x04;    /* NOT contiguous with L & RBUTTON */
        public const uint VK_XBUTTON1 = 0x05;    /* NOT contiguous with L & RBUTTON */
        public const uint VK_XBUTTON2 = 0x06;    /* NOT contiguous with L & RBUTTON */
        public const uint VK_BACK = 0x08;
        public const uint VK_TAB = 0x09;
        public const uint VK_CLEAR = 0x0C;
        public const uint VK_RETURN = 0x0D;
        public const uint VK_SHIFT = 0x10;
        public const uint VK_CONTROL = 0x11;
        public const uint VK_MENU = 0x12;
        public const uint VK_PAUSE = 0x13;
        public const uint VK_CAPITAL = 0x14;
        public const uint VK_KANA = 0x15;
        public const uint VK_HANGEUL = 0x15;  /* old name - should be here for compatibility */
        public const uint VK_HANGUL = 0x15;
        public const uint VK_JUNJA = 0x17;
        public const uint VK_FINAL = 0x18;
        public const uint VK_HANJA = 0x19;
        public const uint VK_KANJI = 0x19;
        public const uint VK_ESCAPE = 0x1B;
        public const uint VK_CONVERT = 0x1C;
        public const uint VK_NONCONVERT = 0x1D;
        public const uint VK_ACCEPT = 0x1E;
        public const uint VK_MODECHANGE = 0x1F;
        public const uint VK_SPACE = 0x20;
        public const uint VK_PRIOR = 0x21;
        public const uint VK_NEXT = 0x22;
        public const uint VK_END = 0x23;
        public const uint VK_HOME = 0x24;
        public const uint VK_LEFT = 0x25;
        public const uint VK_UP = 0x26;
        public const uint VK_RIGHT = 0x27;
        public const uint VK_DOWN = 0x28;
        public const uint VK_SELECT = 0x29;
        public const uint VK_PRINT = 0x2A;
        public const uint VK_EXECUTE = 0x2B;
        public const uint VK_SNAPSHOT = 0x2C;
        public const uint VK_INSERT = 0x2D;
        public const uint VK_DELETE = 0x2E;
        public const uint VK_HELP = 0x2F;
        public const uint VK_LWIN = 0x5B; // ASCII '0' - '9' (0x30 - 0x39)
        // * VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)

        public const uint VK_RWIN = 0x5C;
        public const uint VK_APPS = 0x5D;
        public const uint VK_SLEEP = 0x5F;
        public const uint VK_NUMPAD0 = 0x60;
        public const uint VK_NUMPAD1 = 0x61;
        public const uint VK_NUMPAD2 = 0x62;
        public const uint VK_NUMPAD3 = 0x63;
        public const uint VK_NUMPAD4 = 0x64;
        public const uint VK_NUMPAD5 = 0x65;
        public const uint VK_NUMPAD6 = 0x66;
        public const uint VK_NUMPAD7 = 0x67;
        public const uint VK_NUMPAD8 = 0x68;
        public const uint VK_NUMPAD9 = 0x69;
        public const uint VK_MULTIPLY = 0x6A;
        public const uint VK_ADD = 0x6B;
        public const uint VK_SEPARATOR = 0x6C;
        public const uint VK_SUBTRACT = 0x6D;
        public const uint VK_DECIMAL = 0x6E;
        public const uint VK_DIVIDE = 0x6F;
        public const uint VK_F1 = 0x70;
        public const uint VK_F2 = 0x71;
        public const uint VK_F3 = 0x72;
        public const uint VK_F4 = 0x73;
        public const uint VK_F5 = 0x74;
        public const uint VK_F6 = 0x75;
        public const uint VK_F7 = 0x76;
        public const uint VK_F8 = 0x77;
        public const uint VK_F9 = 0x78;
        public const uint VK_F10 = 0x79;
        public const uint VK_F11 = 0x7A;
        public const uint VK_F12 = 0x7B;
        public const uint VK_F13 = 0x7C;
        public const uint VK_F14 = 0x7D;
        public const uint VK_F15 = 0x7E;
        public const uint VK_F16 = 0x7F;
        public const uint VK_F17 = 0x80;
        public const uint VK_F18 = 0x81;
        public const uint VK_F19 = 0x82;
        public const uint VK_F20 = 0x83;
        public const uint VK_F21 = 0x84;
        public const uint VK_F22 = 0x85;
        public const uint VK_F23 = 0x86;
        public const uint VK_F24 = 0x87;
        public const uint VK_NUMLOCK = 0x90;
        public const uint VK_SCROLL = 0x91;
        public const uint VK_OEM_NEC_EQUAL = 0x92;   // '=' key on numpad
        public const uint VK_OEM_FJ_JISHO = 0x92;   // 'Dictionary' key
        public const uint VK_OEM_FJ_MASSHOU = 0x93;   // 'Unregister word' key
        public const uint VK_OEM_FJ_TOUROKU = 0x94;   // 'Register word' key
        public const uint VK_OEM_FJ_LOYA = 0x95;   // 'Left OYAYUBI' key
        public const uint VK_OEM_FJ_ROYA = 0x96;   // 'Right OYAYUBI' key

        //* VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
        public const uint VK_LSHIFT = 0xA0;
        public const uint VK_RSHIFT = 0xA1;
        public const uint VK_LCONTROL = 0xA2;
        public const uint VK_RCONTROL = 0xA3;
        public const uint VK_LMENU = 0xA4;
        public const uint VK_RMENU = 0xA5;
        public const uint VK_BROWSER_BACK = 0xA6;
        public const uint VK_BROWSER_FORWARD = 0xA7;
        public const uint VK_BROWSER_REFRESH = 0xA8;
        public const uint VK_BROWSER_STOP = 0xA9;
        public const uint VK_BROWSER_SEARCH = 0xAA;
        public const uint VK_BROWSER_FAVORITES = 0xAB;
        public const uint VK_BROWSER_HOME = 0xAC;
        public const uint VK_VOLUME_MUTE = 0xAD;
        public const uint VK_VOLUME_DOWN = 0xAE;
        public const uint VK_VOLUME_UP = 0xAF;
        public const uint VK_MEDIA_NEXT_TRACK = 0xB0;
        public const uint VK_MEDIA_PREV_TRACK = 0xB1;
        public const uint VK_MEDIA_STOP = 0xB2;
        public const uint VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const uint VK_LAUNCH_MAIL = 0xB4;
        public const uint VK_LAUNCH_MEDIA_SELECT = 0xB5;
        public const uint VK_LAUNCH_APP1 = 0xB6;
        public const uint VK_LAUNCH_APP2 = 0xB7;
        public const uint VK_OEM_1 = 0xBA;   // ';:' for US
        public const uint VK_OEM_PLUS = 0xBB;   // '+' any country
        public const uint VK_OEM_COMMA = 0xBC;   // ',' any country
        public const uint VK_OEM_MINUS = 0xBD;   // '-' any country
        public const uint VK_OEM_PERIOD = 0xBE;   // '.' any country
        public const uint VK_OEM_2 = 0xBF;   // '/?' for US
        public const uint VK_OEM_3 = 0xC0;   // '`~' for US
        public const uint VK_OEM_4 = 0xDB;  //  '[{' for US
        public const uint VK_OEM_5 = 0xDC;  //  '\|' for US
        public const uint VK_OEM_6 = 0xDD;  //  ']}' for US
        public const uint VK_OEM_7 = 0xDE;  //  ''"' for US
        public const uint VK_OEM_8 = 0xDF;
        public const uint VK_OEM_AX = 0xE1;  //  'AX' key on Japanese AX kbd
        public const uint VK_OEM_102 = 0xE2;  //  "<>" or "\|" on RT 102-key kbd.
        public const uint VK_ICO_HELP = 0xE3;  //  Help key on ICO
        public const uint VK_ICO_00 = 0xE4;  //  00 key on ICO
        public const uint VK_PROCESSKEY = 0xE5;
        public const uint VK_ICO_CLEAR = 0xE6;
        public const uint VK_PACKET = 0xE7;
        public const uint VK_OEM_RESET = 0xE9;
        public const uint VK_OEM_JUMP = 0xEA;
        public const uint VK_OEM_PA1 = 0xEB;
        public const uint VK_OEM_PA2 = 0xEC;
        public const uint VK_OEM_PA3 = 0xED;
        public const uint VK_OEM_WSCTRL = 0xEE;
        public const uint VK_OEM_CUSEL = 0xEF;
        public const uint VK_OEM_ATTN = 0xF0;
        public const uint VK_OEM_FINISH = 0xF1;
        public const uint VK_OEM_COPY = 0xF2;
        public const uint VK_OEM_AUTO = 0xF3;
        public const uint VK_OEM_ENLW = 0xF4;
        public const uint VK_OEM_BACKTAB = 0xF5;
        public const uint VK_ATTN = 0xF6;
        public const uint VK_CRSEL = 0xF7;
        public const uint VK_EXSEL = 0xF8;
        public const uint VK_EREOF = 0xF9;
        public const uint VK_PLAY = 0xFA;
        public const uint VK_ZOOM = 0xFB;
        public const uint VK_NONAME = 0xFC;
        public const uint VK_PA1 = 0xFD;
        public const uint VK_OEM_CLEAR = 0xFE;
        public const uint VK_0 = 0x30;
        public const uint VK_1 = 0x31;
        public const uint VK_2 = 0x32;
        public const uint VK_3 = 0x33;
        public const uint VK_4 = 0x34;
        public const uint VK_5 = 0x35;
        public const uint VK_6 = 0x36;
        public const uint VK_7 = 0x37;
        public const uint VK_8 = 0x38;
        public const uint VK_9 = 0x39;
        public const uint VK_A = 0x41;
        public const uint VK_B = 0x42;
        public const uint VK_C = 0x43;
        public const uint VK_D = 0x44;
        public const uint VK_E = 0x45;
        public const uint VK_F = 0x46;
        public const uint VK_G = 0x47;
        public const uint VK_H = 0x48;
        public const uint VK_I = 0x49;
        public const uint VK_J = 0x4A;
        public const uint VK_K = 0x4B;
        public const uint VK_L = 0x4C;
        public const uint VK_M = 0x4D;
        public const uint VK_N = 0x4E;
        public const uint VK_O = 0x4F;
        public const uint VK_P = 0x50;
        public const uint VK_Q = 0x51;
        public const uint VK_R = 0x52;
        public const uint VK_S = 0x53;
        public const uint VK_T = 0x54;
        public const uint VK_U = 0x55;
        public const uint VK_V = 0x56;
        public const uint VK_W = 0x57;
        public const uint VK_X = 0x58;
        public const uint VK_Y = 0x59;
        public const uint VK_Z = 0x5A;

        public static bool KeyCodeHasCtrl(uint keycode)
        {
            return (keycode & MOD_CONTROL) > 0;
        }

        public static bool KeyCodeHasAlt(uint keycode)
        {
            return (keycode & MOD_ALT) > 0;
        }

        public static bool KeyCodeHasShift(uint keycode)
        {
            return (keycode & MOD_SHIFT) > 0;
        } 

        public static bool TryGetAlphanumericKeyFromKeyCode(uint keycode, out char alphaNumbericChar)
        {
            if ((keycode >= VK_A && keycode <= VK_Z) ||
                (keycode >= VK_0 && keycode <= VK_9))
            {
                alphaNumbericChar = (char)keycode;
                return true;
            }
            else
            {
                alphaNumbericChar = ' ';
                return false;
            }
        }
    }
}
