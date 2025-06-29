using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace KeyLayer.Core
{
    public class RawInputHandler
    {
        private HwndSource _hwndSource;

        public event Action<IntPtr, int> OnRawKeyPressed;

        public void Register(IntPtr hwnd, IntPtr targetDeviceHandle)
        {
            _hwndSource = HwndSource.FromHwnd(hwnd);
            _hwndSource.AddHook(WndProc);

            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
            rid[0].UsagePage = 0x01; // Generic desktop controls
            rid[0].Usage = 0x06;     // Keyboard
            rid[0].Flags = 0x00000100; // RIDEV_INPUTSINK
            rid[0].Target = hwnd;

            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
            {
                throw new Exception("Failed to register raw input device.");
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_INPUT = 0x00FF;

            if (msg == WM_INPUT)
            {
                uint dwSize = 0;
                GetRawInputData(lParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());

                if (dwSize == 0)
                    return IntPtr.Zero;

                IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
                try
                {
                    if (GetRawInputData(lParam, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) == dwSize)
                    {
                        RAWINPUT raw = Marshal.PtrToStructure<RAWINPUT>(buffer);

                        if (raw.header.dwType == RIM_TYPEKEYBOARD)
                        {
                            // We only care about key down events
                            if (raw.keyboard.Message == WM_KEYDOWN || raw.keyboard.Message == WM_SYSKEYDOWN)
                            {
                                int vk = raw.keyboard.VKey;
                                IntPtr deviceHandle = raw.header.hDevice;

                                OnRawKeyPressed?.Invoke(deviceHandle, vk);
                            }
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            return IntPtr.Zero;
        }

        #region Constants and Native Interop

        private const int RIM_TYPEKEYBOARD = 1;
        private const int RID_INPUT = 0x10000003;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            public ushort UsagePage;
            public ushort Usage;
            public uint Flags;
            public IntPtr Target;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RAWINPUT
        {
            [FieldOffset(0)] public RAWINPUTHEADER header;
            [FieldOffset(16)] public RAWKEYBOARD keyboard;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices(
            [In] RAWINPUTDEVICE[] pRawInputDevices,
            uint uiNumDevices,
            uint cbSize
        );

        [DllImport("User32.dll")]
        public static extern uint GetRawInputData(
            IntPtr hRawInput,
            uint uiCommand,
            IntPtr pData,
            ref uint pcbSize,
            uint cbSizeHeader
        );

        [DllImport("User32.dll")]
        public static extern uint GetRawInputData(
            IntPtr hRawInput,
            uint uiCommand,
            [Out] byte[] pData,
            ref uint pcbSize,
            uint cbSizeHeader
        );

        #endregion
    }
}
