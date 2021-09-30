# Bad .csproj Configuration 😢

* Reference to Mef.Contracts without `Private="false"` and `ExcludeAssets="runtime"`
* Both MEF PackageReferences are copying all their runtime assets to the output path
* `EnableDynamicLoading` is still set to true so all the other dependencies are copied to output (e.g. Newtonsoft.Json)
