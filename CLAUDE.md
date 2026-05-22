# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

`Dog-Exploder` is a .NET 10 Windows Forms desktop app (`net10.0-windows`, `WinExe`). It explores dog breeds from the public `dogapi.dog` API and includes a device-status pane. The solution file uses the new XML format: `Dog-Exploder.slnx`.

- Root namespace: `Dog_Exploder` (note the underscore — `Dog-Exploder` is invalid as a C# identifier so the project file maps the hyphen to an underscore).
- C# is `Nullable enable` + `ImplicitUsings enable`. Don't add `using System;` etc.; the SDK provides them. Add nullable annotations (`?`, `!`) where appropriate.
- Third-party packages: `ClosedXML` (Excel export in `Services/ExcelExportService.cs`), `System.Management` (WMI device queries in `Services/DeviceCheckService.cs`).

## Commands (PowerShell)

```powershell
dotnet restore                  # restore NuGet packages
dotnet build                    # compile (required before opening the VS designer on a control)
dotnet run                      # build + launch the app
```

There is **no test project** in this repository — `dotnet test` will report no tests. Don't add testing scaffolding unless explicitly requested.

To work with the visual designer, open `Dog-Exploder.slnx` in Visual Studio 2022+ and double-click any `*.cs` file under `Forms/` or `Controls/`.

## Architecture

### Entry flow

`Program.cs` runs an `ApplicationContext` (not a single `Application.Run(form)`) that loops between auth and the main window:

1. `LoginForm` shown as a modal dialog. On OK, captures `Username` into the static `Session`.
2. `MainForm` shown non-modally. When it closes, `Session.IsLoggingOut` decides whether to loop back to `LoginForm` or exit the message loop.

This is why "logout" is implemented as `Session.IsLoggingOut = true; Close();` rather than a form swap — see `Forms/MainForm.cs:24` and `Program.cs:35`.

### Session as global state

`Session.cs` is a static class that holds cross-form state (`Username`, cached `Breeds`, `Groups`, `IsLoggingOut`). Controls read from it directly. `Session.Clear()` is called between login cycles. Treat it as a singleton — don't introduce DI containers for this small app.

### MainForm pane-swap pattern

`MainForm` has a sidebar (`SidebarItem` controls keyed by string) and a content panel (`pnlContent`). `ShowPane(key)` lazily creates `UserControl` instances, caches them in `_panes`, swaps them into `pnlContent`, and toggles the `Active` flag on sidebar items.

There is a layout quirk worth knowing: `lblGreeting` is `Dock=Top` and stays parented to `pnlContent` across pane swaps. WinForms docks back-to-front, so the greeting must be `SendToBack()` and the pane `BringToFront()` after each swap — see `Forms/MainForm.cs:43-47`. If you add new panes, follow the same ordering or the greeting bar will eat 32px of the pane.

Panes communicate up to `MainForm` via events (`BreedListControl.BreedSelected`, `BreedDetailControl.BackRequested`). Don't reach across panes directly — wire events through `MainForm`.

### Layers

- `Forms/` — top-level windows (`LoginForm`, `MainForm`). One window per file pair.
- `Controls/` — reusable `UserControl`s and custom-painted `Control`s. Each pane in `MainForm` is a `UserControl` here. Custom-painted controls (`LoadingSpinner`, `NetworkStatusBar`, `BreedCard`'s rounded card) override `OnPaint` with anti-aliased `GraphicsPath`s.
- `Services/` — stateless static classes (or `IDisposable` services like `NetworkMonitorService`) for I/O. `DogApiService` paginates `dogapi.dog/api/v2/*` and resolves group names; `DogImageService` fetches images; `DeviceCheckService` queries WMI; `ExcelExportService` writes XLSX via ClosedXML; `NetworkMonitorService` polls connectivity and raises `StatusChanged`.
- `Models/` — POCOs that mirror the API shape (`Breed`, `Group`, `BreedRange`, `DeviceInfo`). DTOs used for JSON deserialization live inside `DogApiService.cs`.
- `UI/Theme.cs` — design tokens (colors, fonts) and painting helpers (`RoundedRect`, `DrawRoundedBorder`). **Always pull colors and fonts from `Theme`**; don't hardcode `Color.FromArgb(...)` or `new Font(...)` in new code.
- `docs/screen_design/` — Markdown specs of the intended visual design per screen (`login`, `list`, `detail`, `devices`, `fluent_winforms_adaptation`). Consult these when changing UI.
- `docs/superpowers/{specs,plans}/` — historical feature specs and plans created via the brainstorming workflow. Add new ones here when planning non-trivial features.

## Visual Studio Designer compatibility (important)

The user works visually in the Visual Studio Forms Designer. **Every form and `UserControl` must remain designer-loadable.** The conventions below are non-negotiable:

1. **Three-file split per form/control.** Every UI type lives in three sibling files:
   - `Foo.cs` — `public partial class Foo`, holds event handlers and behavior only.
   - `Foo.Designer.cs` — `partial class Foo`, holds private field declarations, `InitializeComponent()`, and the `Dispose(bool)` override that disposes `components`.
   - `Foo.resx` — required even if empty; the designer expects it.

   See `Forms/LoginForm.{cs,Designer.cs,resx}` and `Controls/BreedCard.{cs,Designer.cs,resx}` for the canonical shape. **Never** put `InitializeComponent` in the behavior file, and never collapse a control into a single file (exceptions: pure custom-painted `Control` subclasses with no child controls, like `Controls/LoadingSpinner.cs` and `Controls/NetworkStatusBar.cs`).

2. **Parameterless public constructor.** The designer instantiates controls reflectively. Any `UserControl`/`Control` referenced from another `Designer.cs` must have a `public Foo()` that calls `InitializeComponent()`. Constructor parameters break the designer.

3. **Build before designing.** The designer renders custom controls by loading the compiled assembly. After editing a `UserControl`, run `dotnet build` before reopening a parent form in the designer or the parent will render with placeholder boxes.

4. **No designer-hostile code in `InitializeComponent`.** The designer roundtrips `InitializeComponent()` — it reads it, mutates it, and rewrites it. Keep it to property assignments and `Controls.Add`. Don't add `if`/loops/network calls/`Session.*` reads there. Push behavior to the constructor of the code-behind, to the `Load` event, or to dedicated methods called from event handlers.

5. **Use `Theme` design tokens.** New controls should reference `Dog_Exploder.UI.Theme.Primary`, `Theme.HeadlineMd`, etc. The designer renders these fine because they are static readonly fields.

6. **Dispose pattern.** `Designer.cs` owns the `Dispose(bool)` override and is responsible for disposing `components` and any `IDisposable` field that isn't a child control (see `Forms/MainForm.Designer.cs:28-37` disposing `networkMonitor`, and `Controls/BreedCard.Designer.cs:13` cancelling the image-load `CancellationTokenSource`).

When in doubt, model new UI after `Controls/BreedListControl` (a `UserControl` with child controls and events) or `Controls/LoadingSpinner` (a `Control` subclass with custom painting).

## Async and threading notes

- API and image loads are `async Task` and called from event handlers as `async (s, e) => await ...`. `BreedListControl` yields every 20 cards via `await Task.Yield()` to keep the UI thread responsive during bulk render — preserve this pattern when adding similar bulk operations.
- `NetworkMonitorService` raises `StatusChanged` from a `System.Threading.Timer` callback (background thread). UI subscribers must marshal to the UI thread with `BeginInvoke` before touching controls.
- `BreedCard` cancels in-flight image loads in `Dispose` via a per-card `CancellationTokenSource`. Follow this pattern for any control that fires async work tied to its lifetime.

## Language and tone in UI

User-facing strings mix English (labels, brand) and Vietnamese (status messages like `"Đang tải dữ liệu..."`, `"đang được phát triển"`). Match the existing tone of the screen rather than translating across — see `docs/screen_design/` specs for the intended wording per screen.
