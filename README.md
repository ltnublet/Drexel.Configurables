# Drexel.Configurables
A library for exposing configurable resources.

# Description
TODO

# Building
TODO

# Publishing
1. Download the latest recommended version of [nuget.exe](https://www.nuget.org/downloads)
2. Create an API key on [nuget.org](https://www.nuget.org/account/apikeys)
3. Build the solution in Release mode
4. Pack the `.nuspec` files using the command `nuget pack -Symbols "C:\Path\to\my.nuspec"`
5. Delete the non-`.symbol` packages
6. Set the local machine's nuget key using the command `nuget setApiKey <your_API_key>`
7. Publish the packages using the command `nuget push YourPackage.nupkg -Source https://api.nuget.org/v3/index.json`