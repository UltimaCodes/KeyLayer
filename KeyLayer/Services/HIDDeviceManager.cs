using HidSharp;
using KeyLayer.Models;
using System.Runtime.InteropServices;

namespace KeyLayer.Services;

public class HIDDeviceManager
{
    public event EventHandler<HIDDeviceInfo>? DeviceConnected;
    public event EventHandler<HIDDeviceInfo>? DeviceDisconnected;
    public event EventHandler<KeyEventArgs>? KeyPressed;
    
    private readonly List<HIDDeviceInfo> _connectedDevices = new();
    private readonly Dictionary<string, HidStream> _openDevices = new();
    private readonly Dictionary<string, Thread> _deviceThreads = new();
    private bool _isScanning = false;

    public List<HIDDeviceInfo> GetConnectedDevices()
    {
        return _connectedDevices.ToList();
    }

    public void StartScanning()
    {
        if (_isScanning) return;
        
        _isScanning = true;
        ScanForDevices();
        
        // Start periodic scanning
        var scanTimer = new System.Windows.Forms.Timer();
        scanTimer.Interval = 5000; // Scan every 5 seconds
        scanTimer.Tick += (s, e) => ScanForDevices();
        scanTimer.Start();
    }

    private void ScanForDevices()
    {
        try
        {
            var devices = DeviceList.Local.GetHidDevices().ToArray();
            var currentPaths = _connectedDevices.Select(d => d.DevicePath).ToHashSet();
            var foundPaths = new HashSet<string>();

            foreach (var device in devices)
            {
                if (IsKeyboardDevice(device))
                {
                    var deviceInfo = CreateDeviceInfo(device);
                    foundPaths.Add(deviceInfo.DevicePath);

                    if (!currentPaths.Contains(deviceInfo.DevicePath))
                    {
                        _connectedDevices.Add(deviceInfo);
                        DeviceConnected?.Invoke(this, deviceInfo);
                    }
                }
            }

            // Remove disconnected devices
            var disconnectedDevices = _connectedDevices.Where(d => !foundPaths.Contains(d.DevicePath)).ToList();
            foreach (var device in disconnectedDevices)
            {
                _connectedDevices.Remove(device);
                StopDeviceMonitoring(device.DevicePath);
                DeviceDisconnected?.Invoke(this, device);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scanning devices: {ex.Message}");
        }
    }

    private bool IsKeyboardDevice(HidDevice device)
    {
        try
        {
            // Check if device has input reports and is a keyboard
            if (device.GetMaxInputReportLength() <= 0)
                return false;

            // Try to get report descriptor and check for keyboard usage
            var reportDescriptor = device.GetReportDescriptor();
            if (reportDescriptor?.DeviceItems != null)
            {
                foreach (var deviceItem in reportDescriptor.DeviceItems)
                {
                    // Check for Generic Desktop usage page (0x01) and Keyboard usage (0x06)
                    var usages = deviceItem.Usages?.GetAllValues();
                    if (usages != null)
                    {
                        // Look for keyboard usage (0x10006 = usage page 1, usage 6)
                        if (usages.Contains<uint>(0x10006))
                        {
                            return true;
                        }
                    }
                }
            }

            // Fallback: Check VID/PID for known keyboard manufacturers
            return IsKnownKeyboardVendor(device.VendorID, device.ProductID);
        }
        catch
        {
            // If we can't determine the device type, assume it's not a keyboard
            return false;
        }
    }

    private bool IsKnownKeyboardVendor(int vendorId, int productId)
    {
        var keyboardVendors = new HashSet<int>
        {
            0x046D, // Logitech
            0x1532, // Razer
            0x04F2, // Chicony
            0x413C, // Dell
            0x045E, // Microsoft
            0x0458, // KYE Systems (Genius)
            0x1A2C, // China Resource Semico
            0x04CA, // Lite-On
            0x0B05, // ASUS
            0x1B1C, // Corsair
            0x3938, // MOSART Semi
            0x04D9, // Holtek
            0x0C45, // Sonix/Microdia (Redragon boards)
            0x1C4F, // SiGma Micro
            0x0E8F, // GreenAsia
            0x1241, // Belkin
            0x0A5C, // Broadcom
            0x05AC, // Apple
            0x0079, // DragonRise
            0x1D57, // Xenta
            0x04B4, // Cypress
            0x090C, // Silicon Labs
            0x046A, // Cherry
            0x05F3, // Imex Research
            0x0738, // Cosmo
            0x093A, // Pixart
            0x16C0, // Van Ooijen (QMK controllers)
            0x1B4F, // OLKB
            0x1209, // Openmoko
            0x31E3, // Wooting
            0x258A, // Sinowealth (AJAZZ / Redragon keyboards)
            0x13364, // Keychron
            0x13462, // Keyboard.io
            // Chinese OEMs
            0x8013, // Geniatech
            0x10182, // Goodix
            0x8554, // San Jing Electronics
            0xF055,  // FOSS hardware community (Open-source keyboards)
            0x1A86,  // SimPad
            0x8808  // goofy augh chinese brandaa
        };

        // Windows requires hex int literal
        const int VID_A4TECH = 0x09DA; // A4Tech VID
        keyboardVendors.Add(VID_A4TECH);

        return keyboardVendors.Contains(vendorId);
    }
    
    private HIDDeviceInfo CreateDeviceInfo(HidDevice device)
    {
        return new HIDDeviceInfo
        {
            DevicePath = device.DevicePath,
            ProductName = device.GetProductName() ?? "Unknown Device",
            ManufacturerName = device.GetManufacturer() ?? "Unknown Manufacturer",
            VendorId = device.VendorID,
            ProductId = device.ProductID,
            IsKeyboard = true,
            LastActivity = DateTime.Now
        };
    }

    public bool IsolateDevice(string devicePath)
    {
        try
        {
            var deviceInfo = _connectedDevices.FirstOrDefault(d => d.DevicePath == devicePath);
            if (deviceInfo == null) return false;

            var device = DeviceList.Local.GetHidDevices().FirstOrDefault(d => d.DevicePath == devicePath);
            if (device == null) return false;

            // Open the device for exclusive access
            if (device.TryOpen(out var hidStream))
            {
                _openDevices[devicePath] = hidStream;
                StartDeviceMonitoring(devicePath, hidStream);
                deviceInfo.IsIsolated = true;
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error isolating device: {ex.Message}");
        }
        
        return false;
    }

    public void ReleaseDevice(string devicePath)
    {
        if (_openDevices.TryGetValue(devicePath, out var stream))
        {
            stream.Close();
            _openDevices.Remove(devicePath);
            StopDeviceMonitoring(devicePath);
            
            var deviceInfo = _connectedDevices.FirstOrDefault(d => d.DevicePath == devicePath);
            if (deviceInfo != null)
            {
                deviceInfo.IsIsolated = false;
            }
        }
    }

    private void StartDeviceMonitoring(string devicePath, HidStream stream)
    {
        if (_deviceThreads.ContainsKey(devicePath)) return;

        var thread = new Thread(() => MonitorDevice(devicePath, stream))
        {
            IsBackground = true
        };
        
        _deviceThreads[devicePath] = thread;
        thread.Start();
    }

    private void StopDeviceMonitoring(string devicePath)
    {
        if (_deviceThreads.TryGetValue(devicePath, out var thread))
        {
            thread.Interrupt();
            _deviceThreads.Remove(devicePath);
        }
    }

    private void MonitorDevice(string devicePath, HidStream stream)
    {
        try
        {
            var buffer = new byte[stream.Device.GetMaxInputReportLength()];
            
            while (true)
            {
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    ProcessInputReport(devicePath, buffer, bytesRead);
                }
            }
        }
        catch (ThreadInterruptedException)
        {
            // Expected when stopping monitoring
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error monitoring device {devicePath}: {ex.Message}");
        }
    }

    private void ProcessInputReport(string devicePath, byte[] buffer, int length)
    {
        // Parse HID keyboard report
        // Standard keyboard report: [modifier byte][reserved][key1][key2][key3][key4][key5][key6]
        if (length >= 8)
        {
            var modifiers = buffer[0];
            
            // Check for key presses (bytes 2-7 contain key codes)
            for (int i = 2; i < Math.Min(8, length); i++)
            {
                if (buffer[i] != 0)
                {
                    var keyCode = buffer[i];
                    KeyPressed?.Invoke(this, new KeyEventArgs(devicePath, keyCode, modifiers));
                    
                    // Update last activity
                    var deviceInfo = _connectedDevices.FirstOrDefault(d => d.DevicePath == devicePath);
                    if (deviceInfo != null)
                    {
                        deviceInfo.LastActivity = DateTime.Now;
                    }
                }
            }
        }
    }
}

public class KeyEventArgs : EventArgs
{
    public string DevicePath { get; }
    public int KeyCode { get; }
    public int Modifiers { get; }

    public KeyEventArgs(string devicePath, int keyCode, int modifiers)
    {
        DevicePath = devicePath;
        KeyCode = keyCode;
        Modifiers = modifiers;
    }
}