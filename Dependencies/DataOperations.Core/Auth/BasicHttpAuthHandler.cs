using System.Text;
using Microsoft.Extensions.Options;
namespace DataOperations.Core.Auth.Http
{
    public class BasicHttpAuthHandler : IAuthHandler
    {
        private BasicHttpAuthHandlerOptions _options;
        public BasicHttpAuthHandler(IOptions<BasicHttpAuthHandlerOptions> options)
        {
            _options = options.Value;
            if(!String.IsNullOrEmpty(_options.UserName)){
                Console.WriteLine("UserName set");
            }
            if(!String.IsNullOrEmpty(_options.Password)){
                Console.WriteLine("Password set");
            }
            
        }
        public IOptions<BasicHttpAuthHandlerOptions> Options { get; }
        public async ValueTask<string> GetAuthStringAsync(string scope ="ALL")
        {
            return $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.UserName}:{_options.Password}"))}";
        }
    }
}