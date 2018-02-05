# ASP.NET Core Application Insights integration

This demonstrates the capabilities of `Application Insights` when used in an [Azure App Service][azure-app-service]. The application is composed of:

- An ASP.NET Core 2.0 API

## Configuration

You'll need to configure the following settings:

- `Token:SecretKey` - used to signed the `JWT`

## Deploy the infrastructure

```posh
cd .\template\
.\deploy.ps1 -subscriptionId <subscription-id> -resourceGroupName <resource-group-name> -resourceGroupLocation <resource-group-location>
```

This will create the resource group if it does not exist and:

- Application Insights
- App Service
- Web App
  - The Application Insights instrumentation key will be configured as an app settings
  - PHP will be turned off
  - Will use the 64 bit platform
  - ARR will be turned off

[application-insights]: https://docs.microsoft.com/en-us/azure/application-insights/app-insights-overview
[azure-app-service]: https://docs.microsoft.com/en-au/azure/app-service/app-service-web-overview
