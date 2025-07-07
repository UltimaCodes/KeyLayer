namespace KeyLayer.Models;

public class DeviceProfile
{
    public string DevicePath { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public Dictionary<int, KeyMapping> KeyMappings { get; set; } = new();
    public List<int> LayerKeys { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}