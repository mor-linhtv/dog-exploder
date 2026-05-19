namespace Dog_Exploder.Models;

public enum DeviceStatus { Connected, Disconnected, Degraded, Unknown }

public class DeviceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DeviceStatus Status { get; set; } = DeviceStatus.Unknown;
    public string Detail { get; set; } = string.Empty;
    public DateTime CheckedAt { get; set; } = DateTime.Now;
}
