using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KeyLayer.Models;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace KeyLayer.Core;

public class DeviceManager
{
    private readonly IntPtr _hwnd;
    private readonly List<InputDeviceInfo> _devices = new();

    public event Action<string, ushort> OnDeviceKeyPressed;

    public DeviceManager(IntPtr hwnd)
    {
        _hwnd = hwnd;
        RegisterForRawInput();
    }

    public List<InputDeviceInfo> GetAllKeyboardDevices()
    {
        _devices.Clear();

        uint deviceCount = 0;
        GetRawInputDeviceList(null, ref deviceCount, (uint)Marshal.SizeOf<RAWINPUTDEVICELIST>());
        var list = new RAWINPUTDEVICELIST[deviceCount];
        GetRawInputDeviceList(list, ref deviceCount, (uint)Marshal.SizeOf<RAWINPUTDEVICELIST>());

        foreach (var device in list)
        {
            if (device.dwType != RIM_TYPE.RIM_TYPEKEYBOARD)
                continue;

            uint nameSize = 0;
            GetRawInputDeviceInfo(device.hDevice, RIDI.RIDI_DEVICENAME, IntPtr.Zero, ref nameSize);

            IntPtr namePtr = Marshal.AllocHGlobal((int)nameSize);
            GetRawInputDeviceInfo(device.hDevice, RIDI.RIDI_DEVICENAME, namePtr, ref nameSize);
            string deviceName = Marshal.PtrToStringAnsi(namePtr) ?? "Unknown Device";
            Marshal.FreeHGlobal(namePtr);

            _devices.Add(new InputDeviceInfo
            {
                Name = deviceName,
                DeviceHandle = device.hDevice.DangerousGetHandle().ToString(),
                DeviceType = "Keyboard"
            });
        }

        return _devices;
    }

    private void RegisterForRawInput()
    {
        var rid = new RAWINPUTDEVICE[]
        {
            new RAWINPUTDEVICE
            {
                usUsagePage = 0x01,
                usUsage = 0x06, // Keyboard
                dwFlags = RIDEV.RIDEV_INPUTSINK,
                hwndTarget = _hwnd
            }
        };

        if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf<RAWINPUTDEVICE>()))
        {
            throw new Exception("Failed to register for RawInput.");
        }
    }

    public void ProcessRawInput(IntPtr lParam)
    {
        uint dwSize = 0;
        GetRawInputData(lParam, RID.RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());

        IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
        try
        {
            if (GetRawInputData(lParam, RID.RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) != dwSize)
                return;

            var raw = Marshal.PtrToStructure<RAWINPUT>(buffer);
            if (raw.header.dwType != RIM_TYPE.RIM_TYPEKEYBOARD)
                return;

            ushort vkey = raw.data.keyboard.VKey;
            string handle = raw.header.hDevice.DangerousGetHandle().ToString();

            OnDeviceKeyPressed?.Invoke(handle, vkey);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
