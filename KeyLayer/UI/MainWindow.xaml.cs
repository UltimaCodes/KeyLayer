using System;
using System.Linq;
using System.Windows;
using HidSharp;
using KeyLayer.core;

namespace KeyLayer.ui
{
    public partial class MainWindow : Window
    {
        private HidInputManager _manager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDevices();
        }
        private bool IsLikelyKeyboard(HidDevice device)
        {
            try
            {
                var path = device.DevicePath.ToLower();
                if (path.Contains("kbd")) return true; // Windows flags keyboard interfaces with "kbd"

                var report = device.GetReportDescriptor();
                return report.DeviceItems.Any(item =>
                    item.Usages.GetAllValues().Any(u => u == 0x06 || u == 0x07)); // standard usages
            }
            catch
            {
                return false; // Skip inaccessible or broken devices
            }
        }

        private void LoadDevices()
        {
            var allDevices = DeviceList.Local.GetHidDevices().ToList();
            var filteredDevices = allDevices
                .Where(d => !string.IsNullOrWhiteSpace(d.ProductName) && IsLikelyKeyboard(d))
                .GroupBy(d => $"{d.VendorID:X4}:{d.ProductID:X4}:{d.ProductName}") // dedup per physical device
                .Select(g => g.First())
                .ToList();

            DeviceComboBox.ItemsSource = filteredDevices;
            DeviceComboBox.DisplayMemberPath = "ProductName";

            if (filteredDevices.Count == 0)
            {
                Terminal.AppendText("No compatible HID devices found.\n");
            }
            foreach (var device in allDevices)
            {
                Console.WriteLine($"Found: {device.DevicePath} - {device.ProductName}");
            }

        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceComboBox.SelectedItem is not HidDevice selected)
            {
                System.Windows.MessageBox.Show("Please select a device.");
                return;
            }

            Terminal.Clear();

            _manager = new HidInputManager(
                selected,
                onKeyPressed: (key) => Dispatcher.Invoke(() =>
                {
                    Terminal.AppendText($"Key: {key}\n");
                    Terminal.ScrollToEnd();
                }),
                onInterfaceSwitch: (path) => Dispatcher.Invoke(() =>
                {
                    Terminal.AppendText($"→ Switched to interface: {path}\n");
                    Terminal.ScrollToEnd();
                }));

            await _manager.StartAsync();
        }
    }
}