namespace DataOperations.Core.Auth.Http
{
    public class APIKeyAuthHandlerOptions
    {
        public string APIKey {get; set;}
        public string KeyHeaderName {get; set;}
        public string TraceActive {get; set;}
    }
}