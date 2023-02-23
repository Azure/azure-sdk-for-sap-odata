using System.Text;
using Microsoft.Extensions.Options;
namespace DataOperations.Core.Auth.Http
{
    public class APIKeyAuthHandler : IAuthHandler
    {
        private APIKeyAuthHandlerOptions _options;
        public APIKeyAuthHandler(IOptions<APIKeyAuthHandlerOptions> options)
        {
            _options = options.Value;
            if(!String.IsNullOrEmpty(_options.APIKey)){
                Console.WriteLine("API Key set");
            }
            if(!String.IsNullOrEmpty(_options.KeyHeaderName)){
                Console.WriteLine("KeyHeader name set" + _options.KeyHeaderName);
            }
            if(!String.IsNullOrEmpty(_options.TraceActive)){
                Console.WriteLine("APIM trace active "+ _options.TraceActive);
            }
            
        }
        public IOptions<APIKeyAuthHandlerOptions> Options { get; }

        public async ValueTask<string> GetAuthStringAsync(string scope = "ALL")
        {
            return _options.APIKey;
        }

        public async ValueTask<string> GetKeyHeaderNameAsync()
        {
            return _options.KeyHeaderName;
        }

        public async ValueTask<string> GetTraceActiveAsync()
        {
            return _options.TraceActive;
        }
    }
}