using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyLayer.Services;

public class KeyboardHookService
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    
    private LowLevelKeyboardProc _proc = HookCallback;
    private IntPtr _hookID = IntPtr.Zero;
    private static HashSet<string> _isolatedDevices = new();
    private static KeyboardHookService? _instance;

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    public KeyboardHookService()
    {
        _instance = this;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    public void StartHook()
    {
        _hookID = SetHook(_proc);
    }

    public void StopHook()
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
        }
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule!)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName!), 0);
        }
    }

    public static void AddIsolatedDevice(string devicePath)
    {
        _isolatedDevices.Add(devicePath);
    }

    public static void RemoveIsolatedDevice(string devicePath)
    {
        _isolatedDevices.Remove(devicePath);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && _instance != null)
        {
            // For isolated devices, we block the normal keyboard input
            // This is a simplified approach - in reality, you'd need to determine
            // which device the input came from, which requires more complex Windows API calls
            
            if (_isolatedDevices.Count > 0)
            {
                // Check if this input should be blocked
                // This would require additional device identification logic
            }
        }

        return CallNextHookEx(_instance?._hookID ?? IntPtr.Zero, nCode, wParam, lParam);
    }

    ~KeyboardHookService()
    {
        StopHook();
    }
}