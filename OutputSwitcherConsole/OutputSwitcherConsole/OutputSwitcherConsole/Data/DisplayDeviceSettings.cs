using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutputSwitcherConsole.WinAPI;

namespace OutputSwitcherConsole.Data
{
    [Serializable]
    public class DisplayDeviceSettings
    {
        /// <summary>
        /// Type for DisplayDeviceSettings that mirrors the DEVMODE.mPosition POINTL struct.
        /// </summary>
        [Serializable]
        public struct POINTL
        {
            public Int32 x;
            public Int32 y;
        }

        /// <summary>
        /// Creates an empty DisplayDeviceSettings. Required for serialization.
        /// </summary>
        public DisplayDeviceSettings()
        {

        }

        public DisplayDeviceSettings(string deviceName, string deviceID, DEVMODE devMode)
        {
            mDeviceName = deviceName;
            mPosition.x = devMode.dmPosition.x;
            mPosition.y = devMode.dmPosition.y;
            mPelsHeight = devMode.dmPelsHeight;
            mPelsWidth = devMode.dmPelsWidth;
            mOrientation = devMode.dmDisplayOrientation;
            DisplayFrequency = devMode.dmDisplayFrequency;
            DeviceID = deviceID;
        }

        /// <summary>
        /// X,Y position of the display device in the Desktop. (Since POINTL
        /// is a struct, DisplayDeviceSettings is still immutable because this
        /// will return a copy.)
        /// </summary>
        public POINTL Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public int PelsWidth
        {
            get { return mPelsWidth; }
            set { mPelsWidth = value; }
        }

        public int PelsHeight
        {
            get { return mPelsHeight; }
            set { mPelsHeight = value; }
        }

        public string DeviceName
        {
            get { return mDeviceName; }
            set { mDeviceName = value; }
        }

        public int Orientation
        {
            get { return mOrientation; }
            set { mOrientation = value; }
        }

        public Int32 DisplayFrequency
        {
            get; set;
        }

        /// <summary>
        /// DeviceID from a DISPLAY_DEVICE struct returned by EnumDisplayDevices that identifies
        /// a specific physical monitor.
        /// </summary>
        public string DeviceID
        {
            get; set;
        }

        /// <summary>
        /// Provides a DEVMODE struct populated with the values of this DisplayDeviceSettings
        /// instance with the DEVMODE.dmFields member set to indicate that all counterparts
        /// to this DisplayDeviceSettings' properties are set for use by ChangeDisplaySettings.
        /// </summary>
        /// <returns>DEVMODE struct with display configuration set based on this instance's properties.</returns>
        public DEVMODE ToDevModeForChangeDisplaySettings()
        {
            DEVMODE devMode = new DEVMODE();

            devMode.dmSize = (short)System.Runtime.InteropServices.Marshal.SizeOf(devMode);
            devMode.dmDriverExtra = 0;
            devMode.dmPosition = new WinAPI.POINTL();
            devMode.dmPosition.x = Position.x;
            devMode.dmPosition.y = Position.y;
            devMode.dmPelsWidth = PelsWidth;
            devMode.dmPelsHeight = PelsHeight;
            devMode.dmDisplayOrientation = Orientation;
            devMode.dmDisplayFrequency = DisplayFrequency;
            devMode.dmFields = DM.Position | DM.PelsHeight | DM.PelsWidth | DM.DisplayOrientation | DM.DisplayFrequency;

            return devMode;
        }

        private POINTL mPosition;
        private Int32 mPelsWidth;
        private Int32 mPelsHeight;
        private string mDeviceName;
        private Int32 mOrientation;
    }
}
