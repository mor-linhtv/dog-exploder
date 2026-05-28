# App Config — Configurable Base URL Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace hardcoded API base URLs in `DogApiService` and `DogImageService` with values read from `appsettings.json`, so the target server can be changed without a rebuild.

**Architecture:** A static `AppConfig` class reads `appsettings.json` once on first access via `Lazy<T>`. Both service classes replace their hardcoded URL strings with `AppConfig` property lookups. Falls back to original URLs if the file is missing.

**Tech Stack:** .NET 10, `System.Text.Json` (already in SDK)

**Prerequisite:** `go-dog-api` server must be running on `http://localhost:8080` before doing the integration step. See `C:\Users\ltv\Desktop\WF-demo\go-dog-api\docs\2026-05-28-implementation-plan.md`.

---

## File Map

| File | Change |
|------|--------|
| `appsettings.json` | New — runtime base URL config |
| `AppConfig.cs` | New — static config reader |
| `Dog-Exploder.csproj` | Modified — register appsettings.json as Content CopyAlways |
| `Services/DogApiService.cs` | Modified — 1 line: use `AppConfig.DogApiBaseUrl` |
| `Services/DogImageService.cs` | Modified — 1 line: use `AppConfig.DogImageApiBaseUrl` |

---

## Task 1: Add `appsettings.json`

**Files:**
- Create: `appsettings.json`
- Modify: `Dog-Exploder.csproj`

- [ ] **Step 1: Create `appsettings.json`**

File: `C:\Users\ltv\Desktop\WF-demo\Dog-Exploder\appsettings.json`
```json
{
  "DogApiBaseUrl": "http://localhost:8080/",
  "DogImageApiBaseUrl": "https://dog.ceo/"
}
```

- [ ] **Step 2: Register in `Dog-Exploder.csproj` (Copy to Output Directory = Always)**

Add an `<ItemGroup>` after the existing `<ItemGroup>` in `Dog-Exploder.csproj`:

```xml
  <ItemGroup>
    <Content Include="appsettings.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
```

Full file after edit:
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

  <ItemGroup>
    <Content Include="appsettings.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

</Project>
```

- [ ] **Step 3: Verify file copies on build**

```powershell
cd "C:\Users\ltv\Desktop\WF-demo\Dog-Exploder"
dotnet build
```

Expected: `Build succeeded. 0 Error(s)` and `appsettings.json` present in `bin\Debug\net10.0-windows\`.

- [ ] **Step 4: Commit**

```powershell
cd "C:\Users\ltv\Desktop\WF-demo\Dog-Exploder"
git add appsettings.json Dog-Exploder.csproj
git commit -m "feat: add appsettings.json with configurable API base URLs"
```

---

## Task 2: Add `AppConfig.cs`

**Files:**
- Create: `AppConfig.cs`

- [ ] **Step 1: Create `AppConfig.cs`**

File: `C:\Users\ltv\Desktop\WF-demo\Dog-Exploder\AppConfig.cs`
```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dog_Exploder;

internal static class AppConfig
{
    private static readonly Lazy<ConfigData> _data =
        new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    public static string DogApiBaseUrl      => _data.Value.DogApiBaseUrl;
    public static string DogImageApiBaseUrl => _data.Value.DogImageApiBaseUrl;

    private static ConfigData Load()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
            return new ConfigData();
        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
        }
        catch
        {
            return new ConfigData();
        }
    }

    private sealed class ConfigData
    {
        [JsonPropertyName("DogApiBaseUrl")]
        public string DogApiBaseUrl { get; set; } = "https://dogapi.dog/";

        [JsonPropertyName("DogImageApiBaseUrl")]
        public string DogImageApiBaseUrl { get; set; } = "https://dog.ceo/";
    }
}
```

- [ ] **Step 2: Build**

```powershell
cd "C:\Users\ltv\Desktop\WF-demo\Dog-Exploder"
dotnet build
```

Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```powershell
git add AppConfig.cs
git commit -m "feat: add AppConfig reading base URLs from appsettings.json"
```

---

## Task 3: Wire services to AppConfig

**Files:**
- Modify: `Services/DogApiService.cs:12-14`
- Modify: `Services/DogImageService.cs:27`

- [ ] **Step 1: Update `DogApiService.cs`**

In `Services/DogApiService.cs`, replace lines 11–15:

Old:
```csharp
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("https://dogapi.dog/"),
        Timeout = TimeSpan.FromSeconds(30)
    };
```

New:
```csharp
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri(AppConfig.DogApiBaseUrl),
        Timeout = TimeSpan.FromSeconds(30)
    };
```

- [ ] **Step 2: Update `DogImageService.cs`**

In `Services/DogImageService.cs`, replace line 27:

Old:
```csharp
            var meta = await _http.GetFromJsonAsync<DogCeoResponse>(
                $"https://dog.ceo/api/breed/{slug}/images/random", ct);
```

New:
```csharp
            var meta = await _http.GetFromJsonAsync<DogCeoResponse>(
                $"{AppConfig.DogImageApiBaseUrl}api/breed/{slug}/images/random", ct);
```

- [ ] **Step 3: Build**

```powershell
cd "C:\Users\ltv\Desktop\WF-demo\Dog-Exploder"
dotnet build
```

Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 4: Commit**

```powershell
git add Services/DogApiService.cs Services/DogImageService.cs
git commit -m "feat: read API base URLs from AppConfig instead of hardcoding"
```

---

## Task 4: Integration verification

**Prerequisite:** Complete the `go-dog-api` implementation plan first — the server must be running.

- [ ] **Step 1: Start go-dog-api server**

```powershell
cd "C:\Users\ltv\Desktop\WF-demo\go-dog-api"
go run .
```

Expected log: `go-dog-api listening on :8080`

- [ ] **Step 2: Run Dog-Exploder**

```powershell
cd "C:\Users\ltv\Desktop\WF-demo\Dog-Exploder"
dotnet run
```

Expected: app launches, breed list loads from localhost:8080, no network errors.

- [ ] **Step 3: Verify fallback — rename appsettings.json and restart**

```powershell
Rename-Item appsettings.json appsettings.json.bak
dotnet run
```

Expected: app still launches (AppConfig falls back to hardcoded `https://dogapi.dog/`). Restore:

```powershell
Rename-Item appsettings.json.bak appsettings.json
```
