namespace KeyLayer.Models;

public enum MacroActionType
{
    KeyPress,
    KeyRelease,
    KeyHold,
    MouseClick,
    MouseMove,
    MouseScroll,
    Delay,
    Text,
    Application,
    WindowControl,
    SystemCommand,
    FileOperation,
    WebRequest,
    Conditional,
    Loop,
    Variable,
    Clipboard,
    Screenshot,
    AudioControl,
    DisplayControl
}

public enum WindowAction
{
    Minimize,
    Maximize,
    Restore,
    Close,
    Focus,
    Move,
    Resize
}

public enum SystemCommandType
{
    Shutdown,
    Restart,
    Sleep,
    Lock,
    Logout,
    VolumeUp,
    VolumeDown,
    VolumeMute,
    MediaPlay,
    MediaPause,
    MediaNext,
    MediaPrevious
}

public enum FileOperationType
{
    Copy,
    Move,
    Delete,
    Create,
    Open,
    Execute
}

public enum ConditionalType
{
    WindowExists,
    ProcessRunning,
    FileExists,
    KeyState,
    TimeRange,
    Variable
}

public class MacroAction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public MacroActionType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Value2 { get; set; } = string.Empty; // For additional parameters
    public int Duration { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string Description { get; set; } = string.Empty;
    
    // Advanced properties
    public WindowAction WindowAction { get; set; }
    public SystemCommandType SystemCommand { get; set; }
    public FileOperationType FileOperation { get; set; }
    public ConditionalType ConditionalType { get; set; }
    public string ConditionalValue { get; set; } = string.Empty;
    public List<MacroAction> ConditionalActions { get; set; } = new();
    public List<MacroAction> ElseActions { get; set; } = new();
    public int LoopCount { get; set; } = 1;
    public string VariableName { get; set; } = string.Empty;
    public string VariableValue { get; set; } = string.Empty;
    
    // Visual editor properties
    public int VisualX { get; set; }
    public int VisualY { get; set; }
    public List<Guid> ConnectedActions { get; set; } = new();
}