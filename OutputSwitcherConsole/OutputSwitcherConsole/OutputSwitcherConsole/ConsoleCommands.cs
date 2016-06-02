using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutputSwitcherConsole.WinAPI;

namespace OutputSwitcherConsole
{
    class ConsoleCommands
    {
        static public void ShowDisplayDevicesToConsole()
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
        }

        static public void ShowDisplayAdapterDeviceSettings()
        {
            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            displayDevice.cb = System.Runtime.InteropServices.Marshal.SizeOf(displayDevice);

            uint devNum = 0;

            while (DisplaySettings.EnumDisplayDevices(null, devNum, ref displayDevice, 0))
            {
                string displayAdapterName = displayDevice.DeviceName;

                DEVMODE devMode = new DEVMODE();
                devMode.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(devMode);
                devMode.dmDriverExtra = 0;

                DisplaySettings.EnumDisplaySettingsEx(
                    displayAdapterName,
                    DisplaySettings.ENUM_CURRENT_SETTINGS,
                    ref devMode,
                    0);

                DisplaySettings.WriteDisplayDeviceSettingsToConsole(devMode, displayAdapterName);

                devNum++;
            }
        }
    }
}
