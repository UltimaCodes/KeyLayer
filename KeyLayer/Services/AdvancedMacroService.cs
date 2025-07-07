using KeyLayer.Models;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace KeyLayer.Services;

public class AdvancedMacroService : MacroService
{
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll")]
    protected static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("winmm.dll")]
    private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

    [DllImport("winmm.dll")]
    private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private const int SW_HIDE = 0;
    private const int SW_MAXIMIZE = 3;
    private const int SW_MINIMIZE = 6;
    private const int SW_RESTORE = 9;

    private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
    private const uint WM_SYSCOMMAND = 0x0112;
    private static readonly IntPtr SC_MONITORPOWER = new IntPtr(0xF170);

    private readonly Dictionary<string, object> _variables = new();
    private readonly Stack<int> _loopStack = new();

    public override async Task ExecuteActionAsync(MacroAction action)
    {
        if (!action.IsEnabled) return;

        try
        {
            switch (action.Type)
            {
                case MacroActionType.WindowControl:
                    await ExecuteWindowControlAsync(action);
                    break;
                case MacroActionType.SystemCommand:
                    await ExecuteSystemCommandAsync(action);
                    break;
                case MacroActionType.FileOperation:
                    await ExecuteFileOperationAsync(action);
                    break;
                case MacroActionType.WebRequest:
                    await ExecuteWebRequestAsync(action);
                    break;
                case MacroActionType.Conditional:
                    await ExecuteConditionalAsync(action);
                    break;
                case MacroActionType.Loop:
                    await ExecuteLoopAsync(action);
                    break;
                case MacroActionType.Variable:
                    ExecuteVariableAction(action);
                    break;
                case MacroActionType.Clipboard:
                    await ExecuteClipboardActionAsync(action);
                    break;
                case MacroActionType.Screenshot:
                    await ExecuteScreenshotAsync(action);
                    break;
                case MacroActionType.AudioControl:
                    await ExecuteAudioControlAsync(action);
                    break;
                case MacroActionType.MouseScroll:
                    await ExecuteMouseScrollAsync(action);
                    break;
                case MacroActionType.DisplayControl:
                    await ExecuteDisplayControlAsync(action);
                    break;
                default:
                    await base.ExecuteActionAsync(action);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing action {action.Type}: {ex.Message}");
        }
    }

    private async Task ExecuteWindowControlAsync(MacroAction action)
    {
        var windowHandle = FindWindow(null, action.Value);
        if (windowHandle == IntPtr.Zero)
        {
            // Try to find by partial title
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.MainWindowTitle.Contains(action.Value, StringComparison.OrdinalIgnoreCase))
                {
                    windowHandle = process.MainWindowHandle;
                    break;
                }
            }
        }

        if (windowHandle != IntPtr.Zero)
        {
            switch (action.WindowAction)
            {
                case WindowAction.Minimize:
                    ShowWindow(windowHandle, SW_MINIMIZE);
                    break;
                case WindowAction.Maximize:
                    ShowWindow(windowHandle, SW_MAXIMIZE);
                    break;
                case WindowAction.Restore:
                    ShowWindow(windowHandle, SW_RESTORE);
                    break;
                case WindowAction.Close:
                    var process = Process.GetProcesses()
                        .FirstOrDefault(p => p.MainWindowHandle == windowHandle);
                    process?.CloseMainWindow();
                    break;
                case WindowAction.Focus:
                    SetForegroundWindow(windowHandle);
                    break;
                case WindowAction.Move:
                    GetWindowRect(windowHandle, out var rect);
                    MoveWindow(windowHandle, action.X, action.Y, 
                        rect.Right - rect.Left, rect.Bottom - rect.Top, true);
                    break;
                case WindowAction.Resize:
                    GetWindowRect(windowHandle, out var currentRect);
                    MoveWindow(windowHandle, currentRect.Left, currentRect.Top, 
                        action.X, action.Y, true);
                    break;
            }
        }

        await Task.Delay(100); // Small delay for window operations
    }

    private async Task ExecuteSystemCommandAsync(MacroAction action)
    {
        switch (action.SystemCommand)
        {
            case SystemCommandType.Shutdown:
                Process.Start("shutdown", "/s /t 0");
                break;
            case SystemCommandType.Restart:
                Process.Start("shutdown", "/r /t 0");
                break;
            case SystemCommandType.Sleep:
                Process.Start("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");
                break;
            case SystemCommandType.Lock:
                Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
                break;
            case SystemCommandType.Logout:
                Process.Start("shutdown", "/l");
                break;
            case SystemCommandType.VolumeUp:
                AdjustVolume(0.1f);
                break;
            case SystemCommandType.VolumeDown:
                AdjustVolume(-0.1f);
                break;
            case SystemCommandType.VolumeMute:
                ToggleMute();
                break;
            case SystemCommandType.MediaPlay:
                SendKeys.SendWait("{F22}"); // Media play/pause
                break;
            case SystemCommandType.MediaNext:
                SendKeys.SendWait("{F19}"); // Media next
                break;
            case SystemCommandType.MediaPrevious:
                SendKeys.SendWait("{F18}"); // Media previous
                break;
        }

        await Task.Delay(100);
    }

    private async Task ExecuteFileOperationAsync(MacroAction action)
    {
        var sourcePath = action.Value;
        var targetPath = action.Value2;

        switch (action.FileOperation)
        {
            case FileOperationType.Copy:
                if (File.Exists(sourcePath) && !string.IsNullOrEmpty(targetPath))
                {
                    File.Copy(sourcePath, targetPath, true);
                }
                break;
            case FileOperationType.Move:
                if (File.Exists(sourcePath) && !string.IsNullOrEmpty(targetPath))
                {
                    File.Move(sourcePath, targetPath);
                }
                break;
            case FileOperationType.Delete:
                if (File.Exists(sourcePath))
                {
                    File.Delete(sourcePath);
                }
                break;
            case FileOperationType.Create:
                if (!string.IsNullOrEmpty(sourcePath))
                {
                    await File.WriteAllTextAsync(sourcePath, targetPath ?? "");
                }
                break;
            case FileOperationType.Open:
                if (File.Exists(sourcePath))
                {
                    Process.Start(new ProcessStartInfo(sourcePath) { UseShellExecute = true });
                }
                break;
            case FileOperationType.Execute:
                if (File.Exists(sourcePath))
                {
                    Process.Start(sourcePath, targetPath ?? "");
                }
                break;
        }

        await Task.Delay(100);
    }

    private async Task ExecuteWebRequestAsync(MacroAction action)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(action.Duration > 0 ? action.Duration / 1000 : 30);

            HttpResponseMessage response;
            
            if (string.IsNullOrEmpty(action.Value2))
            {
                response = await client.GetAsync(action.Value);
            }
            else
            {
                var content = new StringContent(action.Value2, System.Text.Encoding.UTF8, "application/json");
                response = await client.PostAsync(action.Value, content);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Store response in variable if variable name is specified
            if (!string.IsNullOrEmpty(action.VariableName))
            {
                _variables[action.VariableName] = responseContent;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Web request failed: {ex.Message}");
        }
    }

    private async Task ExecuteConditionalAsync(MacroAction action)
    {
        bool conditionMet = false;

        switch (action.ConditionalType)
        {
            case ConditionalType.WindowExists:
                var windowHandle = FindWindow(null, action.ConditionalValue);
                conditionMet = windowHandle != IntPtr.Zero;
                break;
            case ConditionalType.ProcessRunning:
                conditionMet = Process.GetProcessesByName(action.ConditionalValue).Length > 0;
                break;
            case ConditionalType.FileExists:
                conditionMet = File.Exists(action.ConditionalValue);
                break;
            case ConditionalType.Variable:
                if (_variables.TryGetValue(action.ConditionalValue, out var value))
                {
                    conditionMet = value?.ToString() == action.Value;
                }
                break;
            case ConditionalType.TimeRange:
                var parts = action.ConditionalValue.Split('-');
                if (parts.Length == 2 && 
                    TimeSpan.TryParse(parts[0], out var start) && 
                    TimeSpan.TryParse(parts[1], out var end))
                {
                    var now = DateTime.Now.TimeOfDay;
                    conditionMet = now >= start && now <= end;
                }
                break;
        }

        var actionsToExecute = conditionMet ? action.ConditionalActions : action.ElseActions;
        foreach (var subAction in actionsToExecute)
        {
            await ExecuteActionAsync(subAction);
        }
    }

    private async Task ExecuteLoopAsync(MacroAction action)
    {
        var loopCount = action.LoopCount;
        if (loopCount <= 0) loopCount = 1;

        _loopStack.Push(loopCount);

        try
        {
            for (int i = 0; i < loopCount; i++)
            {
                // Set loop variable
                _variables["LoopIndex"] = i;
                _variables["LoopCount"] = loopCount;

                // Execute loop actions (would need to be defined in the action)
                // This is a simplified implementation
                await Task.Delay(action.Duration);
            }
        }
        finally
        {
            _loopStack.Pop();
        }
    }

    private void ExecuteVariableAction(MacroAction action)
    {
        if (!string.IsNullOrEmpty(action.VariableName))
        {
            var value = action.VariableValue;
            
            // Process variable value for special cases
            if (value.Contains("{DateTime}"))
            {
                value = value.Replace("{DateTime}", DateTime.Now.ToString());
            }
            if (value.Contains("{Random}"))
            {
                value = value.Replace("{Random}", new Random().Next(1000, 9999).ToString());
            }

            _variables[action.VariableName] = value;
        }
    }

    private async Task ExecuteClipboardActionAsync(MacroAction action)
    {
        switch (action.Value.ToLower())
        {
            case "copy":
                SendKeys.SendWait("^c");
                await Task.Delay(100);
                break;
            case "paste":
                SendKeys.SendWait("^v");
                await Task.Delay(100);
                break;
            case "cut":
                SendKeys.SendWait("^x");
                await Task.Delay(100);
                break;
            case "clear":
                Clipboard.Clear();
                break;
            case "set":
                if (!string.IsNullOrEmpty(action.Value2))
                {
                    Clipboard.SetText(action.Value2);
                }
                break;
            case "get":
                if (Clipboard.ContainsText() && !string.IsNullOrEmpty(action.VariableName))
                {
                    _variables[action.VariableName] = Clipboard.GetText();
                }
                break;
        }
    }

    private async Task ExecuteScreenshotAsync(MacroAction action)
    {
        var bounds = Screen.PrimaryScreen.Bounds;
        using var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);
        
        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
        
        var fileName = string.IsNullOrEmpty(action.Value) 
            ? $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png" 
            : action.Value;
            
        bitmap.Save(fileName, ImageFormat.Png);
        
        await Task.Delay(100);
    }

    private async Task ExecuteAudioControlAsync(MacroAction action)
    {
        switch (action.Value.ToLower())
        {
            case "volumeup":
                AdjustVolume(0.1f);
                break;
            case "volumedown":
                AdjustVolume(-0.1f);
                break;
            case "mute":
                ToggleMute();
                break;
            case "setvolume":
                if (float.TryParse(action.Value2, out var volume))
                {
                    SetVolume(volume);
                }
                break;
        }
        
        await Task.Delay(100);
    }

    private async Task ExecuteMouseScrollAsync(MacroAction action)
    {
        var scrollAmount = action.Duration > 0 ? action.Duration : 120;
        var direction = action.Value.ToLower() == "up" ? 1 : -1;
        
        mouse_event(0x0800, 0, 0, (uint)(scrollAmount * direction), UIntPtr.Zero); // MOUSEEVENTF_WHEEL
        
        await Task.Delay(50);
    }

    private async Task ExecuteDisplayControlAsync(MacroAction action)
    {
        switch (action.Value.ToLower())
        {
            case "turnoff":
                SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, new IntPtr(2));
                break;
            case "turnon":
                SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, new IntPtr(-1));
                break;
            case "standby":
                SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, new IntPtr(1));
                break;
        }
        
        await Task.Delay(100);
    }

    private void AdjustVolume(float adjustment)
    {
        waveOutGetVolume(IntPtr.Zero, out uint currentVolume);
        var leftVolume = (ushort)(currentVolume & 0x0000FFFF);
        var rightVolume = (ushort)((currentVolume & 0xFFFF0000) >> 16);
        
        var newLeft = Math.Max(0, Math.Min(65535, leftVolume + (int)(65535 * adjustment)));
        var newRight = Math.Max(0, Math.Min(65535, rightVolume + (int)(65535 * adjustment)));
        
        var newVolume = (uint)((newRight << 16) | newLeft);
        waveOutSetVolume(IntPtr.Zero, newVolume);
    }

    private void SetVolume(float volume)
    {
        var volumeLevel = (ushort)(65535 * Math.Max(0, Math.Min(1, volume)));
        var newVolume = (uint)((volumeLevel << 16) | volumeLevel);
        waveOutSetVolume(IntPtr.Zero, newVolume);
    }

    private void ToggleMute()
    {
        // Simplified mute toggle - in a real implementation you'd track mute state
        waveOutGetVolume(IntPtr.Zero, out uint currentVolume);
        if (currentVolume == 0)
        {
            SetVolume(0.5f); // Restore to 50%
        }
        else
        {
            waveOutSetVolume(IntPtr.Zero, 0); // Mute
        }
    }

    public string ReplaceVariables(string text)
    {
        foreach (var variable in _variables)
        {
            text = text.Replace($"{{{variable.Key}}}", variable.Value?.ToString() ?? "");
        }
        return text;
    }

    public void SetVariable(string name, object value)
    {
        _variables[name] = value;
    }

    public object? GetVariable(string name)
    {
        return _variables.TryGetValue(name, out var value) ? value : null;
    }
}