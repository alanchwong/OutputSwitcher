using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OutputSwitcher.WinAPI
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        public Int32 x;
        public Int32 y;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECTL
    {
        public Int32 left;
        public Int32 top;
        public Int32 right;
        public Int32 bottom;
    }
}
