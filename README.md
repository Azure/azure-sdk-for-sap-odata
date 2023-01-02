# Azure SDK for SAP OData 🚀

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=578517335)[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-sdk-for-sap-odata%2Fmain%2Fbuildandpublish%2Fazuredeploy.json)

Use this repos to enable services like [Azure Functions](https://learn.microsoft.com/azure/azure-functions/functions-overview) <⚡> to consume [SAP OData services](https://api.sap.com/products/SAPS4HANA/apis/ODATA) and work with plain-old-class-objects (POCO) without any dependencies on other libraries.

Marshalling the OData semantics into the SDK **reduces initial integration effort** and allows developers to **focus on business logic quicker**.

Currently supported output language is C#. You may extend by adding templates for other languages [here](DataOperations.Generator.OData/Templates). Consumers of the SDK can be coded in any language.

> **Note** - due to the nature of the SDK it can be applied to apps or functions coded in NodeJS too.

![Illustration of the function app flow with the OData SDK](/assets/img/function-app-overview.png)

> **Note** - in case you prefer SAP's own Cloud SDK for your Azure app development, have a look at [this sister project](https://github.com/Azure-Samples/app-service-javascript-sap-cloud-sdk-quickstart) deploying to Azure App Service.

## Prerequisites 👨🏾‍🎓

- [Azure Subscription](https://azure.microsoft.com/free/)
- Local Development setup with [Dotnet 6](https://dotnet.microsoft.com/download/dotnet/6.0) and [Azure Functions Core tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local?tabs=v4%2Cwindows%2Ccsharp%2Cportal%2Cbash) or [![GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=578517335) to hit the ground running without any further need for setup.
- Any OData Service (for example [SAP Mock Server](https://sap.github.io/cloud-s4-sdk-book/pages/mock-odata.html), [SAP Demo Gateway (OData Catalog Service)](https://sapes5.sapdevcenter.com/sap/opu/odata/IWFND/CATALOGSERVICE;v=2/), [S/4HANA Cloud](https://api.sap.com/products/SAPS4HANACloud/apis/ODATAV4), [SAP API Business Hub](https://api.sap.com/), [SAP SuccessFactors](https://api.sap.com/products/SAPSuccessFactors/apis/ODATA), S/4HANA Fully-Activated appliance from [SAP CAL](https://cal.sap.com/), your own in-house SAP system, etc.)

> **Note**
> The SAP mock server is a separate project and **not** part of this repo. You can find the mock server [here](https://sap.github.io/cloud-s4-sdk-book/pages/mock-odata.html).

> **Note**
> You can use the [SAP API Business Hub](https://api.sap.com/) to get a free trial of SAP OData services or leverage the public demo system ES5. Find the GWSAMPLE_BASIC demo service on ES5 [here](https://sapes5.sapdevcenter.com/sap/opu/odata/iwbep/GWSAMPLE_BASIC/$metadata). See SAP's [blog post](https://blogs.sap.com/2017/12/05/new-sap-gateway-demo-system-available/) about ES5 for more details and how to request a user.

## Getting Started 🚀

- Get your OData service metadata as file or URL
- `git clone https://github.com/Azure/azure-sdk-for-sap-odata.git`
- Feed your OData metadata to the supplied generator. Learn more about the generator [here](DataOperations.Generator.OData/README.md)

### For Windows

```cmd
cd azure-sdk-for-sap-odata\BinaryDownloads
Expand-Archive DataOperations.Generator.OData_win-x64.zip C:\Generator
cd C:\Generator\publishout
.\DataOperations.Generator.OData.exe --inputfile C:\Generator\publishout\metadata.xml --outputfolder C:\SDK --templatefolder C:\Generator\publishout\Templates --samples true
```

### For Linux

```bash
cd azure-sdk-for-sap-odata/BinaryDownloads
unzip DataOperations.Generator.OData_linux-x64.zip -d /workspaces/Generator
cd /workspaces/Generator/publishout
chmod +x DataOperations.Generator.OData
chmod -R 777 /workspaces/Generator/publishout
./DataOperations.Generator.OData --inputfile /workspaces/Generator/publishout/metadata.xml --outputfolder /workspaces/SDK --templatefolder /workspaces/Generator/publishout/Templates --samples true
```

### For Mac

```bash
cd azure-sdk-for-sap-odata/BinaryDownloads
unzip DataOperations.Generator.OData_osx-x64.zip -d /Generator
cd /Generator/publishout
./DataOperations.Generator.OData --inputfile /Generator/publishout/metadata.xml --outputfolder /SDK --templatefolder /Generator/publishout/Templates --samples true
```

- Enjoy the Microsoft + SAP ASCII chart and find the .NET SDK for your OData service in your specified folder `C:\SDK`.
- Get started with provided Function App sample in `C:\SDK\Samples\FunctionsSample.GWSAMPLE_BASIC`.
- Maintain respective [`local.settings.json`](/Samples/FunctionsSample.GWSAMPLE_BASIC/local.settings_sample.json) with your SAP OData service setup and credentials. Have a look at the provided sample file 'local.settings_sample.json'.
- Open the solution in Visual Studio or Visual Studio Code.
- Execute `func host start` and start coding😎👌🔥
- Call `http://localhost:7071/api/Products/10` to see the generated OData service in action listing the first 10 products.
- Use provided [call library](Samples/FunctionsSample.GWSAMPLE_BASIC/odata-product-sample.http) with [VS Code REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) for convenience.

> **Note**
> Adjust to `https://your-gh-domain.preview.app.github.dev/api/Products/10` for Codespaces.

Learn more about the local Azure Functions start commands per project language [here](https://learn.microsoft.com/azure/azure-functions/functions-run-local?tabs=v4%2Cwindows%2Ccsharp%2Cportal%2Cbash#start).

> **Note**
> Use the [TestClient sample](/Samples/TestClientSample.GWSAMPLE_BASIC/) for local execution without Azure Functions environment in case you are looking for a generic implementation.

### How to use with Azure Functions <⚡>

Have a look at the [SAPBindingDemo.cs](Samples/FunctionsSample.GWSAMPLE_BASIC/SAPBindingDemo.cs) file to learn more about the generated SDK and how to use it in Azure Functions.

- **http Trigger**: specifies the http method and route (e.g. /Products/{param})
- **SAP Input Binding**: enables complex pre-flight requests for SAP business objects and functions served by the OData service setting the scene for the actual request. This simplifies the request structure and puts focus on the business logic.
- **SAP Output Binding**: enables complex post-processing of the SAP business object altered in the function, marshalling the required OData call to finish the request.

> **Note**
> The described approach in this repos is applicable to other Azure PaaS offerings like Azure App Service. See the [Azure App Service SAP Cloud SDK quickstart for JavaScript](https://github.com/Azure-Samples/app-service-javascript-sap-cloud-sdk-quickstart) for reference.

## Deploy to Azure🪂

There are multiple ways to deploy this project to Azure. We provide two options in this repository:

1. via template deployment and VS Code extension - more information is available [here](documentation/DEPLOYMENT-VSCODE.md)
2. via [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/overview) using `azd up`  - more information is available here soon and tracked in [Issue 1](https://github.com/Azure/azure-sdk-for-sap-odata/issues/1)

## Understand your generated SDK structure🫀

You get two folders in your output folder. One containing the helper classes always named "Dependencies" and the other containing the individually generated classes for your OData service. The sub folder `WebJobs` refers to the Azure Functions bindings.

Learn more about the generated SDK structure [here](/DataOperations.Generator.OData/README.md).

## What's next?

You can do a lot more once the app is deployed. Curious? We go you covered with some more information [here](documentation/WHATS-NEXT.md)

## Troubleshooting 🩺

- [Access SAP Gateway logs in /IWFND/ERROR_LOG](https://wiki.scn.sap.com/wiki/display/ABAPConn/SAP+Gateway+Error+Log)
- [Azure Functions diagnostics](https://docs.microsoft.com/azure/app-service/troubleshoot-diagnostic-logs)
- Use [Kudu SSH console](https://learn.microsoft.com/azure/app-service/resources-kudu) from VNet integrated App Service to check connectivity to SAP Gateway with `curl https://<your domain>:<your https port>/sap/opu/odata/sap/API_BUSINESS_PARTNER -u "<user>:<password>"`
- [Azure API Management Request tracing](https://learn.microsoft.com/azure/api-management/api-management-howto-api-inspector)
- [Azure API Management Gateway logs](https://learn.microsoft.com/azure/api-management/api-management-howto-use-azure-monitor#view-diagnostic-data-in-azure-monitor)

## Additional resources

We compared to existing OpenAPI projects, collected additional resources, related efforts and projects [here](documentation/ADDITIONAL-RESOURCES.md).

## Contributing👩🏼‍🤝‍👨🏽

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks™

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/legal/intellectualproperty/trademarks/usage/general). Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party's policies.
