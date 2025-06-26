using System;
using System.Windows;
using System.Windows.Interop;
using KeyLayer.Core;
using KeyLayer.Models;
using KeyLayer.Services;

namespace KeyLayer
{
    public partial class MainWindow : Window
    {
        private DeviceManager _deviceManager;
        private MacroManager _macroManager;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                HwndSource.FromHwnd(hwnd).AddHook(WndProc);

                _deviceManager = new DeviceManager(hwnd);
                _macroManager = new MacroManager();

                _deviceManager.OnDeviceKeyPressed += (device, key) =>
                {
                    _macroManager.HandleKey(device, key);
                };

                var devices = _deviceManager.GetAllKeyboardDevices();
                foreach (var dev in devices)
                    Console.WriteLine(dev);

                // 🔽 Simulate selection of device 0 (manual for now)
                if (devices.Count > 0)
                {
                    _macroManager.SetActiveDevice(devices[0].DeviceHandle);

                    // Macro: '1' key launches Notepad
                    _macroManager.BindMacro(0x31, new MacroAction
                    {
                        Name = "Open Notepad",
                        Action = () => MacroExecutionService.LaunchApp("notepad.exe")
                    });
                }
            };
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_INPUT = 0x00FF;
            if (msg == WM_INPUT)
            {
                _deviceManager?.ProcessRawInput(lParam);
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}