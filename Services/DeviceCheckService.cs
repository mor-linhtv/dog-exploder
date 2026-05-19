using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using Dog_Exploder.Models;

namespace Dog_Exploder.Services;

[SupportedOSPlatform("windows")]
internal static class DeviceCheckService
{
    public static Task<List<DeviceInfo>> EnumerateAsync(CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var list = new List<DeviceInfo>();
            list.AddRange(SafeQuery(QueryCameras,  "Camera"));
            list.AddRange(SafeQuery(QueryBluetooth, "Bluetooth"));
            list.AddRange(SafeQuery(QueryAudio,    "Audio"));
            list.AddRange(SafeQuery(QueryNetwork,  "Network"));
            return list;
        }, ct);

    public static Task<DeviceInfo> RecheckAsync(DeviceInfo device, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var fresh = device.Category switch
            {
                "Camera"    => QueryCameras().FirstOrDefault(d => d.Name == device.Name),
                "Bluetooth" => QueryBluetooth().FirstOrDefault(d => d.Name == device.Name),
                "Audio"     => QueryAudio().FirstOrDefault(d => d.Name == device.Name),
                "Network"   => QueryNetwork().FirstOrDefault(d => d.Name == device.Name),
                _ => null
            };
            return fresh ?? new DeviceInfo
            {
                Name = device.Name,
                Category = device.Category,
                Status = DeviceStatus.Disconnected,
                Detail = "Không tìm thấy thiết bị nữa",
                CheckedAt = DateTime.Now
            };
        }, ct);

    private static IEnumerable<DeviceInfo> SafeQuery(Func<IEnumerable<DeviceInfo>> q, string category)
    {
        try { return q().ToList(); }
        catch (Exception ex)
        {
            return new[] { new DeviceInfo { Name = $"{category} (unavailable)", Category = category, Status = DeviceStatus.Unknown, Detail = ex.Message, CheckedAt = DateTime.Now } };
        }
    }

    private static IEnumerable<DeviceInfo> QueryCameras() =>
        QueryPnp("PNPClass='Camera' OR PNPClass='Image'", "Camera");

    private static IEnumerable<DeviceInfo> QueryBluetooth() =>
        QueryPnp("PNPClass='Bluetooth'", "Bluetooth");

    private static IEnumerable<DeviceInfo> QueryAudio()
    {
        var results = new List<DeviceInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT Name,Status,Manufacturer FROM Win32_SoundDevice");
        foreach (var obj in searcher.Get().Cast<ManagementObject>())
        {
            var name = (obj["Name"] as string) ?? "(unknown audio)";
            var status = (obj["Status"] as string) ?? "Unknown";
            var manu = (obj["Manufacturer"] as string) ?? "";
            results.Add(new DeviceInfo
            {
                Name = name,
                Category = "Audio",
                Status = MapWmiStatus(status, 0),
                Detail = string.IsNullOrEmpty(manu) ? $"Status: {status}" : $"{manu} | Status: {status}",
                CheckedAt = DateTime.Now
            });
        }
        return results;
    }

    private static IEnumerable<DeviceInfo> QueryPnp(string whereClause, string category)
    {
        var results = new List<DeviceInfo>();
        using var searcher = new ManagementObjectSearcher(
            $"SELECT Name,Status,ConfigManagerErrorCode,Manufacturer FROM Win32_PnPEntity WHERE {whereClause}");
        foreach (var obj in searcher.Get().Cast<ManagementObject>())
        {
            var name = (obj["Name"] as string) ?? "(unknown)";
            var status = (obj["Status"] as string) ?? "Unknown";
            var cmErr = Convert.ToInt32(obj["ConfigManagerErrorCode"] ?? 0);
            var manu = (obj["Manufacturer"] as string) ?? "";
            results.Add(new DeviceInfo
            {
                Name = name,
                Category = category,
                Status = MapWmiStatus(status, cmErr),
                Detail = string.IsNullOrEmpty(manu) ? $"Status: {status}, Err: {cmErr}" : $"{manu} | Status: {status}, Err: {cmErr}",
                CheckedAt = DateTime.Now
            });
        }
        return results;
    }

    private static IEnumerable<DeviceInfo> QueryNetwork()
    {
        var results = new List<DeviceInfo>();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Tunnel) continue;

            var ip = nic.GetIPProperties().UnicastAddresses
                        .FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        ?.Address.ToString() ?? "-";
            var mac = string.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));

            results.Add(new DeviceInfo
            {
                Name = nic.Name,
                Category = "Network",
                Status = nic.OperationalStatus == OperationalStatus.Up ? DeviceStatus.Connected : DeviceStatus.Disconnected,
                Detail = $"Type: {nic.NetworkInterfaceType}, IP: {ip}, MAC: {(string.IsNullOrEmpty(mac) ? "-" : mac)}",
                CheckedAt = DateTime.Now
            });
        }
        return results;
    }

    private static DeviceStatus MapWmiStatus(string status, int cmErr) =>
        cmErr > 0                ? DeviceStatus.Disconnected :
        status == "OK"           ? DeviceStatus.Connected   :
        status == "Degraded"     ? DeviceStatus.Degraded    :
        status == "Error"        ? DeviceStatus.Disconnected :
                                   DeviceStatus.Unknown;
}
