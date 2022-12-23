using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using DataOperations.Bindings.Generated;
using DataOperations.Bindings;
using DataOperations;
namespace DataOperations.Bindings
{
    [Extension("DataExtension")]
    public class DataExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly IOperationsDispatcher dispatcher;
        public DataExtensionConfigProvider(IOperationsDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public void Initialize(ExtensionConfigContext context)
        {
            BindingHelper.ConfigureBindings(context, dispatcher);
        }
    }
}