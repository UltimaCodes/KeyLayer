using System;
using System.Windows;
using System.Windows.Interop;
using KeyLayer.Core;
using KeyLayer.ViewModels;

namespace KeyLayer
{
    public partial class MainWindow : Window
    {
        private RawInputHandler _inputHandler;
        private DeviceListViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = (DeviceListViewModel)this.DataContext;
            _viewModel.LoadDevices();

            var hwnd = new WindowInteropHelper(this).Handle;

            _inputHandler = new RawInputHandler();
            _inputHandler.Register(hwnd, IntPtr.Zero); // Listen to ALL initially

            _inputHandler.OnRawKeyPressed += (deviceHandle, vkCode) =>
            {
                Console.WriteLine($"Device: {deviceHandle}, Key: {vkCode}");
                
                var match = FindDeviceByHandle(deviceHandle);
                if (match != null)
                    match.Flash();

                // Handle selected device only
                if (_viewModel.SelectedDevice != null &&
                    _viewModel.SelectedDevice.Handle == deviceHandle)
                {
                    Console.WriteLine($"[KeyLayer] Macro Key: {vkCode}");
                    // TODO: trigger macro here
                }
            };
        }

        private DeviceViewModel FindDeviceByHandle(IntPtr handle)
        {
            foreach (var dev in _viewModel.Devices)
                if (dev.Handle == handle)
                    return dev;
            return null;
        }
    }
}