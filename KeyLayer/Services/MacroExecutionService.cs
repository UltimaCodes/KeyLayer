using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeyLayer.Services;

public static class MacroExecutionService
{
    public static void LaunchApp(string path)
    {
        try
        {
            Process.Start(path);
        }
        catch (Exception e)
        {
            Debug.WriteLine("Launch failed: " + e.Message);
        }
    }

    public static void SendKey(VirtualKeyCode key)
    {
        keybd_event((byte)key, 0, 0, 0);
        keybd_event((byte)key, 0, 2, 0);
    }

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
}

public enum VirtualKeyCode : byte
{
    VK_RETURN = 0x0D,
    VK_LWIN = 0x5B,
    VK_R = 0x52,
    // Extend as needed
}