---
name: release-nugets
description: Build specidied .NET projects and publish their NuGet packages to the NuGet feed.
disable-model-invocation: true
---

Release these NuGet packages by building them, then pushing them to the feed:

- `Zat.SystemTest.Toolkit.TestAdapter`
- `Zat.SystemTest.Toolkit.PackageBase`
- `Zat.SystemTest.Toolkit.Sdk`

(This list is the authoritative source in [`scripts/projects.ps1`](scripts/projects.ps1); update it there if the list changes.)

Run the bundled script with `pwsh`.  ...

[`scripts/build.ps1`](scripts/build.ps1). It builds the three release projects in `Release`, which packs each into its `bin/Release` folder. (It builds the projects directly rather than the whole solution, because the solution also contains VS-SDK/template projects that plain `dotnet build` cannot compile.)

On non-zero exit, report the build error and stop — do not push.

## 2. Push

Run [`scripts/push.ps1`](scripts/push.ps1). For each project it finds the newest `.nupkg` in `bin/Release` and pushes it with `dotnet nuget push ... --source Zat.Packages --skip-duplicate --force-english-output`.

Report the outcome per package:

- **Pushed** — report the upload succeeded.
- **Already on the server** (`already exists`) — report it was skipped; the script keeps going with the remaining packages.
- **Push failed** — the script stops on the first genuine failure and exits non-zero; report the error output and stop.