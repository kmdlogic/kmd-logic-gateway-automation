![Automation Library CI/CD](https://github.com/kmdlogic/kmd-logic-gateway-automation/workflows/Automation%20Library%20CI/CD/badge.svg?branch=master)
![CLI Tool CI/CD](https://github.com/kmdlogic/kmd-logic-gateway-automation/workflows/CLI%20Tool%20CI/CD/badge.svg?branch=master)

# KMD Logic Gateway Automation
A dotnet client library for the KMD Logic Gateway service, which helps Logic client applications to automatically manage their APIs and Products published in Logic Marketplace.

## How to use this client library

### Reference `Kmd.Logic.Gateway.Automation`
In your application that handles management of APIs and Products add a NuGet package reference to [Kmd.Logic.Gateway.Automation](https://www.nuget.org/packages/Kmd.Logic.Gateway.Automation).

## How to configure Kmd.Logic.Gateway.Automation
Probably the easiest way to configure is 
```json
{
  "TokenProvider": {
    "ClientId": "",
    "ClientSecret": "",
    "AuthorizationScope": ""
  },
  "Gateway": {
    "SubscriptionId": "",
    "ProviderId": ""
  },
  "FolderPath": ""
}
```

Then you must create configuration objects.
```cs
/// Create LogicTokenProviderFactory from Kmd.Logic.Identity.Authorization
var logicTokenProviderFactory = new LogicTokenProviderFactory(
  new LogicTokenProviderOptions
  {
    AuthorizationScope = cmd.AuthorizationScope,
    ClientId = cmd.ClientId,
    ClientSecret = cmd.ClientSecret,
  });

// Create Gateway options
var gatewayOptions = new GatewayOptions
{
  SubscriptionId = cmd.SubscriptionId,
  ProviderId = cmd.ProviderId,
};
```

To get started:

1. Create a subscription in [Logic Console](https://console.kmdlogic.io). This will provide you the `SubscriptionId` which will be linked to the client credentials.
2. Request a client credential. Once issued you can view the `ClientId`, `ClientSecret` and `AuthorizationScope` in Logic Console.
3. Create provider in [Logic Marketplace](https://console.kmdlogic.io/marketplace). This will provide you the `ProviderId`. Provider must be approved by Logic Admin.

## How to use Kmd.Logic.Gateway.Automation
```cs
using var httpClient = new HttpClient();

var gatewayAutomation = new GatewayAutomation(httpClient, logicTokenProviderFactory, gatewayOptions);

// Path of the folder containing publish.yml file.
var folderPath = "./SomeFolder";

// Validate
var result = await gatewayAutomation.ValidateAsync(folderPath).ConfigureAwait(false);

// Publish
var publish = await gatewayAutomation.PublishAsync(folderPath).ConfigureAwait(false);
```

## Sample application
A simple console application is included to demonstrate how to use Logic Gateway Automation. You will need to provide the settings described above in `appsettings.json`.

## 'Publish.yml' file structure
Logic Gateway Automation requires specific file structure with file in the root folder called `publish.yml`. You can find example file structure in the [sample application source](./sample/Kmd.Logic.Gateway.Automation.Sample/Publish).

## KMD Logic Gateway Automation CLI
### Prerequisites
1. [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)

### Download as dotnet tool
To use Logic Gateway Automation using CLI you must download it from NuGet.org - [Kmd.Logic.Gateway.Automation.Tool](https://www.nuget.org/packages/Kmd.Logic.Gateway.Automation.Tool/).

To check if it was properly installed, run the following command:
```powershell
kmd-logic-gateway-automation version
```

### Commands:
* Validate - `kmd-logic-gateway-automation validate`
* Publish - `kmd-logic-gateway-automation publish`

### Parameters

| Parameter name       | Description                                                       |
|----------------------|-------------------------------------------------------------------|
| -f, --folderPath     | Required. Path of the root folder with 'publish.yml' file.        |
| -g, --gatewayUrl     | Gateway URL.                                                      |
| --scope              | Authorization scope in Logic Subscription Client Credentials.     |
| --clientId           | Required. Client ID in Logic Subscription Client Credentials.     |
| --secret             | Required. Client secret in Logic Subscription Client Credentials. |
| -p, --providerId     | Required. Provider ID in Logic.                                   |
| -s, --subscriptionId | Required. Subscription ID in Logic.                               |

### Azure Pipelines task

Here is the example tasks definition, that are _must have_ for proper KMD Logic Gateway Automation Tool utilization in Azure Pipelines:

```yaml
- task: UseDotNet@2
  displayName: Use .NET Core 3.1
  inputs:
    packageType: 'sdk'
    version: '3.1.x'

- task: PowerShell@2
  displayName: KMD Logic Gateway Automation CLI
  inputs:
    targetType: 'inline'
    script: |
      dotnet tool install --global kmd.logic.gateway.automation.tool --version <<tool_version>>
      kmd-logic-gateway-automation <<command>> <<parameters>>
```
