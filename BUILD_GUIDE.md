# Build Guide

## Environment

Recommended:
- Visual Studio with .NET MAUI workload
- .NET 10 SDK
- Android SDK / Android emulator
- Windows machine for Visual Studio workflow

Current target frameworks in `.csproj`:
- `net10.0-android`
- `net10.0-windows10.0.19041.0`

Current Android target settings:
- Supported OS version: Android 23+
- Target platform version: Android 36.1

## NuGet Packages

Current packages in `ChroniclesoftheAbyssTower.csproj`:
- `Microsoft.Maui.Controls` 10.0.60
- `Microsoft.Extensions.Logging.Debug` 10.0.7
- `CommunityToolkit.Mvvm` 8.4.2
- `Plugin.Maui.Audio` 4.0.0
- `sqlite-net-pcl` 1.9.172
- `SQLitePCLRaw.bundle_green` 2.1.11
- `Newtonsoft.Json` 13.0.3

## Run In Visual Studio

1. Open `ChroniclesoftheAbyssTowerSln.sln`
2. Select Debug configuration
3. Select Android emulator such as Pixel API 36
4. Press Run

## Common Visual Studio Notes

- Lock icons in Solution Explorer usually mean Git/source control status and do not affect running the app
- If Visual Studio says a file changed externally, choose carefully because reloading can discard unsaved editor changes
- If opening `.jpg`/`.png` shows a binary/unsupported format warning, use an image viewer or Open With -> Image Editor

## Build Command

From repository root:

```powershell
dotnet build ChroniclesoftheAbyssTowerSln.sln
```

For Android release/publish, use Visual Studio Publish or a `dotnet publish` command configured with signing properties.

## Submission Checklist

- App builds successfully
- Login/Register works
- New Game starts
- Story floor 1 loads
- Inventory CRUD demo works
- Journal CRUD demo works
- Save/Load works
- Backup page opens and export/import flow is tested
- Images used by story files exist in `Resources/Images`
- JSON files in `Resources/Raw` remain valid
