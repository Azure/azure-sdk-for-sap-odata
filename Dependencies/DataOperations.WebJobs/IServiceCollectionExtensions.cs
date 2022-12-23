using System.Reflection;
using DataOperations.Bindings;
using Microsoft.Extensions.DependencyInjection;
namespace DataOperations.WebJobs
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAndRegisterBindingsInAllAssemblies(this IServiceCollection services)
        {
            foreach (Assembly ass in Assembly.GetEntryAssembly().GetAllAssemblies())
            {
                services.RegisterTypes(ass, "IBaseDTOWithIDAndETag");
            }
            return services;
        }
        private static IServiceCollection RegisterTypes(this IServiceCollection services, Assembly ass, string InterfaceName)
        {
            ass.GetMatchingTypesForInterface(InterfaceName)
            .ForEach(x =>
                {
                    RegisterSingletonOfTypeForDI(services, x);
                }
            );
            return services;
        }
        private static void RegisterSingletonOfTypeForDI(IServiceCollection services, Type x)
        {
            Type typ = typeof(BaseDTOAsyncCollector<>).MakeGenericType(x.GetType());
            services.AddSingleton(typ);
            Console.WriteLine("Registered Singleton for Binding" + typ.FullName);
        }
    }
}
