# Dog Explorer Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a WinForms Dog Explorer app that lists dog breeds from dogapi.dog (with images from dog.ceo), provides search/filter/detail/refresh, checks device connectivity via WMI, and exports check results to `.xlsx`.

**Architecture:** Single WinForms project (.NET 10 Windows). `LoginForm` → `MainForm` shell with sidebar + content panel. Each pane is a UserControl (`BreedListControl`, `BreedDetailControl`, `DeviceStatusControl`, `ComingSoonControl`) lazily created and swapped into the content panel. Services (`DogApiService`, `DogImageService`, `DeviceCheckService`, `ExcelExportService`) are stateless static helpers. Static `Session` holds username and cached breed list.

**Tech Stack:** C#, .NET 10 WinForms, `System.Net.Http`, `System.Text.Json`, `ClosedXML` 0.104.x, `System.Management` 9.x.

**Spec:** `docs/superpowers/specs/2026-05-18-dog-explorer-design.md`

**TDD note:** Spec explicitly out-of-scopes unit/E2E tests (§10). Verification per task is `dotnet build` (compile-time) + a brief manual smoke described in the step. The final task contains the full end-to-end smoke checklist.

**Working directory:** `C:\Users\ltv\Desktop\WF-demo\Dog-Exploder`. All `git` and `dotnet` commands are run there.

---

## Task 0: Initialize git + cleanup scaffolding

**Files:**
- Delete: `Form1.cs`, `Form1.Designer.cs`, `Form1.resx`
- Create: `.gitignore`

- [ ] **Step 1: Init git repo**

```bash
git init
git checkout -b main
```

Expected: "Initialized empty Git repository" + new branch `main`.

- [ ] **Step 2: Create .gitignore**

Write `C:\Users\ltv\Desktop\WF-demo\Dog-Exploder\.gitignore`:

```gitignore
bin/
obj/
*.user
*.suo
.vs/
*.tmp
```

- [ ] **Step 3: Delete Form1.* files**

```powershell
Remove-Item Form1.cs, Form1.Designer.cs, Form1.resx
```

- [ ] **Step 4: Commit baseline**

```bash
git add .
git commit -m "chore: init git, gitignore, remove default Form1 scaffolding"
```

---

## Task 1: Add NuGet packages + folder scaffold

**Files:**
- Modify: `Dog-Exploder.csproj`
- Create: `Models/`, `Services/`, `Forms/`, `Controls/`, `UI/` folders

- [ ] **Step 1: Overwrite `Dog-Exploder.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <RootNamespace>Dog_Exploder</RootNamespace>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="ClosedXML" Version="0.104.2" />
    <PackageReference Include="System.Management" Version="9.0.0" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Restore packages**

```bash
dotnet restore
```

Expected: "Restored ... Dog-Exploder.csproj" with no errors. Packages downloaded.

- [ ] **Step 3: Create folder scaffold**

```powershell
New-Item -ItemType Directory Models, Services, Forms, Controls, UI -Force
```

- [ ] **Step 4: Verify build still passes (temporary failure expected — Program.cs references deleted Form1)**

Open `Program.cs` and replace its body:

```csharp
namespace Dog_Exploder
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Replaced in Task 17 — for now just print to ensure build passes.
        }
    }
}
```

Run:
```bash
dotnet build
```

Expected: Build succeeded, 0 errors.

- [ ] **Step 5: Commit**

```bash
git add Dog-Exploder.csproj Program.cs Models Services Forms Controls UI
git commit -m "chore: add ClosedXML + System.Management, create folder scaffold"
```

---

## Task 2: Theme + Session

**Files:**
- Create: `UI/Theme.cs`
- Create: `Session.cs`

- [ ] **Step 1: Create `UI/Theme.cs`**

```csharp
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dog_Exploder.UI;

internal static class Theme
{
    public static readonly Color Primary           = Color.FromArgb(0x00, 0x78, 0xD4);
    public static readonly Color PrimaryDark       = Color.FromArgb(0x00, 0x5F, 0xAA);
    public static readonly Color Surface           = Color.FromArgb(0xF9, 0xF9, 0xF9);
    public static readonly Color Canvas            = Color.White;
    public static readonly Color SidebarBg         = Color.FromArgb(0xF3, 0xF3, 0xF3);
    public static readonly Color Border            = Color.FromArgb(0xC0, 0xC7, 0xD4);
    public static readonly Color BorderSoft        = Color.FromArgb(0xD1, 0xD1, 0xD1);
    public static readonly Color TextOnSurface     = Color.FromArgb(0x1A, 0x1C, 0x1C);
    public static readonly Color OnSurfaceVariant  = Color.FromArgb(0x40, 0x47, 0x52);
    public static readonly Color Success           = Color.FromArgb(0x19, 0x87, 0x54);
    public static readonly Color ErrorColor        = Color.FromArgb(0xBA, 0x1A, 0x1A);
    public static readonly Color Warning           = Color.FromArgb(0x97, 0x47, 0x00);
    public static readonly Color BadgeBg           = Color.FromArgb(0xD3, 0xE3, 0xFF);
    public static readonly Color BadgeFg           = Color.FromArgb(0x00, 0x48, 0x83);
    public static readonly Color BadgeDegradedBg   = Color.FromArgb(0xFD, 0xE7, 0xD3);
    public static readonly Color BadgeDegradedFg   = Color.FromArgb(0x97, 0x47, 0x00);
    public static readonly Color BadgeErrorBg      = Color.FromArgb(0xFF, 0xDA, 0xD6);
    public static readonly Color BadgeErrorFg      = Color.FromArgb(0x93, 0x00, 0x0A);
    public static readonly Color HoverBg           = Color.FromArgb(0xE8, 0xE8, 0xE8);
    public static readonly Color ActiveBg          = Color.FromArgb(0xE2, 0xE2, 0xE2);

    public static readonly Font HeadlineLg = new("Segoe UI", 20f, FontStyle.Bold);
    public static readonly Font HeadlineMd = new("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font BodyLg     = new("Segoe UI", 11f, FontStyle.Regular);
    public static readonly Font BodyMd     = new("Segoe UI", 9f,  FontStyle.Regular);
    public static readonly Font LabelLg    = new("Segoe UI", 10f, FontStyle.Bold);
    public static readonly Font LabelMd    = new("Segoe UI", 9f,  FontStyle.Bold);
    public static readonly Font LabelSm    = new("Segoe UI", 8f,  FontStyle.Regular);

    public static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void DrawRoundedBorder(Graphics g, Rectangle r, int radius, Color border, Color? fill = null)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = RoundedRect(new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1), radius);
        if (fill is { } f)
        {
            using var brush = new SolidBrush(f);
            g.FillPath(brush, path);
        }
        using var pen = new Pen(border, 1);
        g.DrawPath(pen, path);
    }
}
```

- [ ] **Step 2: Create `Session.cs`**

```csharp
using Dog_Exploder.Models;

namespace Dog_Exploder;

internal static class Session
{
    public static string Username { get; set; } = string.Empty;
    public static List<Breed>? Breeds { get; set; }
    public static List<Group>? Groups { get; set; }
}
```

- [ ] **Step 3: Build**

```bash
dotnet build
```

Expected: 0 errors. (The `Models.Breed`/`Group` references will fail until Task 3 — if so, leave the file as-is and move on; Task 3 fixes it.)

If build fails due to missing `Models.Breed`, that's expected. Proceed to Task 3 without committing yet.

- [ ] **Step 4: Skip commit until Task 3 completes** (will commit Theme + Session + Models together)

---

## Task 3: Models

**Files:**
- Create: `Models/Breed.cs`, `Models/BreedRange.cs`, `Models/Group.cs`, `Models/DeviceInfo.cs`

- [ ] **Step 1: Create `Models/BreedRange.cs`**

```csharp
namespace Dog_Exploder.Models;

public class BreedRange
{
    public int Min { get; set; }
    public int Max { get; set; }
    public override string ToString() => Min == Max ? Min.ToString() : $"{Min} - {Max}";
}
```

- [ ] **Step 2: Create `Models/Group.cs`**

```csharp
namespace Dog_Exploder.Models;

public class Group
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public override string ToString() => Name;
}
```

- [ ] **Step 3: Create `Models/Breed.cs`**

```csharp
namespace Dog_Exploder.Models;

public class Breed
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BreedRange Life { get; set; } = new();
    public BreedRange MaleWeight { get; set; } = new();
    public BreedRange FemaleWeight { get; set; } = new();
    public BreedRange MaleHeight { get; set; } = new();
    public BreedRange FemaleHeight { get; set; } = new();
    public bool Hypoallergenic { get; set; }
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
}
```

- [ ] **Step 4: Create `Models/DeviceInfo.cs`**

```csharp
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
```

- [ ] **Step 5: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 6: Commit**

```bash
git add UI Session.cs Models
git commit -m "feat: add Theme, Session, and core domain models"
```

---

## Task 4: DogApiService

**Files:**
- Create: `Services/DogApiService.cs`

- [ ] **Step 1: Create `Services/DogApiService.cs`**

```csharp
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dog_Exploder.Models;

namespace Dog_Exploder.Services;

internal static class DogApiService
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("https://dogapi.dog/"),
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static async Task<List<Breed>> GetAllBreedsAsync(CancellationToken ct = default)
    {
        var all = new List<Breed>();
        int page = 1;
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            var url = $"api/v2/breeds?page[number]={page}";
            var resp = await _http.GetFromJsonAsync<JsonApiResponse<BreedDto>>(url, ct)
                       ?? throw new InvalidOperationException("Empty response");
            foreach (var d in resp.Data) all.Add(MapBreed(d));
            int last = resp.Meta?.Pagination?.Last ?? page;
            if (page >= last) break;
            page++;
        }
        return all;
    }

    public static async Task<List<Group>> GetGroupsAsync(CancellationToken ct = default)
    {
        var all = new List<Group>();
        int page = 1;
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            var url = $"api/v2/groups?page[number]={page}";
            var resp = await _http.GetFromJsonAsync<JsonApiResponse<GroupDto>>(url, ct)
                       ?? throw new InvalidOperationException("Empty response");
            foreach (var d in resp.Data)
                all.Add(new Group { Id = d.Id, Name = d.Attributes?.Name ?? "" });
            int last = resp.Meta?.Pagination?.Last ?? page;
            if (page >= last) break;
            page++;
        }
        return all;
    }

    public static void ResolveGroupNames(List<Breed> breeds, List<Group> groups)
    {
        var byId = groups.ToDictionary(g => g.Id, g => g.Name);
        foreach (var b in breeds)
            if (b.GroupId != null && byId.TryGetValue(b.GroupId, out var name))
                b.GroupName = name;
    }

    private static Breed MapBreed(BreedDto d)
    {
        var a = d.Attributes ?? new BreedAttrDto();
        return new Breed
        {
            Id = d.Id,
            Name = a.Name ?? "",
            Description = a.Description ?? "",
            Life = ToRange(a.Life),
            MaleWeight = ToRange(a.MaleWeight),
            FemaleWeight = ToRange(a.FemaleWeight),
            MaleHeight = ToRange(a.MaleHeight),
            FemaleHeight = ToRange(a.FemaleHeight),
            Hypoallergenic = a.Hypoallergenic,
            GroupId = d.Relationships?.Group?.Data?.Id
        };
    }

    private static BreedRange ToRange(RangeDto? r) =>
        r is null ? new BreedRange() : new BreedRange { Min = r.Min, Max = r.Max };

    // --- DTOs matching JSON:API shape (snake_case via JsonPropertyName) ---
    private class JsonApiResponse<T> { [JsonPropertyName("data")] public List<T> Data { get; set; } = new(); [JsonPropertyName("meta")] public MetaDto? Meta { get; set; } }
    private class MetaDto             { [JsonPropertyName("pagination")] public PaginationDto? Pagination { get; set; } }
    private class PaginationDto       { [JsonPropertyName("current")] public int Current { get; set; } [JsonPropertyName("last")] public int Last { get; set; } }
    private class BreedDto            { [JsonPropertyName("id")] public string Id { get; set; } = ""; [JsonPropertyName("attributes")] public BreedAttrDto? Attributes { get; set; } [JsonPropertyName("relationships")] public BreedRelDto? Relationships { get; set; } }
    private class GroupDto            { [JsonPropertyName("id")] public string Id { get; set; } = ""; [JsonPropertyName("attributes")] public GroupAttrDto? Attributes { get; set; } }
    private class GroupAttrDto        { [JsonPropertyName("name")] public string? Name { get; set; } }
    private class BreedAttrDto
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("life")] public RangeDto? Life { get; set; }
        [JsonPropertyName("male_weight")] public RangeDto? MaleWeight { get; set; }
        [JsonPropertyName("female_weight")] public RangeDto? FemaleWeight { get; set; }
        [JsonPropertyName("male_height")] public RangeDto? MaleHeight { get; set; }
        [JsonPropertyName("female_height")] public RangeDto? FemaleHeight { get; set; }
        [JsonPropertyName("hypoallergenic")] public bool Hypoallergenic { get; set; }
    }
    private class RangeDto { [JsonPropertyName("min")] public int Min { get; set; } [JsonPropertyName("max")] public int Max { get; set; } }
    private class BreedRelDto { [JsonPropertyName("group")] public RelLinkDto? Group { get; set; } }
    private class RelLinkDto { [JsonPropertyName("data")] public RelDataDto? Data { get; set; } }
    private class RelDataDto { [JsonPropertyName("id")] public string Id { get; set; } = ""; }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 3: Commit**

```bash
git add Services/DogApiService.cs
git commit -m "feat: add DogApiService for paginated breeds and groups"
```

---

## Task 5: DogImageService

**Files:**
- Create: `Services/DogImageService.cs`

- [ ] **Step 1: Create `Services/DogImageService.cs`**

```csharp
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Dog_Exploder.Services;

internal static class DogImageService
{
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private static readonly Dictionary<string, Image?> _cache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly SemaphoreSlim _lock = new(1, 1);

    public static async Task<Image?> GetImageAsync(string breedName, CancellationToken ct = default)
    {
        var slug = ToSlug(breedName);
        if (string.IsNullOrEmpty(slug)) return null;

        await _lock.WaitAsync(ct);
        try { if (_cache.TryGetValue(slug, out var cached)) return cached; }
        finally { _lock.Release(); }

        Image? img = null;
        try
        {
            var meta = await _http.GetFromJsonAsync<DogCeoResponse>(
                $"https://dog.ceo/api/breed/{slug}/images/random", ct);
            if (meta?.Status == "success" && !string.IsNullOrEmpty(meta.Message))
            {
                using var stream = await _http.GetStreamAsync(meta.Message, ct);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms, ct);
                ms.Position = 0;
                img = Image.FromStream(ms);
            }
        }
        catch { img = null; }

        await _lock.WaitAsync(ct);
        try { _cache[slug] = img; }
        finally { _lock.Release(); }

        return img;
    }

    public static void ClearCache()
    {
        _lock.Wait();
        try
        {
            foreach (var v in _cache.Values) v?.Dispose();
            _cache.Clear();
        }
        finally { _lock.Release(); }
    }

    private static string ToSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        var first = name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return first.ToLowerInvariant();
    }

    private class DogCeoResponse
    {
        [JsonPropertyName("message")] public string? Message { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 3: Commit**

```bash
git add Services/DogImageService.cs
git commit -m "feat: add DogImageService with dog.ceo lookup and cache"
```

---

## Task 6: DeviceCheckService

**Files:**
- Create: `Services/DeviceCheckService.cs`

- [ ] **Step 1: Create `Services/DeviceCheckService.cs`**

```csharp
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
```

- [ ] **Step 2: Build**

```bash
dotnet build
```

Expected: 0 errors. Warnings about `System.Management` Windows-only are OK (project is WinExe Windows).

- [ ] **Step 3: Commit**

```bash
git add Services/DeviceCheckService.cs
git commit -m "feat: add DeviceCheckService for camera/bluetooth/audio/network via WMI"
```

---

## Task 7: ExcelExportService

**Files:**
- Create: `Services/ExcelExportService.cs`

- [ ] **Step 1: Create `Services/ExcelExportService.cs`**

```csharp
using ClosedXML.Excel;
using Dog_Exploder.Models;

namespace Dog_Exploder.Services;

internal static class ExcelExportService
{
    private const string SheetName = "DeviceStatus";
    private static readonly string[] Headers = { "Ngày", "Giờ", "Thiết bị", "Loại", "Trạng thái", "Chi tiết" };

    public static void AppendDeviceStatus(string path, DeviceInfo device)
    {
        XLWorkbook book;
        IXLWorksheet sheet;

        if (File.Exists(path))
        {
            book = new XLWorkbook(path);
            if (!book.Worksheets.TryGetWorksheet(SheetName, out sheet!))
            {
                sheet = book.Worksheets.Add(SheetName);
                WriteHeader(sheet);
            }
        }
        else
        {
            book = new XLWorkbook();
            sheet = book.Worksheets.Add(SheetName);
            WriteHeader(sheet);
        }

        int row = sheet.LastRowUsed()?.RowNumber() + 1 ?? 2;
        if (row < 2) row = 2;
        sheet.Cell(row, 1).Value = device.CheckedAt.ToString("yyyy-MM-dd");
        sheet.Cell(row, 2).Value = device.CheckedAt.ToString("HH:mm:ss");
        sheet.Cell(row, 3).Value = device.Name;
        sheet.Cell(row, 4).Value = device.Category;
        sheet.Cell(row, 5).Value = device.Status.ToString();
        sheet.Cell(row, 6).Value = device.Detail;

        sheet.Columns().AdjustToContents();
        book.SaveAs(path);
        book.Dispose();
    }

    private static void WriteHeader(IXLWorksheet sheet)
    {
        for (int c = 0; c < Headers.Length; c++)
            sheet.Cell(1, c + 1).Value = Headers[c];
        var header = sheet.Range(1, 1, 1, Headers.Length);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromArgb(0xF3, 0xF3, 0xF3);
        header.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 3: Commit**

```bash
git add Services/ExcelExportService.cs
git commit -m "feat: add ExcelExportService append/create for device status xlsx"
```

---

## Task 8: LoginForm

**Files:**
- Create: `Forms/LoginForm.cs`, `Forms/LoginForm.Designer.cs`, `Forms/LoginForm.resx`

- [ ] **Step 1: Create `Forms/LoginForm.resx`**

Use the same standard ResX template as the deleted `Form1.resx` (root + schema + 4 resheaders). Exact content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <resheader name="version"><value>2.0</value></resheader>
  <resheader name="reader"><value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
  <resheader name="writer"><value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
</root>
```

**Note:** the **same .resx body** is reused for every Form/UserControl in the rest of the plan. When a step says "create matching .resx", paste this exact content into the new `.resx` file.

- [ ] **Step 2: Create `Forms/LoginForm.Designer.cs`**

```csharp
namespace Dog_Exploder.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlCard;
    private PictureBox picLogo;
    private Label lblTitle;
    private Label lblSubtitle;
    private Label lblUser;
    private TextBox txtUsername;
    private Label lblPass;
    private TextBox txtPassword;
    private CheckBox chkRemember;
    private LinkLabel lnkForgot;
    private Button btnLogin;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlCard = new Panel();
        picLogo = new PictureBox();
        lblTitle = new Label();
        lblSubtitle = new Label();
        lblUser = new Label();
        txtUsername = new TextBox();
        lblPass = new Label();
        txtPassword = new TextBox();
        chkRemember = new CheckBox();
        lnkForgot = new LinkLabel();
        btnLogin = new Button();

        // pnlCard
        pnlCard.Location = new Point(60, 40);
        pnlCard.Size = new Size(360, 340);
        pnlCard.BackColor = Color.White;
        pnlCard.Paint += PnlCard_Paint;

        // picLogo
        picLogo.Location = new Point(156, 16);
        picLogo.Size = new Size(48, 48);
        picLogo.BackColor = Color.FromArgb(0xE2, 0xE2, 0xE2);
        picLogo.SizeMode = PictureBoxSizeMode.CenterImage;
        picLogo.Paint += PicLogo_Paint;

        // lblTitle
        lblTitle.Text = "Đăng nhập";
        lblTitle.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
        lblTitle.Location = new Point(0, 76);
        lblTitle.Size = new Size(360, 36);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;

        // lblSubtitle
        lblSubtitle.Text = "Chào mừng bạn quay lại với Dog Explorer";
        lblSubtitle.Font = new Font("Segoe UI", 9f);
        lblSubtitle.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblSubtitle.Location = new Point(0, 116);
        lblSubtitle.Size = new Size(360, 20);
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;

        // lblUser
        lblUser.Text = "Tên đăng nhập";
        lblUser.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        lblUser.Location = new Point(20, 150);
        lblUser.Size = new Size(320, 18);

        // txtUsername
        txtUsername.Location = new Point(20, 170);
        txtUsername.Size = new Size(320, 26);
        txtUsername.Font = new Font("Segoe UI", 10f);
        txtUsername.PlaceholderText = "Nhập tên đăng nhập";
        txtUsername.TextChanged += TxtUsername_TextChanged;

        // lblPass
        lblPass.Text = "Mật khẩu";
        lblPass.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        lblPass.Location = new Point(20, 204);
        lblPass.Size = new Size(320, 18);

        // txtPassword
        txtPassword.Location = new Point(20, 224);
        txtPassword.Size = new Size(320, 26);
        txtPassword.Font = new Font("Segoe UI", 10f);
        txtPassword.PlaceholderText = "Nhập mật khẩu";
        txtPassword.UseSystemPasswordChar = true;

        // chkRemember
        chkRemember.Text = "Ghi nhớ đăng nhập";
        chkRemember.Font = new Font("Segoe UI", 9f);
        chkRemember.Location = new Point(20, 260);
        chkRemember.Size = new Size(180, 22);

        // lnkForgot
        lnkForgot.Text = "Quên mật khẩu?";
        lnkForgot.LinkColor = Color.FromArgb(0x00, 0x78, 0xD4);
        lnkForgot.Font = new Font("Segoe UI", 9f);
        lnkForgot.Location = new Point(220, 261);
        lnkForgot.Size = new Size(120, 20);
        lnkForgot.TextAlign = ContentAlignment.MiddleRight;

        // btnLogin
        btnLogin.Text = "Đăng nhập";
        btnLogin.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        btnLogin.Location = new Point(20, 290);
        btnLogin.Size = new Size(320, 36);
        btnLogin.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnLogin.ForeColor = Color.White;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Enabled = false;
        btnLogin.Click += BtnLogin_Click;

        pnlCard.Controls.AddRange(new Control[] { picLogo, lblTitle, lblSubtitle, lblUser, txtUsername, lblPass, txtPassword, chkRemember, lnkForgot, btnLogin });

        // LoginForm
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(480, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Dog Explorer — Đăng nhập";
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
        Controls.Add(pnlCard);
        AcceptButton = btnLogin;
    }
}
```

- [ ] **Step 3: Create `Forms/LoginForm.cs`**

```csharp
using System.Drawing.Drawing2D;

namespace Dog_Exploder.Forms;

public partial class LoginForm : Form
{
    public string Username { get; private set; } = "";

    public LoginForm()
    {
        InitializeComponent();
    }

    private void TxtUsername_TextChanged(object? sender, EventArgs e)
    {
        btnLogin.Enabled = !string.IsNullOrWhiteSpace(txtUsername.Text);
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        Username = txtUsername.Text.Trim();
        if (string.IsNullOrEmpty(Username)) return;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void PnlCard_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
        using var pen = new Pen(Color.FromArgb(0xC0, 0xC7, 0xD4));
        g.DrawRectangle(pen, r);
    }

    private void PicLogo_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(0x00, 0x78, 0xD4));
        // simple paw glyph: 4 toes + heel
        g.FillEllipse(brush, 14, 22, 8, 8);
        g.FillEllipse(brush, 22, 16, 8, 8);
        g.FillEllipse(brush, 30, 16, 8, 8);
        g.FillEllipse(brush, 38, 22, 8, 8);
        g.FillEllipse(brush, 20, 30, 18, 12);
    }
}
```

- [ ] **Step 4: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 5: Commit**

```bash
git add Forms/LoginForm.cs Forms/LoginForm.Designer.cs Forms/LoginForm.resx
git commit -m "feat: add LoginForm with username-only validation"
```

---

## Task 9: ComingSoonControl

**Files:**
- Create: `Controls/ComingSoonControl.cs`, `Controls/ComingSoonControl.Designer.cs`, `Controls/ComingSoonControl.resx`

- [ ] **Step 1: Create `Controls/ComingSoonControl.resx`**

Paste the standard ResX body from Task 8 Step 1.

- [ ] **Step 2: Create `Controls/ComingSoonControl.Designer.cs`**

```csharp
namespace Dog_Exploder.Controls;

partial class ComingSoonControl
{
    private System.ComponentModel.IContainer components = null;
    private Label lblTitle;
    private Label lblSubtitle;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblTitle = new Label();
        lblSubtitle = new Label();

        lblTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblTitle.Text = "Tính năng đang được phát triển";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Height = 40;
        lblTitle.ForeColor = Color.FromArgb(0x1A, 0x1C, 0x1C);

        lblSubtitle.Font = new Font("Segoe UI", 9f);
        lblSubtitle.Text = "Tab này hiện chưa khả dụng trong bản demo.";
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        lblSubtitle.Dock = DockStyle.Top;
        lblSubtitle.Height = 24;
        lblSubtitle.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);

        Controls.Add(lblSubtitle);
        Controls.Add(lblTitle);
        Padding = new Padding(0, 200, 0, 0);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
```

- [ ] **Step 3: Create `Controls/ComingSoonControl.cs`**

```csharp
namespace Dog_Exploder.Controls;

public partial class ComingSoonControl : UserControl
{
    public ComingSoonControl()
    {
        InitializeComponent();
    }

    public void SetTitle(string title) => lblTitle.Text = title;
}
```

- [ ] **Step 4: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 5: Commit**

```bash
git add Controls/ComingSoonControl.*
git commit -m "feat: add ComingSoonControl stub for not-yet-built panes"
```

---

## Task 10: SidebarItem

**Files:**
- Create: `Controls/SidebarItem.cs`, `Controls/SidebarItem.Designer.cs`, `Controls/SidebarItem.resx`

- [ ] **Step 1: Create `Controls/SidebarItem.resx`**

Paste the standard ResX body.

- [ ] **Step 2: Create `Controls/SidebarItem.Designer.cs`**

```csharp
namespace Dog_Exploder.Controls;

partial class SidebarItem
{
    private System.ComponentModel.IContainer components = null;
    private Label lblText;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblText = new Label();

        lblText.Font = new Font("Segoe UI", 9.5f);
        lblText.ForeColor = Color.FromArgb(0x1A, 0x1C, 0x1C);
        lblText.Location = new Point(24, 10);
        lblText.Size = new Size(200, 20);
        lblText.Text = "Item";
        lblText.Cursor = Cursors.Hand;
        lblText.Click += (s, e) => OnClick(EventArgs.Empty);

        Size = new Size(240, 40);
        Cursor = Cursors.Hand;
        BackColor = Color.FromArgb(0xF3, 0xF3, 0xF3);
        Controls.Add(lblText);
        DoubleBuffered = true;
    }
}
```

- [ ] **Step 3: Create `Controls/SidebarItem.cs`**

```csharp
namespace Dog_Exploder.Controls;

public partial class SidebarItem : UserControl
{
    private bool _active;
    private bool _hover;

    public string Key { get; set; } = "";

    public string ItemText
    {
        get => lblText.Text;
        set => lblText.Text = value;
    }

    public bool Active
    {
        get => _active;
        set { _active = value; Invalidate(); }
    }

    public SidebarItem()
    {
        InitializeComponent();
        MouseEnter += (s, e) => { _hover = true; Invalidate(); };
        MouseLeave += (s, e) => { _hover = false; Invalidate(); };
        lblText.MouseEnter += (s, e) => { _hover = true; Invalidate(); };
        lblText.MouseLeave += (s, e) => { _hover = false; Invalidate(); };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var bg = _active ? Color.FromArgb(0xE2, 0xE2, 0xE2)
              : _hover  ? Color.FromArgb(0xE8, 0xE8, 0xE8)
                        : Color.FromArgb(0xF3, 0xF3, 0xF3);
        using (var b = new SolidBrush(bg)) g.FillRectangle(b, ClientRectangle);

        if (_active)
        {
            using var bar = new SolidBrush(Color.FromArgb(0x00, 0x78, 0xD4));
            g.FillRectangle(bar, 0, 6, 3, Height - 12);
        }
        base.OnPaint(e);
    }
}
```

- [ ] **Step 4: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 5: Commit**

```bash
git add Controls/SidebarItem.*
git commit -m "feat: add SidebarItem with active bar and hover state"
```

---

## Task 11: MainForm shell

**Files:**
- Create: `Forms/MainForm.cs`, `Forms/MainForm.Designer.cs`, `Forms/MainForm.resx`

- [ ] **Step 1: Create `Forms/MainForm.resx`**

Paste the standard ResX body.

- [ ] **Step 2: Create `Forms/MainForm.Designer.cs`**

```csharp
using Dog_Exploder.Controls;

namespace Dog_Exploder.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlSidebar;
    private Panel pnlSidebarHeader;
    private Label lblBrandTitle;
    private Label lblBrandSubtitle;
    private Panel pnlSidebarBottom;
    private SidebarItem itemAllBreeds;
    private SidebarItem itemFavorites;
    private SidebarItem itemHistory;
    private SidebarItem itemComparison;
    private SidebarItem itemDevices;
    private SidebarItem itemSettings;
    private SidebarItem itemSupport;
    private Panel pnlContent;
    private Label lblGreeting;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlSidebar = new Panel();
        pnlSidebarHeader = new Panel();
        lblBrandTitle = new Label();
        lblBrandSubtitle = new Label();
        pnlSidebarBottom = new Panel();
        itemAllBreeds = new SidebarItem { Key = "breeds", ItemText = "All Breeds" };
        itemFavorites = new SidebarItem { Key = "favorites", ItemText = "Favorites" };
        itemHistory = new SidebarItem { Key = "history", ItemText = "History" };
        itemComparison = new SidebarItem { Key = "comparison", ItemText = "Comparison" };
        itemDevices = new SidebarItem { Key = "devices", ItemText = "Device Status" };
        itemSettings = new SidebarItem { Key = "settings", ItemText = "Settings" };
        itemSupport = new SidebarItem { Key = "support", ItemText = "Support" };
        pnlContent = new Panel();
        lblGreeting = new Label();

        // pnlSidebar
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Width = 240;
        pnlSidebar.BackColor = Color.FromArgb(0xF3, 0xF3, 0xF3);

        // pnlSidebarHeader
        pnlSidebarHeader.Dock = DockStyle.Top;
        pnlSidebarHeader.Height = 80;
        pnlSidebarHeader.Padding = new Padding(16);

        lblBrandTitle.Text = "Breed Explorer";
        lblBrandTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
        lblBrandTitle.Dock = DockStyle.Top;
        lblBrandTitle.Height = 22;

        lblBrandSubtitle.Text = "WinForms Edition";
        lblBrandSubtitle.Font = new Font("Segoe UI", 8.5f);
        lblBrandSubtitle.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblBrandSubtitle.Dock = DockStyle.Top;
        lblBrandSubtitle.Height = 18;

        pnlSidebarHeader.Controls.Add(lblBrandSubtitle);
        pnlSidebarHeader.Controls.Add(lblBrandTitle);

        // sidebar items stacked top
        itemSupport.Dock = DockStyle.Top;
        itemSettings.Dock = DockStyle.Top;
        pnlSidebarBottom.Dock = DockStyle.Bottom;
        pnlSidebarBottom.Height = 80;
        pnlSidebarBottom.Controls.Add(itemSupport);
        pnlSidebarBottom.Controls.Add(itemSettings);

        itemDevices.Dock = DockStyle.Top;
        itemComparison.Dock = DockStyle.Top;
        itemHistory.Dock = DockStyle.Top;
        itemFavorites.Dock = DockStyle.Top;
        itemAllBreeds.Dock = DockStyle.Top;

        // Dock=Top stacks in REVERSE add order, so add in REVERSE display order:
        pnlSidebar.Controls.Add(itemDevices);
        pnlSidebar.Controls.Add(itemComparison);
        pnlSidebar.Controls.Add(itemHistory);
        pnlSidebar.Controls.Add(itemFavorites);
        pnlSidebar.Controls.Add(itemAllBreeds);
        pnlSidebar.Controls.Add(pnlSidebarHeader);
        pnlSidebar.Controls.Add(pnlSidebarBottom);

        itemAllBreeds.Click  += (s, e) => ShowPane("breeds");
        itemFavorites.Click  += (s, e) => ShowPane("favorites");
        itemHistory.Click    += (s, e) => ShowPane("history");
        itemComparison.Click += (s, e) => ShowPane("comparison");
        itemDevices.Click    += (s, e) => ShowPane("devices");
        itemSettings.Click   += (s, e) => ShowPane("settings");
        itemSupport.Click    += (s, e) => ShowPane("support");

        // pnlContent
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
        pnlContent.Padding = new Padding(24, 48, 24, 24);

        // lblGreeting
        lblGreeting.Dock = DockStyle.Top;
        lblGreeting.Height = 32;
        lblGreeting.TextAlign = ContentAlignment.MiddleRight;
        lblGreeting.Font = new Font("Segoe UI", 9.5f);
        lblGreeting.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);

        // MainForm
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 720);
        MinimumSize = new Size(900, 600);
        Text = "Dog Explorer";
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Controls.Add(pnlContent);
        Controls.Add(pnlSidebar);
        // Note: Add greeting INSIDE pnlContent (Dock=Top) — see MainForm.cs Load
    }
}
```

- [ ] **Step 3: Create `Forms/MainForm.cs`**

```csharp
using Dog_Exploder.Controls;

namespace Dog_Exploder.Forms;

public partial class MainForm : Form
{
    private readonly Dictionary<string, UserControl> _panes = new();
    private SidebarItem[] _items => new[] { itemAllBreeds, itemFavorites, itemHistory, itemComparison, itemDevices, itemSettings, itemSupport };

    public MainForm()
    {
        InitializeComponent();
        // Inject greeting inside pnlContent so it sits ABOVE the active pane.
        pnlContent.Controls.Add(lblGreeting);
        Load += MainForm_Load;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        lblGreeting.Text = $"Hi, {Session.Username} 👋";
        ShowPane("breeds");
    }

    public void ShowPane(string key)
    {
        // Special: detail pane is created on demand via ShowBreedDetail
        if (!_panes.TryGetValue(key, out var ctl))
        {
            ctl = CreatePane(key);
            _panes[key] = ctl;
        }
        ctl.Dock = DockStyle.Fill;

        // Clear pane area (keep greeting)
        var toRemove = pnlContent.Controls.OfType<Control>().Where(c => c != lblGreeting).ToList();
        foreach (var c in toRemove) pnlContent.Controls.Remove(c);
        pnlContent.Controls.Add(ctl);
        ctl.BringToFront();
        lblGreeting.BringToFront();

        // active visual
        foreach (var it in _items)
            it.Active = it.Key == key || (key == "detail" && it.Key == "breeds");
    }

    public void ShowBreedDetail(Models.Breed breed)
    {
        if (!_panes.TryGetValue("detail", out var ctl))
        {
            ctl = new BreedDetailControl();
            ((BreedDetailControl)ctl).BackRequested += (s, e) => ShowPane("breeds");
            _panes["detail"] = ctl;
        }
        ((BreedDetailControl)ctl).SetBreed(breed);
        ShowPane("detail");
    }

    private UserControl CreatePane(string key)
    {
        switch (key)
        {
            case "breeds":
                var list = new BreedListControl();
                list.BreedSelected += (s, b) => ShowBreedDetail(b);
                return list;
            case "devices":
                return new DeviceStatusControl();
            default:
                var stub = new ComingSoonControl();
                stub.SetTitle($"{TitleFor(key)} — đang được phát triển");
                return stub;
        }
    }

    private static string TitleFor(string key) => key switch
    {
        "favorites"  => "Favorites",
        "history"    => "History",
        "comparison" => "Comparison",
        "settings"   => "Settings",
        "support"    => "Support",
        _ => key
    };
}
```

- [ ] **Step 4: Build (expect errors — `BreedListControl`/`BreedDetailControl`/`DeviceStatusControl` not yet defined)**

```bash
dotnet build
```

Expected: 3 errors saying those types are not found. That's correct — they are built in Tasks 13/14/16. Leave commit until Task 16 finishes.

If only those 3 errors, move on. If other errors, stop and fix.

- [ ] **Step 5: Skip commit — MainForm references Tasks 13/14/16 types** (committed together after Task 16)

---

## Task 12: BreedCard

**Files:**
- Create: `Controls/BreedCard.cs`, `Controls/BreedCard.Designer.cs`, `Controls/BreedCard.resx`

- [ ] **Step 1: Create `Controls/BreedCard.resx`**

Paste the standard ResX body.

- [ ] **Step 2: Create `Controls/BreedCard.Designer.cs`**

```csharp
namespace Dog_Exploder.Controls;

partial class BreedCard
{
    private System.ComponentModel.IContainer components = null;
    private PictureBox picImage;
    private Label lblName;
    private Label lblBadge;
    private Label lblDesc;

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _cts.Cancel(); _cts.Dispose(); if (components != null) components.Dispose(); }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        picImage = new PictureBox();
        lblName = new Label();
        lblBadge = new Label();
        lblDesc = new Label();

        picImage.Location = new Point(10, 10);
        picImage.Size = new Size(200, 130);
        picImage.SizeMode = PictureBoxSizeMode.Zoom;
        picImage.BackColor = Color.FromArgb(0xE8, 0xE8, 0xE8);
        picImage.Cursor = Cursors.Hand;
        picImage.Paint += PicImage_Paint;
        picImage.Click += Card_Click;

        lblName.Location = new Point(10, 148);
        lblName.Size = new Size(200, 22);
        lblName.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        lblName.ForeColor = Color.FromArgb(0x1A, 0x1C, 0x1C);
        lblName.Cursor = Cursors.Hand;
        lblName.Click += Card_Click;

        lblBadge.Location = new Point(10, 172);
        lblBadge.Size = new Size(200, 20);
        lblBadge.AutoSize = false;
        lblBadge.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        lblBadge.ForeColor = Color.FromArgb(0x00, 0x48, 0x83);
        lblBadge.TextAlign = ContentAlignment.MiddleLeft;
        lblBadge.Padding = new Padding(0);
        lblBadge.Cursor = Cursors.Hand;
        lblBadge.Click += Card_Click;
        lblBadge.Paint += LblBadge_Paint;

        lblDesc.Location = new Point(10, 196);
        lblDesc.Size = new Size(200, 54);
        lblDesc.Font = new Font("Segoe UI", 8.5f);
        lblDesc.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblDesc.AutoEllipsis = true;
        lblDesc.Cursor = Cursors.Hand;
        lblDesc.Click += Card_Click;

        Size = new Size(220, 260);
        Margin = new Padding(8);
        BackColor = Color.White;
        Cursor = Cursors.Hand;
        DoubleBuffered = true;
        Click += Card_Click;

        Controls.Add(picImage);
        Controls.Add(lblName);
        Controls.Add(lblBadge);
        Controls.Add(lblDesc);
    }
}
```

- [ ] **Step 3: Create `Controls/BreedCard.cs`**

```csharp
using System.Drawing.Drawing2D;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class BreedCard : UserControl
{
    private readonly CancellationTokenSource _cts = new();
    private Breed _breed = null!;

    public event EventHandler<Breed>? BreedSelected;

    public BreedCard()
    {
        InitializeComponent();
    }

    public void Bind(Breed breed)
    {
        _breed = breed;
        lblName.Text = breed.Name;
        lblBadge.Text = "  " + (breed.GroupName ?? "—") + (breed.Hypoallergenic ? "  • Hypoallergenic" : "");
        lblDesc.Text = breed.Description;
        _ = LoadImageAsync();
    }

    private async Task LoadImageAsync()
    {
        try
        {
            var img = await DogImageService.GetImageAsync(_breed.Name, _cts.Token);
            if (IsDisposed || _cts.IsCancellationRequested) return;
            if (img != null && !IsDisposed)
                BeginInvoke(() => { if (!IsDisposed) picImage.Image = img; });
        }
        catch { /* swallow — placeholder remains */ }
    }

    private void PicImage_Paint(object? sender, PaintEventArgs e)
    {
        if (picImage.Image != null) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(0xC0, 0xC7, 0xD4));
        var cx = picImage.Width / 2; var cy = picImage.Height / 2;
        g.FillEllipse(brush, cx - 16, cy - 4, 8, 8);
        g.FillEllipse(brush, cx - 6, cy - 10, 8, 8);
        g.FillEllipse(brush, cx + 4, cy - 10, 8, 8);
        g.FillEllipse(brush, cx + 14, cy - 4, 8, 8);
        g.FillEllipse(brush, cx - 9, cy + 4, 18, 12);
    }

    private void LblBadge_Paint(object? sender, PaintEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(lblBadge.Text)) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var size = TextRenderer.MeasureText(lblBadge.Text, lblBadge.Font);
        var rect = new Rectangle(0, 0, Math.Min(size.Width + 12, lblBadge.Width), lblBadge.Height - 2);
        using var path = UI.Theme.RoundedRect(rect, 4);
        using var bg = new SolidBrush(Color.FromArgb(0xD3, 0xE3, 0xFF));
        g.FillPath(bg, path);
        // Paint event fires AFTER default Label text rendering, so redraw text on top of bg.
        TextRenderer.DrawText(g, lblBadge.Text, lblBadge.Font, rect, lblBadge.ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0xD1, 0xD1, 0xD1));
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = UI.Theme.RoundedRect(r, 8);
        g.DrawPath(pen, path);
    }

    private void Card_Click(object? sender, EventArgs e) => BreedSelected?.Invoke(this, _breed);
}
```

- [ ] **Step 4: Build (will still have Task 11 errors for List/Detail/Device controls — ignore)**

```bash
dotnet build
```

Expected: same 3 unresolved-type errors from Task 11. No new errors.

- [ ] **Step 5: Skip commit** (committed with Task 16)

---

## Task 13: BreedListControl

**Files:**
- Create: `Controls/BreedListControl.cs`, `Controls/BreedListControl.Designer.cs`, `Controls/BreedListControl.resx`

- [ ] **Step 1: Create `Controls/BreedListControl.resx`**

Paste the standard ResX body.

- [ ] **Step 2: Create `Controls/BreedListControl.Designer.cs`**

```csharp
namespace Dog_Exploder.Controls;

partial class BreedListControl
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlTop;
    private Label lblTitle;
    private TextBox txtSearch;
    private ComboBox cboGroup;
    private Button btnRefresh;
    private FlowLayoutPanel pnlGrid;
    private Panel pnlState;
    private Label lblState;
    private Button btnRetry;
    private System.Windows.Forms.Timer searchDebounce;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlTop = new Panel();
        lblTitle = new Label();
        txtSearch = new TextBox();
        cboGroup = new ComboBox();
        btnRefresh = new Button();
        pnlGrid = new FlowLayoutPanel();
        pnlState = new Panel();
        lblState = new Label();
        btnRetry = new Button();
        searchDebounce = new System.Windows.Forms.Timer(components);

        pnlTop.Dock = DockStyle.Top;
        pnlTop.Height = 60;

        lblTitle.Text = "Dog Breed Index";
        lblTitle.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
        lblTitle.Location = new Point(0, 10);
        lblTitle.Size = new Size(300, 36);

        txtSearch.PlaceholderText = "Search breeds…";
        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Location = new Point(320, 18);
        txtSearch.Size = new Size(240, 26);
        txtSearch.TextChanged += TxtSearch_TextChanged;

        cboGroup.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGroup.Font = new Font("Segoe UI", 9.5f);
        cboGroup.Location = new Point(570, 18);
        cboGroup.Size = new Size(180, 26);
        cboGroup.SelectedIndexChanged += CboGroup_SelectedIndexChanged;

        btnRefresh.Text = "↻ Refresh";
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnRefresh.BackColor = Color.White;
        btnRefresh.Location = new Point(760, 17);
        btnRefresh.Size = new Size(100, 28);
        btnRefresh.Click += BtnRefresh_Click;

        pnlTop.Controls.Add(lblTitle);
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(cboGroup);
        pnlTop.Controls.Add(btnRefresh);

        pnlGrid.Dock = DockStyle.Fill;
        pnlGrid.AutoScroll = true;
        pnlGrid.BackColor = Color.White;
        pnlGrid.FlowDirection = FlowDirection.LeftToRight;
        pnlGrid.WrapContents = true;
        pnlGrid.Padding = new Padding(8);

        pnlState.Dock = DockStyle.Fill;
        pnlState.BackColor = Color.White;
        pnlState.Visible = false;

        lblState.Text = "Đang tải...";
        lblState.Font = new Font("Segoe UI", 11f);
        lblState.Dock = DockStyle.Top;
        lblState.Height = 40;
        lblState.TextAlign = ContentAlignment.MiddleCenter;
        lblState.Padding = new Padding(0, 100, 0, 0);

        btnRetry.Text = "Thử lại";
        btnRetry.Visible = false;
        btnRetry.Size = new Size(120, 32);
        btnRetry.Anchor = AnchorStyles.Top;
        btnRetry.Top = 160;
        btnRetry.Left = 0; // centered later
        btnRetry.FlatStyle = FlatStyle.Flat;
        btnRetry.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnRetry.ForeColor = Color.White;
        btnRetry.FlatAppearance.BorderSize = 0;
        btnRetry.Click += BtnRetry_Click;

        pnlState.Controls.Add(btnRetry);
        pnlState.Controls.Add(lblState);
        pnlState.Resize += (s, e) => { btnRetry.Left = (pnlState.Width - btnRetry.Width) / 2; };

        searchDebounce.Interval = 300;
        searchDebounce.Tick += (s, e) => { searchDebounce.Stop(); RenderCards(); };

        Controls.Add(pnlGrid);
        Controls.Add(pnlState);
        Controls.Add(pnlTop);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
```

- [ ] **Step 3: Create `Controls/BreedListControl.cs`**

```csharp
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class BreedListControl : UserControl
{
    public event EventHandler<Breed>? BreedSelected;

    public BreedListControl()
    {
        InitializeComponent();
        Load += async (s, e) => await EnsureLoadedAsync();
    }

    private async Task EnsureLoadedAsync()
    {
        if (Session.Breeds == null)
            await LoadAsync();
        else
        {
            PopulateGroups();
            RenderCards();
        }
    }

    private async Task LoadAsync()
    {
        ShowState("Đang tải dữ liệu...", showRetry: false);
        try
        {
            var breeds = await DogApiService.GetAllBreedsAsync();
            var groups = await DogApiService.GetGroupsAsync();
            DogApiService.ResolveGroupNames(breeds, groups);
            Session.Breeds = breeds;
            Session.Groups = groups;
            HideState();
            PopulateGroups();
            RenderCards();
        }
        catch (Exception ex)
        {
            ShowState($"Không tải được dữ liệu: {ex.Message}", showRetry: true);
        }
    }

    private void PopulateGroups()
    {
        cboGroup.Items.Clear();
        cboGroup.Items.Add("All Groups");
        if (Session.Groups != null)
            foreach (var g in Session.Groups.OrderBy(x => x.Name))
                cboGroup.Items.Add(g);
        cboGroup.SelectedIndex = 0;
    }

    private void RenderCards()
    {
        if (Session.Breeds == null) return;
        var query = txtSearch.Text.Trim();
        var group = cboGroup.SelectedItem as Group;

        var filtered = Session.Breeds
            .Where(b => string.IsNullOrEmpty(query) ||
                        b.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Where(b => group == null || b.GroupId == group.Id)
            .Take(200)
            .ToList();

        pnlGrid.SuspendLayout();
        foreach (Control c in pnlGrid.Controls) c.Dispose();
        pnlGrid.Controls.Clear();
        foreach (var b in filtered)
        {
            var card = new BreedCard();
            card.Bind(b);
            card.BreedSelected += (s, breed) => BreedSelected?.Invoke(this, breed);
            pnlGrid.Controls.Add(card);
        }
        pnlGrid.ResumeLayout();
    }

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        searchDebounce.Stop();
        searchDebounce.Start();
    }

    private void CboGroup_SelectedIndexChanged(object? sender, EventArgs e) => RenderCards();

    private async void BtnRefresh_Click(object? sender, EventArgs e)
    {
        Session.Breeds = null;
        Session.Groups = null;
        DogImageService.ClearCache();
        await LoadAsync();
    }

    private async void BtnRetry_Click(object? sender, EventArgs e) => await LoadAsync();

    private void ShowState(string text, bool showRetry)
    {
        lblState.Text = text;
        btnRetry.Visible = showRetry;
        pnlState.Visible = true;
        pnlState.BringToFront();
    }

    private void HideState()
    {
        pnlState.Visible = false;
        pnlGrid.BringToFront();
    }
}
```

- [ ] **Step 4: Build (Task 11 + 14 + 16 type errors only — should drop from 3 to 2)**

```bash
dotnet build
```

Expected: 2 errors (`BreedDetailControl`, `DeviceStatusControl` still missing). No new errors.

- [ ] **Step 5: Skip commit** (committed with Task 16)

---

## Task 14: BreedDetailControl

**Files:**
- Create: `Controls/BreedDetailControl.cs`, `Controls/BreedDetailControl.Designer.cs`, `Controls/BreedDetailControl.resx`

- [ ] **Step 1: Create `Controls/BreedDetailControl.resx`**

Paste the standard ResX body.

- [ ] **Step 2: Create `Controls/BreedDetailControl.Designer.cs`**

```csharp
namespace Dog_Exploder.Controls;

partial class BreedDetailControl
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlTopBar;
    private Button btnBack;
    private Button btnEdit;
    private Panel pnlHeader;
    private PictureBox picImage;
    private Label lblName;
    private Label lblDescription;
    private Label lblGroupBadge;
    private Panel pnlSpecs;
    private Label lblSpecsTitle;
    private DataGridView dgvSpecs;
    private Panel pnlCare;
    private Label lblCareTitle;
    private Label lblCareBody;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlTopBar = new Panel();
        btnBack = new Button();
        btnEdit = new Button();
        pnlHeader = new Panel();
        picImage = new PictureBox();
        lblName = new Label();
        lblDescription = new Label();
        lblGroupBadge = new Label();
        pnlSpecs = new Panel();
        lblSpecsTitle = new Label();
        dgvSpecs = new DataGridView();
        pnlCare = new Panel();
        lblCareTitle = new Label();
        lblCareBody = new Label();

        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Height = 50;

        btnBack.Text = "← Back to List";
        btnBack.Location = new Point(0, 10);
        btnBack.Size = new Size(130, 30);
        btnBack.FlatStyle = FlatStyle.Flat;
        btnBack.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnBack.BackColor = Color.White;
        btnBack.Click += (s, e) => BackRequested?.Invoke(this, EventArgs.Empty);

        btnEdit.Text = "Edit Record";
        btnEdit.Location = new Point(660, 10);
        btnEdit.Size = new Size(110, 30);
        btnEdit.FlatStyle = FlatStyle.Flat;
        btnEdit.BackColor = Color.White;
        btnEdit.Enabled = false;

        pnlTopBar.Controls.Add(btnBack);
        pnlTopBar.Controls.Add(btnEdit);

        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Height = 220;
        pnlHeader.BackColor = Color.White;
        pnlHeader.Padding = new Padding(16);
        pnlHeader.Paint += Bordered_Paint;

        picImage.Location = new Point(16, 16);
        picImage.Size = new Size(280, 180);
        picImage.SizeMode = PictureBoxSizeMode.Zoom;
        picImage.BackColor = Color.FromArgb(0xE8, 0xE8, 0xE8);
        picImage.Paint += PicImage_Paint;

        lblName.Location = new Point(320, 16);
        lblName.Size = new Size(500, 40);
        lblName.Font = new Font("Segoe UI", 20f, FontStyle.Bold);

        lblDescription.Location = new Point(320, 60);
        lblDescription.Size = new Size(500, 100);
        lblDescription.Font = new Font("Segoe UI", 10f);
        lblDescription.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblDescription.AutoEllipsis = true;

        lblGroupBadge.Location = new Point(320, 168);
        lblGroupBadge.Size = new Size(220, 24);
        lblGroupBadge.Padding = new Padding(8, 4, 8, 4);
        lblGroupBadge.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        lblGroupBadge.ForeColor = Color.FromArgb(0x00, 0x48, 0x83);
        lblGroupBadge.Paint += LblGroupBadge_Paint;

        pnlHeader.Controls.Add(picImage);
        pnlHeader.Controls.Add(lblName);
        pnlHeader.Controls.Add(lblDescription);
        pnlHeader.Controls.Add(lblGroupBadge);

        pnlSpecs.Location = new Point(0, 280);
        pnlSpecs.Size = new Size(440, 280);
        pnlSpecs.BackColor = Color.White;
        pnlSpecs.Padding = new Padding(16);
        pnlSpecs.Paint += Bordered_Paint;

        lblSpecsTitle.Text = "Breed Specifications";
        lblSpecsTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
        lblSpecsTitle.Dock = DockStyle.Top;
        lblSpecsTitle.Height = 30;

        dgvSpecs.Dock = DockStyle.Fill;
        dgvSpecs.AllowUserToAddRows = false;
        dgvSpecs.AllowUserToDeleteRows = false;
        dgvSpecs.ReadOnly = true;
        dgvSpecs.RowHeadersVisible = false;
        dgvSpecs.ColumnHeadersVisible = false;
        dgvSpecs.BackgroundColor = Color.White;
        dgvSpecs.BorderStyle = BorderStyle.None;
        dgvSpecs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSpecs.Columns.Add("Key", "");
        dgvSpecs.Columns.Add("Val", "");
        dgvSpecs.Columns[0].Width = 160;
        dgvSpecs.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvSpecs.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
        dgvSpecs.GridColor = Color.FromArgb(0xE2, 0xE2, 0xE2);

        pnlSpecs.Controls.Add(dgvSpecs);
        pnlSpecs.Controls.Add(lblSpecsTitle);

        pnlCare.Location = new Point(456, 280);
        pnlCare.Size = new Size(440, 280);
        pnlCare.BackColor = Color.White;
        pnlCare.Padding = new Padding(16);
        pnlCare.Paint += Bordered_Paint;

        lblCareTitle.Text = "Temperament & Care";
        lblCareTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
        lblCareTitle.Dock = DockStyle.Top;
        lblCareTitle.Height = 30;

        lblCareBody.Text = "• Trainability: N/A\n• Energy: N/A\n• Shedding: N/A\n\n(API không cung cấp thông tin này.)";
        lblCareBody.Font = new Font("Segoe UI", 9.5f);
        lblCareBody.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblCareBody.Dock = DockStyle.Fill;

        pnlCare.Controls.Add(lblCareBody);
        pnlCare.Controls.Add(lblCareTitle);

        Controls.Add(pnlSpecs);
        Controls.Add(pnlCare);
        Controls.Add(pnlHeader);
        Controls.Add(pnlTopBar);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
```

- [ ] **Step 3: Create `Controls/BreedDetailControl.cs`**

```csharp
using System.Drawing.Drawing2D;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class BreedDetailControl : UserControl
{
    private CancellationTokenSource? _imageCts;

    public event EventHandler? BackRequested;

    public BreedDetailControl()
    {
        InitializeComponent();
    }

    public void SetBreed(Breed breed)
    {
        lblName.Text = breed.Name;
        lblDescription.Text = breed.Description;
        lblGroupBadge.Text = string.IsNullOrEmpty(breed.GroupName) ? "—" : breed.GroupName;

        dgvSpecs.Rows.Clear();
        dgvSpecs.Rows.Add("Life Expectancy", $"{breed.Life} years");
        dgvSpecs.Rows.Add("Weight (Male)", $"{breed.MaleWeight} lb");
        dgvSpecs.Rows.Add("Weight (Female)", $"{breed.FemaleWeight} lb");
        dgvSpecs.Rows.Add("Height (Male)", $"{breed.MaleHeight} in");
        dgvSpecs.Rows.Add("Height (Female)", $"{breed.FemaleHeight} in");
        dgvSpecs.Rows.Add("Hypoallergenic", breed.Hypoallergenic ? "Yes" : "No");

        picImage.Image?.Dispose();
        picImage.Image = null;
        _imageCts?.Cancel();
        _imageCts = new CancellationTokenSource();
        _ = LoadImageAsync(breed.Name, _imageCts.Token);
    }

    private async Task LoadImageAsync(string name, CancellationToken ct)
    {
        try
        {
            var img = await DogImageService.GetImageAsync(name, ct);
            if (ct.IsCancellationRequested || IsDisposed) return;
            if (img != null) BeginInvoke(() => { if (!IsDisposed) picImage.Image = img; });
        }
        catch { }
    }

    private void Bordered_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Control c) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0xD1, 0xD1, 0xD1));
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, c.Width - 1, c.Height - 1), 8);
        g.DrawPath(pen, path);
    }

    private void PicImage_Paint(object? sender, PaintEventArgs e)
    {
        if (picImage.Image != null) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(0xC0, 0xC7, 0xD4));
        var cx = picImage.Width / 2; var cy = picImage.Height / 2;
        g.FillEllipse(brush, cx - 22, cy - 6, 10, 10);
        g.FillEllipse(brush, cx - 10, cy - 14, 10, 10);
        g.FillEllipse(brush, cx + 2,  cy - 14, 10, 10);
        g.FillEllipse(brush, cx + 14, cy - 6, 10, 10);
        g.FillEllipse(brush, cx - 12, cy + 6, 22, 14);
    }

    private void LblGroupBadge_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, lblGroupBadge.Width - 1, lblGroupBadge.Height - 1), 4);
        using var bg = new SolidBrush(Color.FromArgb(0xD3, 0xE3, 0xFF));
        g.FillPath(bg, path);
    }
}
```

- [ ] **Step 4: Build (1 error left: `DeviceStatusControl`)**

```bash
dotnet build
```

Expected: 1 error. No new errors.

- [ ] **Step 5: Skip commit** (committed with Task 16)

---

## Task 15: DeviceCard

**Files:**
- Create: `Controls/DeviceCard.cs`, `Controls/DeviceCard.Designer.cs`, `Controls/DeviceCard.resx`

- [ ] **Step 1: Create `Controls/DeviceCard.resx`**

Paste the standard ResX body.

- [ ] **Step 2: Create `Controls/DeviceCard.Designer.cs`**

```csharp
namespace Dog_Exploder.Controls;

partial class DeviceCard
{
    private System.ComponentModel.IContainer components = null;
    private Label lblName;
    private Label lblBadge;
    private Label lblCategory;
    private Label lblDetail;
    private Label lblChecked;
    private RadioButton rbSelect;
    private Button btnCheck;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblName = new Label();
        lblBadge = new Label();
        lblCategory = new Label();
        lblDetail = new Label();
        lblChecked = new Label();
        rbSelect = new RadioButton();
        btnCheck = new Button();

        lblName.Location = new Point(16, 14);
        lblName.Size = new Size(230, 22);
        lblName.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        lblName.AutoEllipsis = true;

        lblBadge.Location = new Point(252, 14);
        lblBadge.AutoSize = false;
        lblBadge.Size = new Size(100, 22);
        lblBadge.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        lblBadge.TextAlign = ContentAlignment.MiddleCenter;
        lblBadge.Padding = new Padding(8, 2, 8, 2);
        lblBadge.Paint += LblBadge_Paint;

        lblCategory.Location = new Point(16, 44);
        lblCategory.Size = new Size(330, 18);
        lblCategory.Font = new Font("Segoe UI", 9f);
        lblCategory.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);

        lblDetail.Location = new Point(16, 66);
        lblDetail.Size = new Size(330, 80);
        lblDetail.Font = new Font("Segoe UI", 9f);
        lblDetail.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblDetail.AutoEllipsis = true;

        lblChecked.Location = new Point(16, 150);
        lblChecked.Size = new Size(330, 18);
        lblChecked.Font = new Font("Segoe UI", 8.5f);
        lblChecked.ForeColor = Color.FromArgb(0x71, 0x77, 0x83);

        rbSelect.Text = "Chọn để export";
        rbSelect.Location = new Point(16, 178);
        rbSelect.Size = new Size(160, 22);
        rbSelect.Font = new Font("Segoe UI", 9f);

        btnCheck.Text = "Check Connection";
        btnCheck.Location = new Point(196, 175);
        btnCheck.Size = new Size(150, 28);
        btnCheck.FlatStyle = FlatStyle.Flat;
        btnCheck.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnCheck.BackColor = Color.White;
        btnCheck.Click += BtnCheck_Click;

        Size = new Size(360, 220);
        Margin = new Padding(8);
        BackColor = Color.White;
        DoubleBuffered = true;

        Controls.Add(lblName);
        Controls.Add(lblBadge);
        Controls.Add(lblCategory);
        Controls.Add(lblDetail);
        Controls.Add(lblChecked);
        Controls.Add(rbSelect);
        Controls.Add(btnCheck);
    }
}
```

- [ ] **Step 3: Create `Controls/DeviceCard.cs`**

```csharp
using System.Drawing.Drawing2D;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class DeviceCard : UserControl
{
    public DeviceInfo Device { get; private set; } = new();

    public bool IsSelected => rbSelect.Checked;

    public event EventHandler<DeviceInfo>? Updated;

    public DeviceCard()
    {
        InitializeComponent();
    }

    public void Bind(DeviceInfo device)
    {
        Device = device;
        lblName.Text = device.Name;
        lblCategory.Text = device.Category;
        lblDetail.Text = device.Detail;
        lblChecked.Text = $"Last checked: {device.CheckedAt:HH:mm:ss}";
        lblBadge.Text = device.Status.ToString();
        Invalidate();
    }

    public void SetSelectionGroup(string groupName)
    {
        // RadioButton group by container — RadioButton is auto-grouped by parent.
        // No-op here; provided for future flexibility.
    }

    public void ClearSelection() => rbSelect.Checked = false;

    private async void BtnCheck_Click(object? sender, EventArgs e)
    {
        btnCheck.Enabled = false;
        try
        {
            var fresh = await DeviceCheckService.RecheckAsync(Device);
            Bind(fresh);
            Updated?.Invoke(this, fresh);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không check được: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnCheck.Enabled = true;
        }
    }

    private void LblBadge_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var (bg, fg) = Device.Status switch
        {
            DeviceStatus.Connected    => (Color.FromArgb(0xCF, 0xE2, 0xFF), Color.FromArgb(0x00, 0x3F, 0x6D)),
            DeviceStatus.Degraded     => (Color.FromArgb(0xFD, 0xE7, 0xD3), Color.FromArgb(0x97, 0x47, 0x00)),
            DeviceStatus.Disconnected => (Color.FromArgb(0xFF, 0xDA, 0xD6), Color.FromArgb(0x93, 0x00, 0x0A)),
            _                          => (Color.FromArgb(0xE8, 0xE8, 0xE8), Color.FromArgb(0x40, 0x47, 0x52)),
        };
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, lblBadge.Width - 1, lblBadge.Height - 1), 4);
        using var bgBrush = new SolidBrush(bg);
        g.FillPath(bgBrush, path);
        // Paint event runs AFTER Label.OnPaint, so default text was drawn but is now hidden by bg;
        // redraw it on top in the badge's foreground color.
        TextRenderer.DrawText(g, lblBadge.Text, lblBadge.Font, new Rectangle(0, 0, lblBadge.Width, lblBadge.Height), fg,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0xD1, 0xD1, 0xD1));
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 8);
        g.DrawPath(pen, path);
    }
}
```


- [ ] **Step 4: Build (still 1 error: `DeviceStatusControl`)**

```bash
dotnet build
```

Expected: 1 error.

- [ ] **Step 5: Skip commit** (committed with Task 16)

---

## Task 16: DeviceStatusControl

**Files:**
- Create: `Controls/DeviceStatusControl.cs`, `Controls/DeviceStatusControl.Designer.cs`, `Controls/DeviceStatusControl.resx`

- [ ] **Step 1: Create `Controls/DeviceStatusControl.resx`**

Paste the standard ResX body.

- [ ] **Step 2: Create `Controls/DeviceStatusControl.Designer.cs`**

```csharp
namespace Dog_Exploder.Controls;

partial class DeviceStatusControl
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlTop;
    private Button btnRefreshAll;
    private Button btnAddDevice;
    private Label lblLastUpdated;
    private Panel pnlBottom;
    private Button btnExportSelected;
    private FlowLayoutPanel pnlDevices;
    private Panel pnlState;
    private Label lblState;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlTop = new Panel();
        btnRefreshAll = new Button();
        btnAddDevice = new Button();
        lblLastUpdated = new Label();
        pnlBottom = new Panel();
        btnExportSelected = new Button();
        pnlDevices = new FlowLayoutPanel();
        pnlState = new Panel();
        lblState = new Label();

        pnlTop.Dock = DockStyle.Top;
        pnlTop.Height = 60;

        btnRefreshAll.Text = "↻ Refresh All";
        btnRefreshAll.Location = new Point(0, 14);
        btnRefreshAll.Size = new Size(130, 32);
        btnRefreshAll.FlatStyle = FlatStyle.Flat;
        btnRefreshAll.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnRefreshAll.ForeColor = Color.White;
        btnRefreshAll.FlatAppearance.BorderSize = 0;
        btnRefreshAll.Click += BtnRefreshAll_Click;

        btnAddDevice.Text = "+ Add Device";
        btnAddDevice.Location = new Point(140, 14);
        btnAddDevice.Size = new Size(130, 32);
        btnAddDevice.FlatStyle = FlatStyle.Flat;
        btnAddDevice.BackColor = Color.White;
        btnAddDevice.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnAddDevice.Enabled = false;

        lblLastUpdated.Text = "Last updated: —";
        lblLastUpdated.Font = new Font("Segoe UI", 9f);
        lblLastUpdated.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblLastUpdated.Location = new Point(280, 22);
        lblLastUpdated.Size = new Size(500, 22);

        pnlTop.Controls.Add(btnRefreshAll);
        pnlTop.Controls.Add(btnAddDevice);
        pnlTop.Controls.Add(lblLastUpdated);
        pnlTop.Resize += (s, e) => { lblLastUpdated.Left = pnlTop.Width - lblLastUpdated.Width - 8; };

        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Height = 60;

        btnExportSelected.Text = "Export trạng thái thiết bị (.xlsx)";
        btnExportSelected.Size = new Size(280, 36);
        btnExportSelected.Location = new Point(0, 14);
        btnExportSelected.FlatStyle = FlatStyle.Flat;
        btnExportSelected.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnExportSelected.ForeColor = Color.White;
        btnExportSelected.FlatAppearance.BorderSize = 0;
        btnExportSelected.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnExportSelected.Click += BtnExportSelected_Click;

        pnlBottom.Controls.Add(btnExportSelected);
        pnlBottom.Resize += (s, e) => { btnExportSelected.Left = pnlBottom.Width - btnExportSelected.Width - 8; };

        pnlDevices.Dock = DockStyle.Fill;
        pnlDevices.AutoScroll = true;
        pnlDevices.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
        pnlDevices.FlowDirection = FlowDirection.LeftToRight;
        pnlDevices.WrapContents = true;
        pnlDevices.Padding = new Padding(8);

        pnlState.Dock = DockStyle.Fill;
        pnlState.Visible = false;
        pnlState.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);

        lblState.Text = "Đang kiểm tra thiết bị...";
        lblState.Dock = DockStyle.Fill;
        lblState.TextAlign = ContentAlignment.MiddleCenter;
        lblState.Font = new Font("Segoe UI", 11f);
        pnlState.Controls.Add(lblState);

        Controls.Add(pnlDevices);
        Controls.Add(pnlState);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
```

- [ ] **Step 3: Create `Controls/DeviceStatusControl.cs`**

```csharp
using System.Diagnostics;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class DeviceStatusControl : UserControl
{
    public DeviceStatusControl()
    {
        InitializeComponent();
        Load += async (s, e) => await RefreshAllAsync();
    }

    private async Task RefreshAllAsync()
    {
        ShowState("Đang kiểm tra thiết bị...");
        try
        {
            var devices = await DeviceCheckService.EnumerateAsync();
            HideState();
            RenderCards(devices);
            lblLastUpdated.Text = $"Last updated: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            ShowState($"Lỗi: {ex.Message}");
        }
    }

    private void RenderCards(List<DeviceInfo> devices)
    {
        pnlDevices.SuspendLayout();
        foreach (Control c in pnlDevices.Controls) c.Dispose();
        pnlDevices.Controls.Clear();
        foreach (var d in devices)
        {
            var card = new DeviceCard();
            card.Bind(d);
            pnlDevices.Controls.Add(card);
        }
        pnlDevices.ResumeLayout();
    }

    private async void BtnRefreshAll_Click(object? sender, EventArgs e) => await RefreshAllAsync();

    private async void BtnExportSelected_Click(object? sender, EventArgs e)
    {
        var selected = pnlDevices.Controls.OfType<DeviceCard>().FirstOrDefault(c => c.IsSelected);
        if (selected == null)
        {
            MessageBox.Show("Vui lòng chọn 1 thiết bị để export.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new SaveFileDialog
        {
            Filter = "Excel files|*.xlsx",
            FileName = "device-status-log.xlsx",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            OverwritePrompt = false  // we APPEND, never overwrite blindly
        };
        if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;

        try
        {
            var path = dlg.FileName;
            var device = selected.Device;
            device.CheckedAt = DateTime.Now;
            await Task.Run(() => ExcelExportService.AppendDeviceStatus(path, device));

            var open = MessageBox.Show($"Đã ghi vào:\n{path}\n\nMở thư mục chứa?", "Thành công",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (open == DialogResult.Yes)
                Process.Start("explorer.exe", $"/select,\"{path}\"");
        }
        catch (IOException)
        {
            MessageBox.Show("File đang được mở. Vui lòng đóng Excel rồi thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi ghi file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowState(string text)
    {
        lblState.Text = text;
        pnlState.Visible = true;
        pnlState.BringToFront();
    }

    private void HideState()
    {
        pnlState.Visible = false;
        pnlDevices.BringToFront();
    }
}
```

- [ ] **Step 4: Build**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 5: Commit (large commit covering Tasks 11–16)**

```bash
git add Forms/MainForm.* Controls/BreedCard.* Controls/BreedListControl.* Controls/BreedDetailControl.* Controls/DeviceCard.* Controls/DeviceStatusControl.*
git commit -m "feat: add MainForm shell + breed list/detail + device status + cards"
```

---

## Task 17: Wire Program.cs + final smoke test

**Files:**
- Modify: `Program.cs`

- [ ] **Step 1: Replace `Program.cs` body**

```csharp
using Dog_Exploder.Forms;

namespace Dog_Exploder
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK) return;
            Session.Username = login.Username;
            Application.Run(new MainForm());
        }
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build
```

Expected: 0 errors, 0 warnings (or only nullable warnings).

- [ ] **Step 3: Run smoke checklist**

```bash
dotnet run
```

Manually verify each item. Tick the box when verified:

- [ ] (a) LoginForm appears. Empty username → Login button disabled.
- [ ] (b) Enter "ltv" → click Login → MainForm opens, greeting "Hi, ltv 👋" top-right.
- [ ] (c) Sidebar items click — content swaps. All Breeds & Device Status fully rendered, others show ComingSoonControl with name.
- [ ] (d) All Breeds initial: loading panel → grid populates with ≥10 cards (name, group badge, description). API call hits `https://dogapi.dog/api/v2/breeds`.
- [ ] (e) Cards show placeholder paw first, then images filled in over 1–3s. Breeds with no dog.ceo match keep paw.
- [ ] (f) Type "ret" in search → after ~300ms grid filters. Clear → all breeds.
- [ ] (g) Pick a group in dropdown → cards filter.
- [ ] (h) Click Refresh → loading → reloads from API.
- [ ] (i) Click a card → Detail pane: name, description, life/weight/height table, group badge. Image loads.
- [ ] (j) Click Back to List → returns to grid; search text retained.
- [ ] (k) Click Device Status → cards appear for camera/audio/network/bluetooth on this machine, color-coded badges.
- [ ] (l) Click Refresh All on Device Status → re-enumerates.
- [ ] (m) Click Export with no device selected → MessageBox "Vui lòng chọn 1 thiết bị".
- [ ] (n) Select a device radio → click Export → SaveFileDialog → pick new path → file is created with header row + 1 data row. Confirm by opening file.
- [ ] (o) Click Export again to same path → appends row 2.
- [ ] (p) Open the file in Excel and keep it open → click Export → MessageBox "File đang được mở...". No crash.
- [ ] (q) Resize MainForm → sidebar stays 240px, content reflows.
- [ ] (r) Disconnect network → vào All Breeds → error panel + Retry button. Reconnect → Retry → loads.
- [ ] (s) Click Check Connection on a single device card → status badge re-evaluates (may stay same on a working device).
- [ ] (t) `Form1.cs` no longer exists in the project (use `Get-ChildItem` to verify).

- [ ] **Step 4: Commit + tag**

```bash
git add Program.cs
git commit -m "feat: wire LoginForm -> MainForm in Program.Main"
git tag v0.1.0
```

- [ ] **Step 5: Verify Visual Studio Designer compatibility**

This step requires Visual Studio (not Claude). User opens the solution in VS and double-clicks each of:
- `Forms/LoginForm.cs`
- `Forms/MainForm.cs`
- `Controls/SidebarItem.cs`
- `Controls/BreedListControl.cs`
- `Controls/BreedCard.cs`
- `Controls/BreedDetailControl.cs`
- `Controls/DeviceCard.cs`
- `Controls/DeviceStatusControl.cs`
- `Controls/ComingSoonControl.cs`

Each should open in the Designer surface without "designer cannot display this file" errors. (Claude cannot verify this — flag in handoff.)

---

## Spec coverage check

| Spec section | Plan task |
|---|---|
| §3 Folder structure | Task 0 + 1 |
| §4 Models | Task 3 |
| §5.1 DogApiService | Task 4 |
| §5.2 DogImageService | Task 5 |
| §5.3 DeviceCheckService + RecheckAsync | Task 6 |
| §5.4 ExcelExportService | Task 7 |
| §6.1 Theme | Task 2 |
| §6.2 LoginForm | Task 8 |
| §6.3 MainForm + SidebarItem | Tasks 10, 11 |
| §6.4 BreedListControl | Task 13 |
| §6.5 BreedCard | Task 12 |
| §6.6 BreedDetailControl | Task 14 |
| §6.7 DeviceStatusControl | Task 16 |
| §6.8 DeviceCard | Task 15 |
| §6.9 ComingSoonControl | Task 9 |
| §7 Data flow + Session | Tasks 2, 11, 13, 14, 16, 17 |
| §8 Error handling | Tasks 6, 7, 13, 16 |
| §9 Threading | Tasks 4, 5, 6, 7, 12, 13, 14, 16 |
| §10 Smoke checklist | Task 17 |
| §11 Dependencies | Task 1 |
