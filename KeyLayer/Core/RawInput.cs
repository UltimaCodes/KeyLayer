using System;
using System.Runtime.InteropServices;

namespace KeyLayer.Core
{
    public static class RawInput
    {
        public const int RID_INPUT = 0x10000003;
        public const int RIM_TYPEKEYBOARD = 1;

        public enum RawInputDeviceType : uint
        {
            MOUSE = 0,
            KEYBOARD = 1,
            HID = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            public uint dwType;
        }

        [DllImport("User32.dll")]
        public static extern uint GetRawInputDeviceList(
            [In, Out] RAWINPUTDEVICELIST[] pRawInputDeviceList,
            ref uint puiNumDevices,
            uint cbSize
        );

        [DllImport("User32.dll", SetLastError = true)]
        public static extern uint GetRawInputDeviceInfo(
            IntPtr hDevice,
            uint uiCommand,
            IntPtr pData,
            ref uint pcbSize
        );

        [DllImport("User32.dll", SetLastError = true)]
        public static extern uint GetRawInputDeviceInfo(
            IntPtr hDevice,
            uint uiCommand,
            [Out] byte[] pData,
            ref uint pcbSize
        );

        public const int RIDI_DEVICENAME = 0x20000007;
    }
}