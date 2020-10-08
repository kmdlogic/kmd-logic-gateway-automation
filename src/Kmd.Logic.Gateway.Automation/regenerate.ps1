autorest --input-file=gateway.swagger.json --output-folder=. --namespace=Kmd.Logic.Gateway.Automation --csharp --override-client-name=GatewayClient --add-credentials

(Get-Content "GatewayClient.cs") |
	Foreach-Object {$_ -replace 'public partial class GatewayClient', 'internal partial class GatewayClient'} | 
	Set-Content "GatewayClient.cs"

(Get-Content "IGatewayClient.cs") | 
	Foreach-Object {$_ -replace 'public partial interface IGatewayClient', 'internal partial interface IGatewayClient'} |
	Set-Content "IGatewayClient.cs"

(Get-Content "GatewayClientExtensions.cs") | 
	Foreach-Object {$_ -replace 'public static partial class GatewayClientExtensions', 'internal static partial class GatewayClientExtensions'} |
	Set-Content "GatewayClientExtensions.cs"