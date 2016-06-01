using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutputSwitcherConsole.WinAPI;

namespace OutputSwitcherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DISPLAY_DEVICE adapterDisplayDevice = new DISPLAY_DEVICE();
            adapterDisplayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(adapterDisplayDevice);

            uint adapterDevNum = 0;
            string displayAdapterName;

            while (DisplaySettings.EnumDisplayDevices(null, adapterDevNum, ref adapterDisplayDevice, 0))
            {
                displayAdapterName = adapterDisplayDevice.DeviceName;

                DisplaySettings.WriteDisplayDeviceToConsole(adapterDisplayDevice, adapterDevNum);

                uint devNum = 0;

                DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
                displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);

                while (DisplaySettings.EnumDisplayDevices(displayAdapterName, devNum, ref displayDevice, 0))
                {
                    DisplaySettings.WriteDisplayDeviceToConsole(displayDevice, devNum);
                    devNum++;
                }

                adapterDevNum++;
            }

            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
