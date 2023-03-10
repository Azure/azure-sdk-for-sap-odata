# Deployment via Azure Developer CLI

The project structure to support the Azure Developer CLI is not yet released!

**UNDER CONSTRUCTION**

## Deployment

In this example we use the [Azure Developer CLI](https://github.com/Azure/azure-dev) to deploy the project. Learn more about [this tool on Microsoft learn](https://learn.microsoft.com/azure/developer/azure-developer-cli/overview)

1. Deploy the infrastructure and your app via `azd up`. You will get asked some questions during the procedure to specify e.g. your subscription.
2. Browse your new app powered by the SAP Cloud SDK (it takes a while the first time).

If you want to separate the steps and have a closer look what is happing you can split the deployment via:

1. `azd provision` - this will exclusively set up your infrastructure.
2. `azd deploy` - this will deploy your application to the provisioned infrastructure.

## Cleanup

If you want to clean up your deployment execute the command `azd down` which will delete your deployment and your resources.
