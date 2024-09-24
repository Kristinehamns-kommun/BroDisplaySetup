# Release

## Minor Release

Open `BroDisplaySetup.csproj` and update all three versions to match the new release:

```xml
<PropertyGroup>
  <Version>0.9.3</Version> <!-- NuGet package/Project version -->
  <AssemblyVersion>0.9.3</AssemblyVersion>
  <FileVersion>0.9.3</FileVersion>
</PropertyGroup>
```

Set the version in the BroDisplaySetup properties to the same version as the project version. This action will ask you to update the ProductCode. Click Yes.

Push the changes to the repository and create a new release in GitHub:

1. Go to https://github.com/Kristinehamns-kommun/BroDisplaySetup/releases/new
1. Choose the tag version (e.g., `v0.9.3`) to create a new tag that matches the version in the project file.
1. Add a title and description for the release.
1. Click "Publish release".

## Major Release

Open `BroDisplaySetup.csproj` and update all three versions to match the new major release (i.e. Version is incremented in the first digit):

```xml
<PropertyGroup>
  <Version>2.0.0</Version> <!-- NuGet package/Project version -->
  <AssemblyVersion>2.0.0</AssemblyVersion>
  <FileVersion>0.9.3</FileVersion>
</PropertyGroup>
```

Set the version in the BroDisplaySetup properties to the same version as the project version.

For major upgrades to work, you need to:

* Change the ProductCode (same as minor upgrades).
* Keep the UpgradeCode the same.
* Ensure the Version is incremented in the first digit (e.g., 2.0.0).
* Explicitly add an "Upgrade Path" in the installer to allow major upgrades.
  * If you don't configure an upgrade path for major version changes, the installer won’t recognize it as an upgrade, and instead, you'll see the message asking to uninstall the old version manually.

Push the changes to the repository and create a new release in GitHub:

1. Go to https://github.com/Kristinehamns-kommun/BroDisplaySetup/releases/new
1. Choose the tag version (e.g., `v2.0.0`) to create a new tag that matches the version in the project file.
1. Add a title and description for the release.
1. Click "Publish release".
