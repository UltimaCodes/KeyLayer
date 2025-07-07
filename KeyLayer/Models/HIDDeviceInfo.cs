namespace KeyLayer.Models;

public class HIDDeviceInfo
{
    public string DevicePath { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ManufacturerName { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public int ProductId { get; set; }
    public bool IsKeyboard { get; set; }
    public bool IsIsolated { get; set; }
    public DateTime LastActivity { get; set; }
}