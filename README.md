# Risky Cohesion

Source for the [Risky Cohesion](RiskyCohesion/Thunderstore/README.md) Risk of Rain 2 mod.

## Building

Requires the .NET SDK (6 or later). Game, BepInEx, R2API and Risk Of Options references come from NuGet (`NuGet.config` adds the BepInEx feed). RiskyMod, EliteReworks, Aetherium, Sandswept, Starstorm 2, Spikestrip 2.0, Moonstorm Shared Utils and LookingGlass are referenced from the r2modman profile, so they must be installed there.

```
dotnet build RiskyCohesion.sln
```

Every build copies the mod into the r2modman profile named by `ProfileName` in `Directory.Build.props`. A Release build (`-c Release`) also writes a Thunderstore package to `dist/`.

To use another profile, create `Directory.Build.props.user` next to the solution:

```xml
<Project>
  <PropertyGroup>
    <ProfileName>Default</ProfileName>
  </PropertyGroup>
</Project>
```
