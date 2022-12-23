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
            Console.WriteLine("UserName: {0}", _options.UserName);
            Console.WriteLine("Password: {0}", _options.Password);
        }
        public IOptions<BasicHttpAuthHandlerOptions> Options { get; }
        public async ValueTask<string> GetAuthStringAsync(string scope ="ALL")
        {
            return $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.UserName}:{_options.Password}"))}";
        }
    }
}