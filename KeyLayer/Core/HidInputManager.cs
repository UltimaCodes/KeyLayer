using HidSharp;
using KeyLayer.utils;
using System.IO;

namespace KeyLayer.core
{
    public class HidInputManager : IDisposable
    {
        private List<HidDevice> _interfaces;
        private List<Task> _tasks;
        private List<HidStream> _streams;
        private Dictionary<HidDevice, byte[]> _buffers;
        private readonly Action<string> _onKey;
        private readonly Action<string> _onActiveInterfaceChange;
        private CancellationTokenSource _cts;
        private DateTime _lastInputTime;
        private readonly TimeSpan _inactivityThreshold = TimeSpan.FromSeconds(10);
        private HidDevice _activeInterface;
        private bool _isRestarting = false;

        public HidInputManager(HidDevice baseDevice, Action<string> onKeyPressed, Action<string> onInterfaceSwitch)
        {
            _onKey = onKeyPressed;
            _onActiveInterfaceChange = onInterfaceSwitch;
            _interfaces = GetSiblingInterfaces(baseDevice);
            _tasks = new List<Task>();
            _streams = new List<HidStream>();
            _buffers = new Dictionary<HidDevice, byte[]>();
        }

        private List<HidDevice> GetSiblingInterfaces(HidDevice baseDevice)
        {
            string key = $"{baseDevice.VendorID:X4}:{baseDevice.ProductID:X4}:{baseDevice.ProductName}";
            return DeviceList.Local.GetHidDevices()
                .Where(d => $"{d.VendorID:X4}:{d.ProductID:X4}:{d.ProductName}" == key)
                .ToList();
        }

        public async Task StartAsync()
        {
            _cts = new CancellationTokenSource();
            _lastInputTime = DateTime.Now;

            foreach (var device in _interfaces)
            {
                if (device.TryOpen(out HidStream stream))
                {
                    var buffer = new byte[device.GetMaxInputReportLength()];
                    _streams.Add(stream);
                    _buffers[device] = buffer;

                    var task = Task.Run(() => ReadLoop(device, stream, buffer, _cts.Token));
                    _tasks.Add(task);
                }
            }

            // Inactivity watchdog
            _ = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                    if (_isRestarting) continue;

                    if (DateTime.Now - _lastInputTime > _inactivityThreshold)
                    {
                        Console.WriteLine("[INFO] Inactivity timeout reached. Restarting listeners...");
                        _isRestarting = true;
                        System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                        {
                            await RestartListeners();
                        });
                        break;
                    }
                }
            });
        }

        private void ReadLoop(HidDevice device, HidStream stream, byte[] buffer, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    int count = stream.Read(buffer, 0, buffer.Length);
                    if (count > 0)
                    {
                        _lastInputTime = DateTime.Now;

                        if (_activeInterface != device)
                        {
                            _activeInterface = device;
                            _onActiveInterfaceChange?.Invoke(device.DevicePath);
                        }

                        string decoded = KeyDecoder.Decode(buffer);
                        _onKey?.Invoke(decoded);
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine($"[ERROR] Read failed on {device.ProductName}: Stream closed.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Unexpected error on {device.ProductName}: {ex.Message}");
                    break;
                }
            }
        }

        private async Task RestartListeners()
        {
            Dispose();

            await Task.Delay(500); // Allow time for device handles to clear

            _interfaces = GetSiblingInterfaces(_interfaces.FirstOrDefault());
            _tasks.Clear();
            _streams.Clear();
            _buffers.Clear();

            _isRestarting = false;

            await StartAsync();
        }

        public void Dispose()
        {
            try
            {
                _cts?.Cancel();

                foreach (var stream in _streams)
                {
                    try
                    {
                        stream?.Close();
                        stream?.Dispose();
                    }
                    catch { /* Ignore cleanup errors */ }
                }

                _streams.Clear();
                _buffers.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dispose ERROR] {ex.Message}");
            }
        }
    }
}
