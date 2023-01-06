# Deployment via VS Code Extension

In this example we use the [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions) for VS Code to deploy the project. Learn more about [this process on Microsoft learn](https://learn.microsoft.com/azure/azure-functions/functions-create-first-function-resource-manager?tabs=azure-cli)

1. Create an Azure Functions app with Dotnet 6 and Windows Consumption plan using the [VS Code extension for Azure](https://code.visualstudio.com/docs/azure/extensions) or use below button
2. Deploy to Functions App from VS Code or [GitHub Codespaces](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=578517335) (right click in the explorer on the project folder and select **"Deploy to Function App..."**.
3. Browse your new app powered by the Azure SDK for SAP OData (it takes a while the first time): `https://your-function-app.azurewebsites.net/api/Products/10`

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-sdk-for-sap-odata%2Fmain%2Fbuildandpublish%2Fazuredeploy.json)
