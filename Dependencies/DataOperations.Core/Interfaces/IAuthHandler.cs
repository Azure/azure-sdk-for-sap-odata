namespace DataOperations.Core.Auth
{
    public interface IAuthHandler
    {
        ValueTask<string> GetAuthStringAsync(string scope ="ALL");
    }
}