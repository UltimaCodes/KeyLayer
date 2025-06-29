using HidSharp;

namespace KeyLayer.core
{
    public static class HidDeviceManager
    {
        private static List<HidDevice> ListAllHidDevices()
        {
            return DeviceList.Local.GetHidDevices().ToList();
        }

        public static void PrintDevices()
        {
            var devices = ListAllHidDevices();
            if (!devices.Any())
            {
                Console.WriteLine("No HID devices found.");
                return;
            }

            for (int i = 0; i < devices.Count; i++)
            {
                var d = devices[i];
                Console.WriteLine($"[{i}] {d.ProductName} - VID_{d.VendorID:X4}&PID_{d.ProductID:X4}");
            }
        }
    }
}