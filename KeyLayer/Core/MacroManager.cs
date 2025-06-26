using System.Collections.Generic;
using KeyLayer.Models;

namespace KeyLayer.Core;

public class MacroManager
{
    private readonly Dictionary<ushort, MacroAction> _macros = new();
    public string BoundDeviceHandle { get; private set; }

    public void SetActiveDevice(string handle)
    {
        BoundDeviceHandle = handle;
        _macros.Clear(); // reset bindings on new device
    }

    public void BindMacro(ushort keyCode, MacroAction action)
    {
        _macros[keyCode] = action;
    }

    public void HandleKey(string deviceHandle, ushort keyCode)
    {
        if (deviceHandle != BoundDeviceHandle) return;

        if (_macros.TryGetValue(keyCode, out var action))
        {
            action.Execute();
        }
    }
}