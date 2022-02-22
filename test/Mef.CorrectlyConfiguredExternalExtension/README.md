# Correct .csproj Configuration

* Reference to Mef.Contracts has `Private="false"` and `ExcludeAssets="runtime"`
* The flavor of MEF used also is decorated with the `ExcludeAssets="runtime"`
  - System.Composition and System.ComponentModel.Composition will both be provided by the host
* Both `EnableDynamicLoading` and `HasRuntimeOutput` are both set to `true`
  - HasRuntimeOutput will make sure the runtimeconfig.json is transitively copied to referencing projects (not necessary here since it's an external extension)
