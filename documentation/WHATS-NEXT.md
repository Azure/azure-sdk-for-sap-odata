# What's next?

## Authentication with Azure AD 🔐

[Configure](https://learn.microsoft.com/azure/app-service/configure-authentication-provider-aad?toc=%2Fazure%2Fazure-functions%2Ftoc.json) your App Service or Azure Functions app to use Azure AD login. Use standard variable `X-MS-TOKEN-AAD-ACCESS-TOKEN` to retrieve the access token from the request header. [Learn more](https://learn.microsoft.com/azure/app-service/configure-authentication-oauth-tokens#retrieve-tokens-in-app-code)

Consider SAP Principal Propagation for your authentication scenario handled by [Azure API Management](https://learn.microsoft.com/azure/api-management/sap-api#production-considerations).

[Learn more](https://github.com/Azure/api-management-policy-snippets/blob/master/examples/Request%20OAuth2%20access%20token%20from%20SAP%20using%20AAD%20JWT%20token.xml)

## Token handling

The generated SDK handles ETags for update operations automatically. For CSRF tokens it relies on a centralized solutions with [Azure API Management policies](https://github.com/Azure/api-management-policy-snippets/blob/master/examples/Get%20X-CSRF%20token%20from%20SAP%20gateway%20using%20send%20request.policy.xml) rather than implementing it in every client. Find a complete policy including SAP Principal Propagation [here](https://github.com/Azure/api-management-policy-snippets/blob/master/examples/Request%20OAuth2%20access%20token%20from%20SAP%20using%20AAD%20JWT%20token.xml). In case you require a client-side solution for CSRF have a look at [this class](Dependencies/DataOperations.OData/DTO/BaseDTOWithIDAndETag.cs) to get started.

Azure AD tokens are handled by Azure without any code dependencies. Use standard variable `X-MS-TOKEN-AAD-ACCESS-TOKEN` to retrieve the access token from the request header and work with [TokenAuthHandler](Dependencies/DataOperations.Core/Auth/TokenAuthHandler.cs). [Learn more](https://learn.microsoft.com/azure/app-service/configure-authentication-oauth-tokens#retrieve-tokens-in-app-code)

## Connectivity to SAP backends and secure virtual network access 🔌

SAP backends on Azure typically run in fully isolated virtual networks. There are multiple ways to connect to them. Most popular ones are:

- Integrate your Azure Function App with an Azure virtual network (VNet). [Learn more](https://learn.microsoft.com/azure/azure-functions/functions-networking-options).
- Private Endpoints for Azure Functions. [Learn more](https://learn.microsoft.com/azure/azure-functions/functions-create-vnet?source=recommendations)
- User Azure API Management for OData with SAP Principal Propagation. [Learn more](https://learn.microsoft.com/azure/api-management/sap-api#production-considerations)

VNet integration enables your app to securely access resources in your VNet, such as your SAP Gateway, but doesn't block public access to your Function app. To achieve full private connectivity for the app service too, look into private endpoints.