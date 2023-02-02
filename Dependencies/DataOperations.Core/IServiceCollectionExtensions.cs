using DataOperations.Core.Auth.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataOperations.Core
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterBasicHttpAuthHandler(this IServiceCollection services, IConfiguration config)
        {
            return services
                .Configure<BasicHttpAuthHandlerOptions>(config.GetSection("BasicHttpAuthHandler"))
                .AddSingleton<DataOperations.Core.Auth.IAuthHandler, BasicHttpAuthHandler>()
            ;
        }

        public static IServiceCollection RegisterAPIKeyAuthHandler(this IServiceCollection services, IConfiguration config)
        {
            return services
                .Configure<APIKeyAuthHandlerOptions>(config.GetSection("APIKeyAuthHandler"))
                .AddSingleton<DataOperations.Core.Auth.IAuthHandler, APIKeyAuthHandler>()
            ;
        }
    }
}