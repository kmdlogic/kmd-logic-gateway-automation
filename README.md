![Automation Library CI/CD](https://github.com/kmdlogic/kmd-logic-gateway-automation/workflows/Automation%20Library%20CI/CD/badge.svg?branch=master)
![CLI Tool CI/CD](https://github.com/kmdlogic/kmd-logic-gateway-automation/workflows/CLI%20Tool%20CI/CD/badge.svg?branch=master)

# KMD Logic Gateway Automation
A dotnet client library for the KMD Logic Gateway service, which helps Logic client applications to automatically manage their APIs and Products published in Logic Marketplace.

## How to use this client library
### Reference `Kmd.Logic.Gateway.Automation`
In your application that handles management of APIs and Products add a NuGet package reference to [Kmd.Logic.Gateway.Automation](https://www.nuget.org/packages/Kmd.Logic.Gateway.Automation).

### Appsettings


### and the `IPublish` interface like this:
```
var publish = new Publish(httpClient, tokenProviderFactory, configuration.Gateway);
await publish.ProcessAsync({folderPath}).ConfigureAwait(false);
```