using KeyLayer.Models;
using System.Runtime.InteropServices;
using System.Text;

namespace KeyLayer.Services;

public class MacroService
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    private const uint MOUSEEVENTF_MOVE = 0x0001;

    private readonly Dictionary<string, DeviceProfile> _deviceProfiles = new();
    private readonly Dictionary<string, int> _activeLayerStates = new();
    private bool _isRecording = false;
    private List<MacroAction> _recordingBuffer = new();
    private DateTime _recordingStartTime;

    public async Task ExecuteMacroAsync(List<MacroAction> actions)
    {
        foreach (var action in actions)
        {
            await ExecuteActionAsync(action);
        }
    }

    public virtual async Task ExecuteActionAsync(MacroAction action)
    {
        switch (action.Type)
        {
            case MacroActionType.KeyPress:
                if (byte.TryParse(action.Value, out byte keyCode))
                {
                    keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    if (action.Duration > 0)
                    {
                        await Task.Delay(action.Duration);
                        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    }
                }
                break;

            case MacroActionType.KeyRelease:
                if (byte.TryParse(action.Value, out byte releaseKey))
                {
                    keybd_event(releaseKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                break;

            case MacroActionType.MouseClick:
                if (action.Value == "left")
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                    await Task.Delay(50);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                }
                else if (action.Value == "right")
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                    await Task.Delay(50);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                }
                break;

            case MacroActionType.MouseMove:
                SetCursorPos(action.X, action.Y);
                break;

            case MacroActionType.Text:
                SendKeys.SendWait(action.Value);
                break;

            case MacroActionType.Delay:
                await Task.Delay(action.Duration);
                break;

            case MacroActionType.Application:
                try
                {
                    System.Diagnostics.Process.Start(action.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start application: {ex.Message}");
                }
                break;
        }
    }

    public void StartRecording()
    {
        _isRecording = true;
        _recordingBuffer.Clear();
        _recordingStartTime = DateTime.Now;
    }

    public List<MacroAction> StopRecording()
    {
        _isRecording = false;
        return _recordingBuffer.ToList();
    }

    public void RecordAction(MacroAction action)
    {
        if (_isRecording)
        {
            action.Timestamp = DateTime.Now;
            _recordingBuffer.Add(action);
        }
    }

    public void SaveDeviceProfile(DeviceProfile profile)
    {
        _deviceProfiles[profile.DevicePath] = profile;
        SaveProfilesToFile();
    }

    public DeviceProfile? GetDeviceProfile(string devicePath)
    {
        return _deviceProfiles.TryGetValue(devicePath, out var profile) ? profile : null;
    }

    public async Task ProcessKeyPress(string devicePath, int keyCode, int modifiers)
    {
        var profile = GetDeviceProfile(devicePath);
        if (profile == null || !profile.IsActive) return;

        // Check if this is a layer key
        if (profile.LayerKeys.Contains(keyCode))
        {
            _activeLayerStates[devicePath] = keyCode;
            return;
        }

        // Get the appropriate key mapping
        if (profile.KeyMappings.TryGetValue(keyCode, out var mapping))
        {
            List<MacroAction> actionsToExecute;

            // Check if we're in a layer state
            if (_activeLayerStates.TryGetValue(devicePath, out var activeLayer) && 
                mapping.LayerActions.TryGetValue(activeLayer, out var layerActions))
            {
                actionsToExecute = layerActions;
            }
            else
            {
                actionsToExecute = mapping.Actions;
            }

            if (actionsToExecute.Count > 0)
            {
                await ExecuteMacroAsync(actionsToExecute);
            }
        }
    }

    public void ProcessKeyRelease(string devicePath, int keyCode)
    {
        var profile = GetDeviceProfile(devicePath);
        if (profile == null || !profile.IsActive) return;

        // Check if this was a layer key being released
        if (profile.LayerKeys.Contains(keyCode))
        {
            _activeLayerStates.Remove(devicePath);
        }
    }

    private void SaveProfilesToFile()
    {
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(_deviceProfiles, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("profiles.json", json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving profiles: {ex.Message}");
        }
    }

    public void LoadProfilesFromFile()
    {
        try
        {
            if (File.Exists("profiles.json"))
            {
                var json = File.ReadAllText("profiles.json");
                var profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DeviceProfile>>(json);
                if (profiles != null)
                {
                    foreach (var kvp in profiles)
                    {
                        _deviceProfiles[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading profiles: {ex.Message}");
        }
    }
}