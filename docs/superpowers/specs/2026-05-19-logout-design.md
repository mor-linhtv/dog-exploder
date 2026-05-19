# Logout Feature Design

**Date:** 2026-05-19  
**Scope:** Add a Logout button to MainForm that returns the user to LoginForm

---

## Overview

Add a Logout button at the bottom of the sidebar in MainForm. Clicking it clears the session and shows a fresh LoginForm. If the user logs in again, a new MainForm is created (full reset). If the user closes the LoginForm, the app exits.

---

## Architecture

### AppApplicationContext (new class in Program.cs)

Replaces the direct `Application.Run(new MainForm())` call. Manages the login → main → logout → login lifecycle.

```
Application.Run(new AppApplicationContext())

AppApplicationContext
  ├── ShowLogin()
  │     ├── LoginForm.ShowDialog()
  │     │     OK     → Session.Username = login.Username, create MainForm, main.Show()
  │     │     Cancel → ExitThread()
  │     └── main.FormClosed handler
  │           Session.IsLoggingOut == true  → Session.Clear(), ShowLogin()
  │           Session.IsLoggingOut == false → ExitThread()
  └── exits when ExitThread() is called
```

### Session changes

Add `bool IsLoggingOut` flag to `Session`. Set to `true` when Logout is clicked. Reset to `false` inside `ShowLogin()` before creating a new MainForm — prevents the flag from leaking into the next session.

Add `Session.Clear()` static method: resets `Username = ""`, `Breeds = null`, `Groups = null`, `IsLoggingOut = false`.

### MainForm changes

- Logout button click handler: `Session.IsLoggingOut = true; this.Close();`
- No other logic needed — AppApplicationContext handles the rest.

---

## UI

**Location:** Bottom of `pnlSidebarBottom`, below Support item.

```
+---------------------------+
|  Settings                 |
|  Support                  |
|---------------------------|  ← 1px separator, color #C0C7D4
|  Logout                   |  ← Button, text #C42B1C, hover bg #FDE7E9
+---------------------------+
```

**Button spec:**
- Type: `Button`, `FlatStyle.Flat`, no border
- Text: `"Logout"`, font Segoe UI 9.5pt, color `#C42B1C`
- Height: 40px, `Dock = DockStyle.Top`
- Hover: `BackColor = #FDE7E9` via `MouseEnter`/`MouseLeave` events; normal: `Color.Transparent`
- `pnlSidebarBottom` height increases from 80px → 125px

**Separator spec:**
- `Panel`, height=1, `Dock = DockStyle.Top`, `BackColor = #C0C7D4`

---

## Edge Cases

| Scenario | Behavior |
|---|---|
| User closes LoginForm (X button) after logout | `ShowDialog()` returns Cancel → `ExitThread()` → app exits |
| `IsLoggingOut` flag leak | Reset to `false` in `Session.Clear()` before each new session |
| Data isolation between sessions | `Session.Clear()` nulls Breeds/Groups so new MainForm starts fresh |
| No confirm dialog | Logout is immediate — user chose not to add confirmation |

---

## Files Changed

| File | Change |
|---|---|
| `Program.cs` | Replace `Application.Run(new MainForm())` with `Application.Run(new AppApplicationContext())`. Add `AppApplicationContext` class. |
| `Session.cs` | Add `bool IsLoggingOut`, add `Clear()` method |
| `Forms/MainForm.Designer.cs` | Add separator panel + logout button to `pnlSidebarBottom` |
| `Forms/MainForm.cs` | Add logout button click handler |
