using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace KeyLayer.Core
{
    public class DeviceInfo
    {
        public string RawName { get; set; }     // The full device path (e.g., "\\?\HID#...")
        public string CleanName { get; set; }   // A simplified name for UI display
        public IntPtr Handle { get; set; }      // Device handle
        public RawInput.RawInputDeviceType Type { get; set; }
    }

    public class DeviceManager
    {
        public static List<DeviceInfo> GetAllKeyboardDevices()
        {
            uint deviceCount = 0;
            RawInput.GetRawInputDeviceList(null, ref deviceCount, (uint)Marshal.SizeOf<RawInput.RAWINPUTDEVICELIST>());
            var rawDeviceList = new RawInput.RAWINPUTDEVICELIST[deviceCount];
            RawInput.GetRawInputDeviceList(rawDeviceList, ref deviceCount, (uint)Marshal.SizeOf<RawInput.RAWINPUTDEVICELIST>());

            var devices = new List<DeviceInfo>();

            for (int i = 0; i < deviceCount; i++)
            {
                var rawDevice = rawDeviceList[i];

                if (rawDevice.dwType != (uint)RawInput.RawInputDeviceType.KEYBOARD)
                    continue;

                uint pcbSize = 0;
                RawInput.GetRawInputDeviceInfo(rawDevice.hDevice, RawInput.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);
                if (pcbSize == 0) continue;

                var pData = new byte[pcbSize];
                RawInput.GetRawInputDeviceInfo(rawDevice.hDevice, RawInput.RIDI_DEVICENAME, pData, ref pcbSize);

                string rawName = Encoding.ASCII.GetString(pData).TrimEnd('\0');
                string cleanName = GenerateCleanName(rawName);

                devices.Add(new DeviceInfo
                {
                    RawName = rawName,
                    CleanName = cleanName,
                    Handle = rawDevice.hDevice,
                    Type = RawInput.RawInputDeviceType.KEYBOARD
                });
            }

            return devices;
        }

        private static string GenerateCleanName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName)) return "Unknown Device";

            // Example rawName: \\?\HID#VID_045E&PID_07B2#8&2a5d08c&0&0000#{guid}
            var name = rawName.Replace(@"\\?\HID#", "")
                              .Replace(@"\\?\", "")
                              .Replace("#", " - ")
                              .Replace("{", "")
                              .Replace("}", "")
                              .Replace("ROOT", "RootDevice");

            return name.Length > 80 ? name.Substring(0, 80) + "..." : name;
        }
    }
}
