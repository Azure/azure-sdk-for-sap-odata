using Microsoft.Azure.WebJobs.Host.Config;
namespace DataOperations.Bindings
{
    public static class ExtensionConfigContextExtensions
    {
        public static void BindToCollector<T,K>(this ExtensionConfigContext context, IOperationsDispatcher dispatcher) where T: Attribute, IOutputAttribute where K: IBaseDTOWithIDAndETag 
        {
            context.AddBindingRule<T>().BindToCollector<K>((x) => new BaseDTOAsyncCollector<K>(dispatcher));
            Console.WriteLine("Bound to Collector" + typeof(K).FullName);
        }
        public static void BindToInput<T,K>(this ExtensionConfigContext context, Func<T, K> execThis) where T: Attribute, IInputAttribute where K: IBaseDTOWithIDAndETag 
        {
            context.AddBindingRule<T>().BindToInput<K>(execThis);
        }
        public static void BindToInputSet<T,K,L>(this ExtensionConfigContext context, Func<T, K> execThis) where T: Attribute, IInputAttribute where K: IQuerySetOperations<L> where L : IBaseDTOWithIDAndETag 
        {
            context.AddBindingRule<T>().BindToInput<K>(execThis);
        }
    }
}