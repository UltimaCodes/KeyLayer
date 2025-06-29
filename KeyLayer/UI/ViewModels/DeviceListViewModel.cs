using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using KeyLayer.Core;

namespace KeyLayer.ViewModels
{
    public class DeviceViewModel : INotifyPropertyChanged
    {
        public IntPtr Handle { get; }
        public string CleanName { get; }
        public string RawName { get; }

        private bool _wasPressed;
        public bool WasPressed
        {
            get => _wasPressed;
            set
            {
                if (_wasPressed != value)
                {
                    _wasPressed = value;
                    OnPropertyChanged();
                }
            }
        }

        public DeviceViewModel(DeviceInfo info)
        {
            Handle = info.Handle;
            CleanName = info.CleanName;
            RawName = info.RawName;
        }

        public void Flash()
        {
            WasPressed = true;

            var timer = new System.Timers.Timer(300);
            timer.Elapsed += new ElapsedEventHandler((s, e) =>
            {
                WasPressed = false;
                timer.Stop();
                timer.Dispose();
            });
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class DeviceListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DeviceViewModel> Devices { get; } = new();

        private DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    OnPropertyChanged();
                }
            }
        }

        public void LoadDevices()
        {
            Devices.Clear();
            foreach (var dev in DeviceManager.GetAllKeyboardDevices())
            {
                Devices.Add(new DeviceViewModel(dev));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
