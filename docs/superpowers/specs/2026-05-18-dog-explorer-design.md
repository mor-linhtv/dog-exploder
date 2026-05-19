# Dog Explorer — WinForms Edition (Design Spec)

**Date:** 2026-05-18
**Owner:** LinhTV
**Target framework:** .NET 10 (Windows), WinForms, C#

## 1. Mục tiêu

Ứng dụng Windows Forms demo dùng [Dog API v2](https://dogapi.dog/docs) hiển thị danh sách giống chó, tìm kiếm, xem chi tiết, refresh dữ liệu, kèm chức năng kiểm tra trạng thái thiết bị (camera, bluetooth, network, audio) và xuất kết quả ra file `.xlsx`.

UI dựa trên 4 mockup đã có sẵn trong `docs/screen_design/` (login, list, detail, devices) theo design system "Fluent WinForms Adaptation" (Windows Blue `#0078d4`, sidebar 240px, corner radius 4-8px, không dùng drop shadow).

Cấu trúc project là WinForms tiêu chuẩn, tận dụng Visual Studio Designer (mỗi Form/UserControl có file `.cs` + `.Designer.cs` + `.resx`).

## 2. Scope (đã chốt với user)

| Tính năng | In | Out |
|---|---|---|
| Login (chỉ nhập username, không validate) | ✅ | |
| Sidebar 7 mục, 2 mục thực + 5 stub "Coming soon" | ✅ | |
| All Breeds: list, search, filter group, refresh | ✅ | |
| Breed detail | ✅ | |
| Ảnh breed qua dog.ceo (fallback placeholder) | ✅ | |
| Device Status: enum WMI thật, refresh | ✅ | |
| Export Excel SaveFileDialog + append | ✅ | |
| Favorites / History / Comparison / Settings / Support | | ❌ stub "Coming soon" |
| Persist state qua phiên | | ❌ in-memory only |
| Unit test, E2E test | | ❌ manual smoke |
| Localization | | ❌ hard-code tiếng Việt |

## 3. Folder structure

```
Dog-Exploder/
├─ Dog-Exploder.csproj         (thêm ClosedXML 0.104.x, System.Management 9.x)
├─ Program.cs                  (LoginForm → MainForm)
├─ Session.cs                  (static: Username, Breeds, Groups)
├─ UI/
│   └─ Theme.cs                (static color/font constants)
├─ Models/
│   ├─ Breed.cs
│   ├─ BreedRange.cs
│   ├─ Group.cs
│   └─ DeviceInfo.cs
├─ Services/
│   ├─ DogApiService.cs
│   ├─ DogImageService.cs
│   ├─ DeviceCheckService.cs
│   └─ ExcelExportService.cs
├─ Forms/
│   ├─ LoginForm.cs / .Designer.cs / .resx
│   └─ MainForm.cs / .Designer.cs / .resx
├─ Controls/
│   ├─ SidebarItem.cs / .Designer.cs / .resx
│   ├─ BreedListControl.cs / .Designer.cs / .resx
│   ├─ BreedCard.cs / .Designer.cs / .resx
│   ├─ BreedDetailControl.cs / .Designer.cs / .resx
│   ├─ DeviceStatusControl.cs / .Designer.cs / .resx
│   ├─ DeviceCard.cs / .Designer.cs / .resx
│   └─ ComingSoonControl.cs / .Designer.cs / .resx
└─ docs/superpowers/specs/2026-05-18-dog-explorer-design.md
```

`Form1.cs` / `Form1.Designer.cs` / `Form1.resx` hiện tại bị xoá (thay bằng `MainForm`).

## 4. Models

```csharp
public class Breed
{
    public string Id { get; set; }            // GUID từ JSON:API "id"
    public string Name { get; set; }
    public string Description { get; set; }
    public BreedRange Life { get; set; }       // years
    public BreedRange MaleWeight { get; set; } // pounds
    public BreedRange FemaleWeight { get; set; }
    public BreedRange MaleHeight { get; set; } // inches
    public BreedRange FemaleHeight { get; set; }
    public bool Hypoallergenic { get; set; }
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }     // resolved sau khi load Groups
}

public class BreedRange { public int Min { get; set; } public int Max { get; set; } }
public class Group     { public string Id { get; set; } public string Name { get; set; } }

public enum DeviceStatus { Connected, Disconnected, Degraded, Unknown }

public class DeviceInfo
{
    public string Name { get; set; }
    public string Category { get; set; }       // Camera | Bluetooth | Network | Audio
    public DeviceStatus Status { get; set; }
    public string Detail { get; set; }         // free text: IP, MAC, error, …
    public DateTime CheckedAt { get; set; }
}
```

## 5. Services

### 5.1 `DogApiService`
- `static HttpClient _http` (BaseAddress `https://dogapi.dog/`)
- `Task<List<Breed>> GetAllBreedsAsync(CancellationToken)`:
  - Loop `GET api/v2/breeds?page[number]={n}` cho đến hết (đọc `meta.pagination.last`).
  - Parse JSON:API qua `System.Text.Json` + DTO `JsonApiResponse<BreedDto>`.
  - Map sang `Breed`.
- `Task<List<Group>> GetGroupsAsync(CancellationToken)` — tương tự với `api/v2/groups`.
- Sau khi có cả 2 list, gọi internal `ResolveGroupNames(breeds, groups)`.

### 5.2 `DogImageService`
- `Task<Image?> GetImageAsync(string breedName, CancellationToken)`:
  - Slug = `breedName.ToLowerInvariant().Split(' ')[0]`
  - Cache `Dictionary<string, Image?>` theo slug
  - GET `https://dog.ceo/api/breed/{slug}/images/random` → JSON `{ "message": "<url>", "status":"success" }`
  - GET URL → `Image.FromStream(stream)`
  - Lỗi/404 → cache `null`, return null
- `void ClearCache()` cho Refresh

### 5.3 `DeviceCheckService`
- `Task<List<DeviceInfo>> EnumerateAsync()` (`Task.Run` wrap):
  - **Camera**: `Win32_PnPEntity WHERE PNPClass='Camera' OR PNPClass='Image'`
  - **Bluetooth**: `Win32_PnPEntity WHERE PNPClass='Bluetooth'`
  - **Audio**: `Win32_SoundDevice`
  - **Network**: `NetworkInterface.GetAllNetworkInterfaces()` (lọc loopback/tunnel)
  - Status map:
    - WMI `Status='OK'` & `ConfigManagerErrorCode=0` → Connected
    - WMI `Status='Degraded'` → Degraded
    - WMI `Status='Error'` hoặc `ConfigManagerErrorCode>0` → Disconnected
    - NetworkInterface `OperationalStatus.Up` → Connected, else Disconnected
  - Detail string chứa thông tin phụ (ManufacturerName/Description/IP/MAC).
  - Mỗi category lỗi (permission/v.v.) → trả 1 `DeviceInfo` placeholder status=Unknown với Detail=error message thay vì throw.
- `Task<DeviceInfo> RecheckAsync(DeviceInfo device)`: re-query 1 device theo `Category` + `Name` qua WMI hoặc `NetworkInterface`. Trả về bản sao có `CheckedAt` mới và `Status`/`Detail` cập nhật. Nếu không tìm thấy thiết bị nữa → status Disconnected.

### 5.4 `ExcelExportService`
- `void AppendDeviceStatus(string path, DeviceInfo device)`:
  - Dùng `ClosedXML.Excel.XLWorkbook`
  - Nếu file không tồn tại: tạo mới, sheet `DeviceStatus`, row 1 = header `Ngày | Giờ | Thiết bị | Loại | Trạng thái | Chi tiết`, style header bold + BG `#f3f3f3` + border bottom.
  - Nếu file tồn tại: `new XLWorkbook(path)`, lấy sheet `DeviceStatus` (tạo nếu chưa có), tìm last row có data, append.
  - Ghi:
    - Col A: `device.CheckedAt.ToString("yyyy-MM-dd")`
    - Col B: `device.CheckedAt.ToString("HH:mm:ss")`
    - Col C: `device.Name`
    - Col D: `device.Category`
    - Col E: `device.Status.ToString()`
    - Col F: `device.Detail`
  - `workbook.SaveAs(path)` (overwrite OK với ClosedXML)
  - Throw `IOException` nếu file đang mở (UI catch hiển thị "Đóng file Excel rồi thử lại").

## 6. UI

### 6.1 Theme
Static class `Dog_Exploder.UI.Theme` cung cấp constants:
- Colors: `Primary=#0078d4`, `PrimaryDark=#005faa`, `Surface=#f9f9f9`, `Canvas=#ffffff`, `SidebarBg=#f3f3f3`, `Border=#c0c7d4`, `TextOnSurface=#1a1c1c`, `OnSurfaceVariant=#404752`, `Success=#198754`, `Error=#ba1a1a`, `Warning=#974700`, `BadgeBg=#d3e3ff`, `BadgeFg=#004883`
- Fonts (build lười tại getter): `HeadlineLg = Segoe UI 20pt Semibold`, `HeadlineMd = Segoe UI 14pt Semibold`, `BodyLg = Segoe UI 11pt`, `BodyMd = Segoe UI 9pt`, `LabelMd = Segoe UI 9pt Semibold`
- Helper `Theme.RoundedRect(Graphics g, Rectangle r, int radius, Color border, Color fill)`

### 6.2 LoginForm (480×420, fixed dialog, centered)
- Card panel 360×340 ở giữa, BG=Canvas, border 1px.
- PictureBox logo 48×48 (vẽ paw bằng GDI+ trong Paint).
- Label "Đăng nhập" (HeadlineLg), Label phụ (BodyMd OnSurfaceVariant).
- Field "Tên đăng nhập" (Label LabelMd) + TextBox `txtUsername` (placeholder "Nhập tên đăng nhập", height 32, border 1px, focus → border Primary qua custom Paint).
- Field "Mật khẩu" + TextBox `txtPassword` `UseSystemPasswordChar=true` — **decorative only**, không đọc giá trị, không validate (theo Q3 scope: chỉ cần username).
- CheckBox "Ghi nhớ đăng nhập" (decorative), LinkLabel "Quên mật khẩu?" (decorative).
- Button `btnLogin` (full width 32px height, BG=Primary, text "Đăng nhập"). Enabled chỉ khi `txtUsername.Text.Trim().Length > 0`.
- Click → set `Username` public property + `DialogResult=OK` + close.

### 6.3 MainForm (1100×720, MinSize 900×600)
- `pnlSidebar` Dock=Left Width=240 BG=SidebarBg
  - Top header (90px): paw icon 32×32 + "Breed Explorer" (LabelLg) + "WinForms Edition" (LabelSm)
  - Stack of `SidebarItem`: All Breeds, Favorites, History, Comparison, Device Status
  - Bottom (Dock=Bottom 80px): Settings, Support
- `pnlContent` Dock=Fill, padding 24px (vẽ background Surface), Top label "Hi, {Username}" Dock=Top 32px align right
- `SidebarItem` UserControl 240×40:
  - 3px vertical bar trái (visible khi `Active=true`, BG=Primary)
  - 16px icon + Label (BodyMd) 16px text left padding
  - Hover: BG=#e8e8e8
  - Active: BG=#e2e2e2 + bar visible
  - Click event raised lên MainForm
- Lazy create UserControls: `Dictionary<string, UserControl> _panes`. `ShowPane(key)` clears `pnlContent`, adds cached or creates new.
- Pane keys: `"breeds"` (BreedListControl), `"detail"` (BreedDetailControl — không có sidebar item), `"devices"` (DeviceStatusControl), `"favorites"` / `"history"` / `"comparison"` / `"settings"` / `"support"` (đều cùng dùng ComingSoonControl instance riêng với label phù hợp).

### 6.4 BreedListControl (Dock=Fill)
- Top bar (Dock=Top 60px):
  - Label "Dog Breed Index" (HeadlineLg)
  - TextBox `txtSearch` 240×32 với search icon vẽ trong Paint (placeholder "Search breeds…")
  - ComboBox `cboGroup` 180×32 DropDownStyle=DropDownList (first item "All Groups")
  - Button `btnRefresh` 32×32 refresh icon
- Body:
  - Loading: Panel với Label "Đang tải dữ liệu..." + ProgressBar Marquee
  - Error: Panel với Label đỏ + Button "Thử lại"
  - Grid: `FlowLayoutPanel pnlGrid` Dock=Fill AutoScroll BG=Canvas, FlowDirection=LeftToRight, WrapContents=true
- Behavior: xem §7.

### 6.5 BreedCard (UserControl 220×260)
- Border 1px #d1d1d1, corner 8px (vẽ Paint, anti-aliased)
- PictureBox 200×140 SizeMode=Zoom, BG xám khi chưa load
- Label tên (LabelLg) bold
- Panel chứa 1-2 badge (group, hypoallergenic) — mỗi badge custom Paint chip rounded 4px
- Label mô tả (BodyMd OnSurfaceVariant) 2 dòng, AutoEllipsis
- Cursor=Hand toàn card, Click event `BreedSelected(Breed)` raised

### 6.6 BreedDetailControl (Dock=Fill)
- Top bar 60px: Button "← Back to List" (secondary), spacer, Button "Edit Record" (decorative, disabled). Mockup có nút "Add to Favorites" nhưng **Favorites đã out of scope** (Q1) → bỏ luôn nút này, không hiển thị.
- Card header (Dock=Top 220px): PictureBox 280×180 trái, panel phải gồm tên (HeadlineLg) + mô tả (BodyMd 3 dòng auto-ellipsis) + 2 badge (group, hypoallergenic).
- 2 card dưới side-by-side 50/50:
  - "Breed Specifications": Table 2 cột, các row: Life Expectancy, Weight (Male), Weight (Female), Height (Male), Height (Female), Coat Type (placeholder "—").
  - "Temperament & Care": 3 static row (Trainability, Energy, Shedding) với placeholder text — vì API không cung cấp.
- `SetBreed(Breed b)`: populate text + `_ = DogImageService.GetImageAsync(b.Name)` để load ảnh.

### 6.7 DeviceStatusControl (Dock=Fill)
Docking order quan trọng (WinForms dock theo thứ tự thêm — bottom/top phải add trước Fill):
- `pnlTop` Dock=Top 60px:
  - Button `btnRefreshAll` (primary "↻ Refresh All") + Button `btnAddDevice` (secondary "+ Add Device", disabled — decorative).
  - Label "Last updated: HH:mm" align right.
- `pnlBottom` Dock=Bottom 60px:
  - Button `btnExportSelected` (primary "Export trạng thái thiết bị (.xlsx)") align right.
- `pnlDevices` `FlowLayoutPanel` Dock=Fill AutoScroll, chứa `DeviceCard` instances (add cuối cùng để fill phần giữa).
- Behavior: xem §7.

### 6.8 DeviceCard (UserControl ~360×240)
- Border 1px + radius 8px (Paint).
- Header row: icon (router/bluetooth/camera/audio tuỳ category) + Name (LabelLg) + Badge status (chip rounded 4px):
  - Connected: BG `#cfe2ff` FG `#003f6d`
  - Degraded: BG `#fde7d3` FG `#974700`
  - Disconnected: BG `#ffdad6` FG `#93000a`
  - Unknown: BG `#e8e8e8` FG `#404752`
- Rows (key-value): tuỳ category, hiển thị Category, Detail, CheckedAt (HH:mm:ss).
- Footer: RadioButton `rbSelect` "Chọn để export" + Button "Check Connection" (re-check riêng card này, gọi service).

### 6.9 ComingSoonControl (Dock=Fill)
- Center label HeadlineMd "Tính năng này đang được phát triển" + BodyMd subtext.

## 7. Data flow

### 7.1 Khởi động
```csharp
// Program.Main
ApplicationConfiguration.Initialize();
using var login = new LoginForm();
if (login.ShowDialog() != DialogResult.OK) return;
Session.Username = login.Username;
Application.Run(new MainForm());
```

### 7.2 Session (in-memory)
```csharp
static class Session
{
    public static string Username;
    public static List<Breed>? Breeds;
    public static List<Group>? Groups;
}
```

### 7.3 Breed list
- `BreedListControl.OnLoad`: nếu `Session.Breeds == null` → `await LoadAsync()`.
- `LoadAsync`:
  1. Hiện loading panel.
  2. Try `Session.Breeds = await _api.GetAllBreedsAsync()`, `Session.Groups = await _api.GetGroupsAsync()`.
  3. Populate `cboGroup` (item 0 = "All Groups", còn lại từ Groups).
  4. `RenderCards(filter="", group=null)`.
  5. Ẩn loading.
  - Catch → hiện error panel với message + button "Thử lại" gọi lại LoadAsync.
- `txtSearch.TextChanged`:
  - Reset `_searchDebounce` Timer 300ms; Tick → `RenderCards`.
- `cboGroup.SelectedIndexChanged` → `RenderCards`.
- `btnRefresh.Click`:
  - `Session.Breeds = null; Session.Groups = null; _imageService.ClearCache();`
  - `await LoadAsync()`.
- `RenderCards(filter, group)`:
  - Clear `pnlGrid.Controls`, dispose cũ.
  - Filter `Session.Breeds.Where(b => b.Name.Contains(filter, OrdinalIgnoreCase) && (group==null || b.GroupId==group.Id))`.
  - Add max 200 cards (an toàn UI).
  - Mỗi card: `card.BreedSelected += OnBreedSelected` → raise event lên parent (MainForm).

### 7.4 Image lazy load
- `BreedCard.OnLoad`: `_ = LoadImageAsync()` fire-forget.
- `LoadImageAsync`: `var img = await _imageService.GetImageAsync(_breed.Name, _cts.Token); if (img!=null && !IsDisposed) picBox.Image = img;`.
- `Dispose`: `_cts.Cancel()`.

### 7.5 Detail navigation
- `BreedCard.BreedSelected` → `BreedListControl` raises `BreedSelected` lên `MainForm`.
- `MainForm.OnBreedSelected(breed)`:
  - `_detailControl.SetBreed(breed)`
  - `ShowPane("detail")` (special — không có sidebar item tương ứng, active state vẫn giữ "All Breeds").
- `BreedDetailControl.btnBack.Click` → raise `BackRequested` → `MainForm.ShowPane("breeds")`.

### 7.6 Device flow
- `DeviceStatusControl.OnLoad`: `await RefreshAllAsync()`.
- `RefreshAllAsync`:
  - Clear cards. Hiện loading panel.
  - `var devices = await _deviceService.EnumerateAsync();`
  - Render `DeviceCard` cho mỗi device, group radio button.
  - Update "Last updated".
- `btnRefreshAll.Click` → `RefreshAllAsync()`.
- `DeviceCard.btnCheck.Click`:
  - `await _deviceService.RecheckAsync(device)` (re-query 1 device qua WMI hoặc NetworkInterface theo Category + Name)
  - Update card UI.
- `btnExportSelected.Click`:
  - Tìm card có `rbSelect.Checked` → nếu không có → `MessageBox.Show("Vui lòng chọn 1 thiết bị để export.")`.
  - `SaveFileDialog` filter `Excel files|*.xlsx`, default name `device-status-log.xlsx`, default folder `MyDocuments`.
  - Try `await Task.Run(() => _excel.AppendDeviceStatus(path, selected.Device))`.
  - Catch `IOException` → MessageBox "File đang được mở. Vui lòng đóng file Excel rồi thử lại."
  - Success → MessageBox "Đã ghi vào {path}" với button "Mở thư mục" (Process.Start explorer.exe).

## 8. Error handling

| Tình huống | Xử lý |
|---|---|
| Không có internet khi load breeds | Error panel + nút "Thử lại"; không crash. |
| Image dog.ceo 404/timeout | Card hiển thị placeholder paw, không log lỗi UI. |
| WMI permission denied | Devicecard placeholder `Unknown` status + Detail = error message. |
| Excel file đang mở (IOException) | MessageBox cảnh báo, không crash. |
| Excel sheet "DeviceStatus" mất | Tạo lại sheet và header. |

## 9. Threading

- Tất cả I/O (HTTP, WMI, file): `async` hoặc `Task.Run`.
- WinForms tự `Post` continuation về UI thread (SynchronizationContext) → safely set control properties sau `await`.
- `CancellationTokenSource` cho image load trong `BreedCard`, hủy khi card dispose.
- Không dùng `.Wait()` / `.Result` trên main thread.

## 10. Testing & verification

### Build
- `dotnet build` ở thư mục project: 0 errors. Warning chấp nhận được.
- `dotnet restore` lấy `ClosedXML 0.104.x` và `System.Management 9.x`.

### Smoke checklist (chạy `dotnet run`)
1. LoginForm hiện. Username rỗng → nút Login disabled. Nhập "ltv" → MainForm mở, header chào "Hi, ltv".
2. Sidebar click — content panel đổi. All Breeds & Device Status có UI thật, 5 mục còn lại hiện ComingSoon.
3. All Breeds initial load: loading → ≥10 cards có tên, mô tả, group badge.
4. Cards xuất hiện trước, ảnh fill vào sau. Breed không tìm thấy ảnh → placeholder.
5. Search "ret" → lọc còn breeds chứa "ret". Xoá → đầy đủ lại.
6. Group filter → cards lọc đúng.
7. Refresh → loading → reload từ API + clear image cache.
8. Click card → Detail. Verify name/desc/life/weight/height. Back → list state giữ search.
9. Device Status: cards hiện camera/network/audio/bluetooth tuỳ máy, badge màu đúng.
10. Refresh All → re-enum.
11. Export Excel:
    - Không chọn device → cảnh báo.
    - Chọn 1 device → SaveFileDialog → file mới có header + 1 row.
    - Export tiếp vào cùng file → append row 2.
    - File đang mở → MessageBox "Đóng file...".
12. Resize MainForm → sidebar 240px giữ, content fill, cards reflow.
13. Tắt internet → All Breeds → error + Retry. Bật lại → Retry → load OK.

### Acceptance
- Build pass.
- 13 mục smoke pass trên máy Windows có WMI hoạt động.
- Mở mỗi Form/UserControl trong VS Designer hiển thị được (user xác nhận thủ công).

### Out of scope
- Unit/E2E test tự động.
- Favorites/History/Comparison/Settings/Support feature.
- Localization runtime switch.
- Persist state qua phiên.

## 11. Dependencies

- `ClosedXML` 0.104.x (Apache-2.0) — read/write `.xlsx`.
- `System.Management` 9.x — WMI.
- Không cần khác. Project hiện ở .NET 10 Windows.

## 12. Open questions

Không còn — tất cả ambiguity đã chốt qua 5 câu Q&A.
