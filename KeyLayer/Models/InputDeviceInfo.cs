namespace KeyLayer.Models;

public class InputDeviceInfo
{
    public string Name { get; set; }
    public string DeviceHandle { get; set; }
    public uint VendorId { get; set; }
    public uint ProductId { get; set; }
    public string DeviceType { get; set; }

    public override string ToString()
    {
        return $"{Name} ({VendorId:X4}:{ProductId:X4})";
    }
}