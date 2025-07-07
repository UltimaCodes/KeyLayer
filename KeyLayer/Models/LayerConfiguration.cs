namespace KeyLayer.Models;

public class LayerConfiguration
{
    public int LayerId { get; set; }
    public string LayerName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LayerActivationType ActivationType { get; set; }
    public List<int> TriggerKeys { get; set; } = new();
    public bool IsToggle { get; set; }
    public int Priority { get; set; }
    public TimeSpan? AutoDeactivateAfter { get; set; }
    public Dictionary<int, KeyMapping> LayerKeyMappings { get; set; } = new();
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string TextColor { get; set; } = "#000000";
    public bool IsActive { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public enum LayerActivationType
{
    Hold,           // Active while key is held
    Toggle,         // Toggle on/off with key press
    Momentary,      // Active for specified duration
    Conditional,    // Active based on conditions
    Sequential      // Activated in sequence
}

public class LayerGroup
{
    public string GroupName { get; set; } = string.Empty;
    public List<LayerConfiguration> Layers { get; set; } = new();
    public bool IsExclusive { get; set; } = true; // Only one layer active at a time
    public int DefaultLayerId { get; set; }
}