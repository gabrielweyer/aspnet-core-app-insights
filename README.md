# ASP.NET Core Application Insights integration

This demonstrates the capabilities of [Application Insights][application-insights] when used in an [Azure App Service][azure-app-service]. The application is composed of:

- An `ASP.NET Core 2.2` API
- A `Web Job (.NET Core 2.2 console app)`
- An `Azure Service Bus` namespace with a topic and a subscription to allow the API to delegate some tasks to the `Web Job`

## Configuration

You'll need to configure the following secrets:

- `Jwt:SecretKey` - used to sign the `JWT`, should be at least `16` characters
- `ServiceBus:ConnectionString`
- `APPINSIGHTS_INSTRUMENTATIONKEY`

## Deploy the infrastructure

```posh
cd .\template\
.\deploy.ps1 -subscriptionId <subscription-id> -resourceGroupName <resource-group-name> -resourceGroupLocation <resource-group-location>
```

This will create the resource group if it does not exist and:

- Application Insights
- An Azure Service Bus namespace with a topic and a subscription
- App Service
- Web App
  - The Application Insights instrumentation key will be configured as an app settings
  - The Topic Sender SAS connection string will be configured as an app settings
  - PHP will be turned off
  - Will use the 64 bit platform
  - ARR will be turned off

Alternatively you can [sign-in to Azure][sign-in-azure] and [test the deployment][test-deployment]. Execute the commands line by line:

```posh
Login-AzureRmAccount
Get-AzureRMSubscription
Set-AzureRmContext -SubscriptionID <subscription-id>
# Either test the deployment:
Test-AzureRmResourceGroupDeployment -ResourceGroupName "<resource-group-name>" -TemplateFile .\template.json -TemplateParameterFile .\parameters.json
# Or deploy it:
New-AzureRmResourceGroupDeployment -ResourceGroupName "<resource-group-name>" -TemplateFile .\template.json -TemplateParameterFile .\parameters.json
```

[application-insights]: https://docs.microsoft.com/en-us/azure/application-insights/app-insights-overview
[azure-app-service]: https://docs.microsoft.com/en-au/azure/app-service/app-service-web-overview
[sign-in-azure]: https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-resource-manager-overview#log-in-to-azure-and-set-the-azure-subscription
[test-deployment]: https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-resource-manager-overview#test-the-deployment
