using System;

namespace KeyLayer
{
    public static class InputTracker
    {
        private static IntPtr _lastDevice = IntPtr.Zero;
        private static DateTime _lastInputTime = DateTime.MinValue;
        private static readonly object _lock = new();

        public static void UpdateLastDevice(IntPtr device)
        {
            lock (_lock)
            {
                _lastDevice = device;
                _lastInputTime = DateTime.Now;
            }
        }

        public static bool TryGetRecentDevice(out IntPtr device)
        {
            lock (_lock)
            {
                if ((DateTime.Now - _lastInputTime).TotalMilliseconds < 80)
                {
                    device = _lastDevice;
                    return true;
                }

                device = IntPtr.Zero;
                return false;
            }
        }
    }
}