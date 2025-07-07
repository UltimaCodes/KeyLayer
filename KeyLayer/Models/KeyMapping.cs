namespace KeyLayer.Models;

public class KeyMapping
{
    public int KeyCode { get; set; }
    public string KeyName { get; set; } = string.Empty;
    public bool IsLayerKey { get; set; }
    public int LayerId { get; set; }
    public List<MacroAction> Actions { get; set; } = new();
    public Dictionary<int, List<MacroAction>> LayerActions { get; set; } = new();
}