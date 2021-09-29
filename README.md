# VS MEF with AssemblyLoaderContext

## Purpose

This repo showcases the latest version of VS-MEF with custom [AssemblyLoadContexts](https://docs.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext).

The [PartDiscovery.Combine](https://github.com/microsoft/vs-mef/blob/1a202e5a008144431e2ba02739722e3602f95db9/src/Microsoft.VisualStudio.Composition/PartDiscovery.cs#L48) method now accepts custom assembly resolvers which means that you can customize how the part discovery chain loads assemblies from the paths you give it.

### Repo AssemblyLoadContexts
  * IsolatedLoadContext
    - This one was inspired from the dotnet/msbuild repo. See MSBuildLoadContext.cs for the original
    - Checks for culture and defaults back to loading the assembly inside `AssemblyLoadContext.Default` if all else fails.
  * PluginLoader
    - Derived from the MSDN documentation on 'Building an application with plugins'
    - Uses `AssemblyDependencyResolver` (aka `*.runtime.json` file) to find and load dependencies

## Sample "Extension" dlls

There are currently two extension dlls.
* `Mef.HostExtension.dll`
  - Showcases what is essentially a "Host" extension that is built alongside your app and is put in alongside the host in the output folder
* `Mef.ExternalExtension.dll`
  - This dll is located outside the the main app directory. You host app would have some sort of configuration setup that plugins need to hook to to tell the main app where they are stored. Ofc, there are many other different ways to go about this. It's just to showcase dlls outside of the main app path.

## Troubleshooting

Initially, getting VS-MEF to recognize imports on other assemblies that were loaded in silo'd AssemblyLoadContexts was difficult. I haven't fully traced down the problem but it _was_ remiedied by making what MSBuild calls "WellKnownAssemblies" resolve to the ones that are distributed by the host application.

In my case, these well known assemblies are
* Mef.Contracts
  - Without this, I got cast exceptions when trying to cast the ExternalExtension to the Host's IExtension interface
* System.Component.Model.Composition
  - Without this, the parts aren't seen as exports by VS MEF catalogue.

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

## TODO
- Unit tests
- MEFv1 exports and their well known assemblies
