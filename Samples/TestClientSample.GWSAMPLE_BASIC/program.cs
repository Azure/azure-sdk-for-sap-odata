using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DataOperations.Core;
using DataOperations.OData;
using DataOperations.OData.Client;

namespace GWSAMPLE_BASIC
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {        
            var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context,services) =>
            {
                // Register the SDK and dependencies for use in the SDK sample service
                services
                    .RegisterBasicHttpAuthHandler(context.Configuration) // Register the BasicHttpAuthHandler for use in the SDK sample service, or whichever auth handler you need
                    .RegisterODataServices(context.Configuration)        // Code is in Dependencies\DataOperations.OData\IServiceCollectionExtensions.cs, registers the OData Client Classes
                    .AddSingleton<DataOperations.IOperationsDispatcher, ODataOperationsDispatcher>()
                    .RegisterSetsAsSingletons()                          // Generated in the Data.GWSAMPLE_BASIC project, registers the poco classes as singletons
                    .AddHostedService<TestService>();                    // Inject the test background service into the DI container
            })
            .ConfigureLogging(logging =>
            {   
                logging.AddConsole()
                .SetMinimumLevel(LogLevel.Warning);
            })
            .Build();
            try
            {
                await host.RunAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}