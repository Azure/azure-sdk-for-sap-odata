namespace DataOperations.Core.Auth.Http
{
    public class TokenAuthHandler : IAuthHandler
    {
        private readonly string _JWTs = "";
        public TokenAuthHandler(string JWTs = null)
        {
            if (JWTs != null) _JWTs = JWTs;
        }
        public async ValueTask<string> GetAuthStringAsync(string scope = ".default")
        {
            return $"Bearer {_JWTs}";
        }
    }
}