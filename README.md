# Drexel.Configurables
A library for exposing configurable resources.

# Description
Drexel.Configurables consists of three projects:
1. `Drexel.Configurables.Externals`: Defines & implements simple classes shared across other projects.
2. `Drexel.Configurables.Contracts`: Defines the primary interfaces (`IConfigurationRequirement`, `IRequirementSource`,
   `IConfigurator`, `IConfiguration`, and `IMapping`.) Some simple contract classes are also included, such as
   `ConfigurationRequirementType` and `CollectionInfo`.
3. `Drexel.Configurables`: Implements the contracts defined by `Drexel.Configurables.Contracts` with some canonical
   defaults. The canonical implementations are designed to function in simplel use-cases. **This is probably the
   package you're interested in.**
Other noteable projects include:
* `Drexel.Configurables.Sample`: Sample project demonstrating a simple example of using `Drexel.Configurables`.
* `Drexel.Configurables.Tests`: Contains the tests for the `Drexel.Configurables` namespace. ~92% code coverage.

# Building
The solution should build without issue on Visual Studio 2017 Community Edition. The projects target .NET Framework
version 4.7.2, which at the time of writing is not available through the Visual Studio Installer. It can be downloaded
[here](https://www.microsoft.com/net/download/thank-you/net472).
All projects except the sample are configured for StyleCop using a lightly modified ruleset, and a custom dictionary.

# Use
1. Add a reference to the `Drexel.Configurables` NuGet package in your project.
2. Implement the `IRequirementSource` interface on an object which has requirements (for example, a factory class.)
3. Either implement the `IConfigurator` interface on an object which transforms user-input to `Configuration` objects,
   or define your own interface to do something similar (ex. `IMyFactory.GetInstance(...)`.)

# Publishing
1. Download the latest recommended version of [nuget.exe](https://www.nuget.org/downloads)
2. Create an API key on [nuget.org](https://www.nuget.org/account/apikeys)
3. Build the solution in Release mode
4. Pack the `.nuspec` files using the command `nuget pack -Symbols "C:\Path\to\my.nuspec"`
5. Delete the non-`.symbol` packages
6. Set the local machine's nuget key using the command `nuget setApiKey <your_API_key>`
7. Publish the packages using the command `nuget push YourPackage.nupkg -Source https://api.nuget.org/v3/index.json`