# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

BroDisplaySetup is a Windows Forms (.NET 7, `net7.0-windows`) desktop app that simplifies arranging multi-monitor setups. It shows a numbered overlay on each connected screen, lets the user type the order the numbers appear in (left to right), then arranges the screens accordingly via Win32 display APIs, sets the left-most external monitor as primary (unless a single large "conference room" screen is detected — see below), and applies each screen's optimal resolution.

## Build / run

Open `BroDisplaySetup.sln` in Visual Studio 2022, or from the CLI:

```
dotnet build BroDisplaySetup.csproj
dotnet run --project BroDisplaySetup.csproj
```

The app is Windows-only (WinForms + `System.Management`/WMI + user32.dll P/Invoke) and must be built/run on Windows — it cannot run on Linux/macOS or in a headless environment, and manipulates the real display configuration when run, so exercise care when testing.

Docker build (produces the same output as CI, without needing VS installed):

```
docker/build.sh
```

Output: `bin\Release\net7.0-windows\publish\win-x64\BroDisplaySetup.exe`

There is no automated test suite — `TESTING.md` describes a manual test checklist (see below).

## Manual testing

Follow `TESTING.md`. Key points:
- `extras\EraseScreenConfig.reg` clears saved Windows display configurations before testing so the app is exercised against a clean state; reboot after applying it.
- Test with varying monitor counts (including single-monitor, which should just apply optimal resolution and exit), lid-closed boot with external monitors, pre-existing "misconfigured" arrangements in Windows display settings, and invalid/fuzzed input into the numbering text boxes.

## Release process

See `RELEASE.md` for the full procedure. In short: bump `Version`/`AssemblyVersion`/`FileVersion` in `BroDisplaySetup.csproj` (all three, kept in sync), let the installer project prompt to update its `ProductCode`, push, then create a GitHub release tagged `vX.Y.Z`. The `.github/workflows/publish.yml` workflow (triggered on `v*.*.*` tags) builds the app, builds the MSI installer via `devenv` against `installer\BroDisplaySetupInstaller.vdproj`, and attaches the exe + MSI + `setup.exe` to the GitHub release. Major-version upgrades additionally require an explicit "Upgrade Path" entry in the installer project (`UpgradeCode` stays constant across all versions).

## Architecture

The app has no dependency-injection or layering — it's a small set of static helper classes called directly from `Program.Main`.

- **`Program.cs`** — entry point. Builds the primary overlay form returned by `Displays.ArrangeManuallyFromLTRWithAutoResolutionForm()`, then bolts on chrome (menu strip with "Show screen info" / "Forget conference room choices" / a "Conference room mode" checkbox toggle / "About", a custom-drawn close button, and a help-text/logo overlay drawn in `Form.Paint`). There is no separate top-level `Form` class for the main window — it's built imperatively.
- **`Displays.cs`** — the core workflow. `ConfigureDisplayOrderAndArrangeForm()` creates one borderless, screen-sized `Form` per monitor (in `Screen.AllScreens`/`Extern` order) showing a big number, plus single-digit `RoundedTextBox` inputs on the primary screen's form for the user to type the order. As digits are typed, the corresponding overlay form is highlighted; on completing input, `ArrangeLTRWithAutoPrimary()` is called to actually apply the layout, then all overlay forms close. Also owns per-monitor DPI auto-scaling (`autoscaleExternalDisplays` / `resetScalingForExternalDisplays`, tied to the "scale displays" checkbox) and `ArrangeAutomaticallyFromLTRWithAutoResolution()` (a non-interactive path, currently unused by `Program.cs` but kept for programmatic/automatic arrangement).
  - **Conference room handling**: `IsLargeRoomDisplay(DisplayInfo)` flags a display as a physically large "room" screen (EDID diagonal ≥ 50", falling back to a 4K-resolution check when EDID doesn't report physical size — see `DisplayInfo.DiagonalInches`). Any external display flagged this way gets forced to 250% DPI scaling via `applyForcedScalingForExternalDisplays()` (Windows itself recommends 300% for these, but that was judged excessive) (which also hides the "scale displays" checkbox for that run, since there's nothing left to opt into). If there is *exactly one* external display and it's flagged, `ConfigureDisplayOrderAndArrangeForm()` records that display's serial in `ConferenceRoomCandidateSerial` and shows an inline `ScalableCheckBox` (in the same slot the "scale displays" checkbox would otherwise occupy, since the two conditions are mutually exclusive) defaulting to checked, or to the remembered per-display answer from `ConferenceRoomPreferences` if one exists. Checking it keeps the internal panel primary instead of the usual left-most-external rule. The result is stored in the static `ConferenceRoomModeActive` and read *live* by `ArrangeLTRWithAutoPrimary()` at arrange-time — not captured into a closure — since `Program.cs`'s always-enabled "Konferensrumsläge" menu checkbox (`Displays.SetConferenceRoomMode()`) can also flip it before the user finishes typing the screen order (the menu item works even without a detected candidate, as a manual fallback for setups the size heuristic doesn't flag — though in that case there's no serial to persist the answer against). The inline checkbox and the menu checkbox are two independent controls for the same state, kept in sync via the static `Displays.ConferenceRoomModeChanged` event, which each side raises/subscribes to (and `SetConferenceRoomMode` no-ops if the value isn't actually changing, which is what keeps that mutual subscription from looping). `ScalableCheckBox.CheckedChanged` exists specifically to support this — it fires from the `IsChecked` setter itself (only on an actual change) rather than from `OnClick`, so external listeners (and programmatic sets, eg. from the sync above) always observe the post-toggle value.
- **`ConferenceRoomPreferences.cs`** — persists the conference-room yes/no answer keyed by `DisplayInfo.Serial` as JSON at `%AppData%\BroDisplaySetup\conference-rooms.json`. Blank serials are never persisted (some EDIDs report a blank/shared serial, which would misattribute one room's answer to another screen). `ForgetAll()` backs the "Forget conference room choices" menu item in `Program.cs`.
- **`DisplayInfo.cs`** — cross-references four different Windows data sources to build a full picture of each monitor: WMI `WmiMonitorID` (friendly name, serial, manufacturer), `Extern.Displays.GetVideoOutputTechnologyByDevicePathMap()` (used to detect the *internal* laptop panel via `DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL`), `DPIHelper` (current/min/max/recommended DPI scaling), and WMI `WmiMonitorBasicDisplayParams` (physical `PhysicalWidthCm`/`PhysicalHeightCm` from EDID, exposed as computed `DiagonalInches`). Devices are correlated by PnP device ID / device path / `InstanceName` string matching (WMI `InstanceName` vs. the `\\?\...` paths from `user32`), since there's no single API that returns everything.
- **`Extern.cs`** (`BroDisplaySetup.Extern` namespace) — all P/Invoke: `DEVMODE`/`DISPLAY_DEVICE` structs, the `DISPLAYCONFIG_*` structs/enums for the modern Windows display config API, and the `User_32` class of `user32.dll` externs. `Extern.Displays` is the actual Win32 wrapper layer:
  - `Arrange(primaryDisplayName, leftDisplayNames[], rightDisplayNames[])` — positions displays left-to-right around the primary (which is always placed at `0,0`), aligning all screens along the bottom edge, upgrading each to its optimal mode first if the current mode is worse.
  - `SwitchToExtendModeIfClone()` — forces extend topology if the OS reports a clone/mirrored topology.
  - `SetDpiScaling(pnpDeviceId, percent)` — sets per-monitor DPI scaling using the undocumented `DISPLAYCONFIG_DEVICE_INFO_GET/SET_DPI_SCALE` device-info type (`DISPLAYCONFIG_DEVICE_INFO_TYPE_CUSTOM`), expressed as steps relative to the recommended value from `DPI.DPIConstants.DpiVals`.
  - `GetOptimalDisplayMode` / `ComputeDisplayModeScore` — picks the "best" mode per display over `EnumDisplaySettings`. Prefers the display's native/recommended resolution first (via `GetPreferredDisplayMode`, which queries the CCD API's `DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE` - the same EDID preferred-timing source Windows itself uses to mark a resolution "Recommended"), picking the best `width * height * bpp + refreshRate`-scored mode among those matching it; falls back to that raw heuristic across all modes if the preferred resolution can't be determined. This exists because some panels accept a wider "signal" resolution than their native panel resolution (eg. a native 3840x2160 UHD panel that also accepts, but crops rather than scales, a 4096x2160 DCI 4K input) - naively maximizing `width * height` alone would pick a mode the display can't actually show correctly.
- **`DpiHelper.cs`** (`BroDisplaySetup.DPI` namespace) — `DPIScalingInfo` converts the OS's *relative* DPI scale (steps from recommended) into absolute percentages using the fixed `DpiVals` ladder (100–500%). `DPIHelper.GetDpiScalingInfoByDevicePathMap()` bridges `Extern.Displays` DPI queries to this representation.
- **`RoundedTextBox.cs`** / **`ScalableCheckBox.cs`** — custom-drawn WinForms controls (rounded-corner text input, a checkbox with independently scalable box/text) used by the overlay form.
- **`ScreenInfoForm.cs`**, **`About.cs`** — secondary dialogs reachable from the menu strip, showing per-display diagnostic info (`DisplayInfo.ToString()`) and app/version info respectively.
- **`MainForm.cs`** — a designer-generated form with buttons wired to `Displays.*`; not instantiated anywhere (`Program.cs` builds its UI programmatically instead). Treat as legacy/dead code, not the live entry point.

### Display arrangement model

Monitors are identified throughout by their Win32 **device name** (e.g. `\\.\DISPLAY1`), which is distinct from the **PnP device ID** (used to correlate WMI/DPI data) and the **device path** used by the `DISPLAYCONFIG_*` DPI-scaling APIs — a given physical monitor has all three, and code frequently has to convert between them via string matching (see `DisplayInfo.GetDisplayInfoForAllConnectedDisplayDevices`). The initial (pre-user-input) LTR ordering always puts the internal laptop panel first (`Displays.GetAutoArrangedLTRScreenDeviceNames`); the user-entered order can place it anywhere, but the internal panel is never chosen as primary if any external monitor is present — the left-most external monitor becomes primary instead (`Displays.ArrangeLTRWithAutoPrimary`), **unless** the conference-room prompt was answered "yes" for a single large external display, in which case `ArrangeLTRWithAutoPrimary(_, keepInternalPrimary: true)` pins the internal panel as primary instead (see "Conference room handling" above).

## Localization

UI strings live in `Properties/Resources.resx` (Swedish is the primary language for on-screen text, e.g. "Avancerat", "Visa skärminformation..."). `README.md`/`README.sv-se.md` and `extras/README.md`/`extras/README.sv-se.md` are maintained as English/Swedish pairs — update both when changing user-facing docs.

## Known limitations (see README.md)

The app does not detect monitors connected or disconnected while it's running — it must be restarted to pick up hardware changes.
