using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using DataOperations.OData;
using DataOperations.Core;
using DataOperations.WebJobs;
using GWSAMPLE_BASIC;
using Microsoft.Azure.WebJobs;
using DataOperations.Bindings;

[assembly: FunctionsStartup(typeof(FunctionsDemo.Startup))]
namespace FunctionsDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {       
            var config = builder.GetContext().Configuration;
            // Register the SDK and dependencies for use in the SDK sample service
            builder.Services
                .RegisterBasicHttpAuthHandler(config)                // Code is in Dependencies\DataOperations.Core\IServiceCollectionExtensions.cs, Registers the BasicHttpAuthHandler for use in the SDK sample service, or whichever auth handler you need
                .RegisterODataServices(config)                       // Code is in Dependencies\DataOperations.OData\IServiceCollectionExtensions.cs, registers the OData Client Classes
                .RegisterSetsAsSingletons()                          // Generated in the Data.GWSAMPLE_BASIC project, registers the poco classes as singletons
                .ConfigureAndRegisterBindingsInAllAssemblies()      // Uses reflection to go and locate the function bindings for the poco classes to enable the SDK in function bindings
                .AddWebJobs(x => { return; }).AddExtension<DataExtensionConfigProvider>();

            }

    }
}