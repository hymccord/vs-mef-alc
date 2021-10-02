# VS MEF with AssemblyLoadContext

## Purpose

This repo showcases the latest version of VS-MEF with custom [AssemblyLoadContexts](https://docs.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext).

The [PartDiscovery.Combine](https://github.com/microsoft/vs-mef/blob/1a202e5a008144431e2ba02739722e3602f95db9/src/Microsoft.VisualStudio.Composition/PartDiscovery.cs#L48) method now accepts custom assembly resolvers which means that you can customize how the part discovery chain loads assemblies from the paths you give it.

## Sample "Extension" dlls

There are currently two extension dlls.

* `Mef.HostExtension.dll`
  * Showcases what is essentially a "Host" extension that is built alongside your app and is put in alongside the host in the output folder
  * Uses `Newtonsoft.Json` v13.0.1
* `Mef.ExternalExtension.dll`
  * This dll is located outside the the main app directory. You host app would have some sort of configuration setup that plugins need to hook to to tell the main app where they are stored. Ofc, there are many other different ways to go about this. It's just to showcase dlls outside of the main app path.
  * Uses `Newtonsoft.Json` v12.0.3

Also shows that different versions of Newtonsoft.Json are loaded inside the same appdomain with no problem. Each isolated to it's own extension.

### Repo AssemblyLoadContexts

* IsolatedLoadContext
  * This one was inspired from the dotnet/msbuild repo. See MSBuildLoadContext.cs for the original
  * Checks for culture and defaults back to loading the assembly inside `AssemblyLoadContext.Default` if all else fails.
* PluginLoader
  * Derived from the MSDN documentation on 'Building an application with plugins'
  * Uses `AssemblyDependencyResolver` (aka `*.runtime.json` file) to find and load dependencies

## Troubleshooting

Initially, getting VS-MEF to recognize imports on other assemblies that were loaded in silo'd AssemblyLoadContexts was harder than expected (because I didn't read [this good documentation](https://docs.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext#complications)).
I went the route of what MSBuild does and construct an list of assembly names that are "well known" and need to be shared by default.

In my case, these well known assemblies are

* Mef.Contracts
* System.Component.Model.Composition
* System.Composition.*

## Concerns

Since part exports are loaded in their own assembly, I'm fairly confident that MEF exports and imports between two extensions would not compatible.
All imports would need to come from the contract provided interfaces or from within it's own load context.

## Getting Started (developing)

You can build with Visual Studio or `dotnet build`

### Prerequisites

This repo currently uses the latest build of [vs-mef](https://github.com/microsoft/vs-mef), currently v17.1.3-alpha.
You will need to `dotnet pack` that repo and put the packages in a place where the nuget restore can find them.

Personally, I use the user level NuGet.Config located in %APPDATA%/NuGet/NuGet.config and add a line to where I put locally built packages

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="Microsoft Visual Studio Offline Packages" value="C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\" />
    <add key="local-nuget-source" value="<MY_USER_PROFILE>\local-nuget-source\" />
  </packageSources>
</configuration>
```

Once there is a nuget package of v17 available, you can just change the package reference in Mef.Host to that version.

## Running

Once built (or publish), you can invoke the two available load context like so

Using the `IsolatedLoadContext`

```cmd
Mef.Host --loader Isolated
```

Using the `PluginLoadContext`

```cmd
Mef.Host --loader Plugin
```
