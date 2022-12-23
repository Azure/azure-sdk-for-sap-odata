using DataOperations.OData.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataOperations;

namespace DataOperations.OData
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterODataServices(this IServiceCollection services, IConfiguration config)
        {
            IConfigurationSection contextSection = config.GetSection("ODataHttpClientContext");
            string contextClientName = contextSection.GetValue<string>("NamedHttpClientName");
            string baseClientAddress = contextSection.GetValue<string>("NamedHttpClientBaseUri");

            services
                .Configure<ODataHttpClientContextOptions>(contextSection)
                .Configure<ODataOperationsDispatcherOptions>(config.GetSection("ODataOperationsDispatcher"))
                .AddHttpClient(contextClientName, (client) => {client.BaseAddress = new System.Uri(baseClientAddress);});
                
            return services
                .AddSingleton<IClientContext,ODataHttpClientContext>()
                .AddSingleton<DataOperations.IOperationsDispatcher, ODataOperationsDispatcher>();
        }
    }
}