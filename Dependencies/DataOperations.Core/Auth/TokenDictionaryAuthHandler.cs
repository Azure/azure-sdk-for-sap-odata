namespace DataOperations.Core.Auth.Http
{
    public class TokenDictionaryAuthHandler : IAuthHandler
    {
        private readonly Dictionary<string,string> _JWTs = new Dictionary<string,string>();
        public TokenDictionaryAuthHandler(Dictionary<string,string> JWTs = null)
        {
            if (JWTs != null) _JWTs = JWTs;
        }
        public async ValueTask<string> GetAuthStringAsync(string scope = ".default")
        {
            return $"Bearer {_JWTs[scope]}";
        }
    }
}