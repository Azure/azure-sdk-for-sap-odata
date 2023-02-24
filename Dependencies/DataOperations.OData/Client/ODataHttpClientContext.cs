using System.Text.Json;
using System.Text.Json.Serialization;
using DataOperations.OData.Deserialization;
using DataOperations.OData.Serialization;
using DataOperations.Core.Auth;
using Microsoft.Extensions.Options;
using DataOperations.Core.Auth.Http;

namespace DataOperations.OData.Client
{
    public class ODataHttpClientContext : IClientContext
    {
        private IHttpClientFactory _clientFactory;
        private readonly BasicHttpAuthHandler _userAuthHandler;
        private readonly APIKeyAuthHandler _apiAuthHandler;
        private readonly ODataHttpClientContextOptions _Options;


        // Instantiate the client as a context class 
        // We'll need an HtpClientFactory with an already defined base address and token callback
        // We'll also need a callback to fetch a JWT based on the URL and or the object type or method
        public ODataHttpClientContext(IHttpClientFactory clientFactory, IEnumerable<IAuthHandler> authHandlers, IOptions<ODataHttpClientContextOptions> options)
        {
            _clientFactory = clientFactory;
            
            foreach(var authHandler in authHandlers){
                if(authHandler is BasicHttpAuthHandler){
                    this._userAuthHandler = (BasicHttpAuthHandler)authHandler;
                }
                if(authHandler is APIKeyAuthHandler){
                    this._apiAuthHandler = (APIKeyAuthHandler)authHandler;
                }
            }
            
            this._Options = options.Value;
        }

        public virtual async ValueTask<V> FireRemoteRequestAsync<T, V>(T Payload, string tailUri = "/", HttpMethod method = null, Dictionary<string, object> KeyParams = null,Dictionary<string,string> CustomHeaders = null)
        where T : IBaseDTOWithIDAndETag where V : IBaseDTOWithIDAndETag  
        {
            HttpRequestMessage _requestMessage = await BuildHttpRequestMessageAsync(NullWrapSerializePayload(Payload), tailUri, method, KeyParams, CustomHeaders);
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            return await DeserializeAsyncResponse<V>(_resp);
        }

        public virtual async ValueTask<object> FireRemoteRequestAsyncWithSimpleReturn<T>(T payload, string tailUri = "/", HttpMethod method = null, Dictionary<string, object> KeyParams = null, Dictionary<string, string> CustomHeaders = null) 
        where T : IBaseDTOWithIDAndETag
        {
            HttpRequestMessage _requestMessage = await BuildHttpRequestMessageAsync(NullWrapSerializePayload(payload), tailUri, method, KeyParams, CustomHeaders);
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            return await DeserializeAsyncResponse<object>(_resp);
        }

        public virtual async ValueTask<IEnumerable<V>> FireRemoteRequestAsyncWithNoPayloadAndEnumerable<T, V>(string tailUri = "/", HttpMethod method = null, Dictionary<string, object> KeyParams = null, Dictionary<string, string> CustomHeaders = null)
            where T : IBaseDTOWithIDAndETag where V : IBaseDTOWithIDAndETag  
        {
            HttpRequestMessage _requestMessage = await BuildHttpRequestMessageAsync(NullWrapSerializePayload(default(T)), tailUri, method, KeyParams, CustomHeaders);
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            return (await DeserializeAsyncResponse<RootList<V>>(_resp)).d.results;
        }

        public virtual async ValueTask<V> FireRemoteRequestAsyncWithEtag<T, V>(T Payload, string tailUri = "/", HttpMethod method = null,  Dictionary<string, object> KeyParams = null, Dictionary<string,string> CustomHeaders = null, string etag = ""
        )   where T : IBaseDTOWithIDAndETag where V : IBaseDTOWithIDAndETag      
        {
            HttpRequestMessage _requestMessage = await BuildHttpRequestMessageAsync(NullWrapSerializePayload(Payload), tailUri, method, KeyParams, CustomHeaders, etag);
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            V _result = (await DeserializeAsyncResponse<RootSingle<V>>(_resp)).d;
            ETagMe<V, T>(_resp, _result);
            return _result;
        }

        public virtual async ValueTask<T> FireRemoteRequestAsyncDirectUri<T>(string tailUri = "/", HttpMethod method = null) where T : IBaseDTOWithIDAndETag
        {
            HttpResponseMessage _resp = await ExecuteHttpRequest(await BuildHttpRequestMessageAsync("", new Uri(tailUri).PathAndQuery, method));
            return (await DeserializeAsyncResponse<RootSingle<T>>(_resp)).d;    
        }

        public virtual async ValueTask<IEnumerable<T>> FireRemoteRequestAsyncDirectUriIEnumerable<T>(string tailUri = "/", HttpMethod method = null) 
        where T : IBaseDTOWithIDAndETag
        {
            HttpResponseMessage _resp = await ExecuteHttpRequest(await BuildHttpRequestMessageAsync("",  new Uri(tailUri).PathAndQuery, method));
            return (await DeserializeAsyncResponse<RootList<T>>(_resp)).d.results; 
        }
        private static string NullWrapSerializePayload<T>(T Payload, bool ChangesOnly = false) where T : IBaseDTOWithIDAndETag
        {
            string SPayload = "";
            if (Payload != null) SPayload = SerializePayload(Payload, ChangesOnly);
            return SPayload;
        }
        private static async Task<T> DeserializeAsyncResponse<T>(HttpResponseMessage _resp)
        {
            JsonSerializerOptions jso = GetDeserializerOptions();

            // Should really Asynchronously deserialize an instance of T from the response body here
            // For now we'll just return the response body as a string.
            // This is a hack to get the debug data whilst in dev mode / preview.
            string debug = await _resp.Content.ReadAsStringAsync();
            T ret = (JsonSerializer.Deserialize<T>(debug, jso));
            return ret;
        }
        private async ValueTask<HttpRequestMessage> BuildHttpRequestMessageAsync(string Payload, string tailUri, HttpMethod method, Dictionary<string, object> KeyParams = null, Dictionary<string,string> CustomHeaders = null, string etag = "")
        {
            if (method == HttpMethod.Get)
            {
                if (tailUri.Contains("?"))
                {
                    tailUri = tailUri + "&$format=json";
                }
                else
                {
                    tailUri = tailUri + "?$format=json";
                }
            }

            var _requestMessage = new HttpRequestMessage(method ?? HttpMethod.Get, tailUri);

            if(_apiAuthHandler != null)
            {
                //in case API key provided add with defined header name. Azure APIM uses 'Ocp-Apim-Subscription-Key' and SAP APIM uses 'APIKey'
                _requestMessage.Headers.Add($"{await _apiAuthHandler.GetKeyHeaderNameAsync()}", $"{await _apiAuthHandler.GetAuthStringAsync("ALL")}");
                //tracing flag opinionated towards Azure APIM
                _requestMessage.Headers.Add("Ocp-Apim-Trace", $"{await _apiAuthHandler.GetTraceActiveAsync()}");
            }
            
            //only add basic auth header if BASIC credentials are maintained
            string basicAuthToken = await _userAuthHandler.GetAuthStringAsync("ALL");
            if(!String.IsNullOrEmpty(basicAuthToken))
            {
                _requestMessage.Headers.Add("Authorization", basicAuthToken);
            }

            _requestMessage.Headers.Add("Accept", "application/json");

            if(etag != "")
            {
                _requestMessage.Headers.Add("If-Match", etag);
            }

            if (CustomHeaders != null)
            {
                foreach (var header in CustomHeaders)
                {
                    _requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            if (Payload != "" && (method ?? HttpMethod.Get) != HttpMethod.Get)
            {
                _requestMessage.Content = new StringContent(
                    Payload,
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
            }

            return _requestMessage;
        }
        private static string SerializePayload<T>(T Payload, bool ChangesOnly = false) where T: IBaseDTOWithIDAndETag
        {
            var opt = new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                NumberHandling = JsonNumberHandling.WriteAsString,
                AllowTrailingCommas = true,
                IncludeFields = true
            };
            opt.Converters.Add(new CustomODataByteConverter());
            opt.Converters.Add(new CustomODataDateConverter());

            string output = ""; 
            if(!ChangesOnly) 
            {
                // This may need some extra formatting here to get the correct output structure that SAP Expects an ODATA PATCH to be in.
                // For now it's just sending a Dictionary of changes.
                output = JsonSerializer.Serialize<Dictionary<string,object>>(Payload.GetChangeLog(), opt);
            }
            else
            {
                output = JsonSerializer.Serialize<T>(Payload, opt);
            }
            return output;

        }
        private async ValueTask<HttpResponseMessage> ExecuteHttpRequest(HttpRequestMessage _requestMessage)
        {
            using (HttpClient _client = _clientFactory.CreateClient(_Options.NamedHttpClientName))
            {
                string debug = "";
                HttpResponseMessage _resp;
                try
                {
                    _client.BaseAddress = new Uri(_Options.NamedHttpClientBaseUri);
                    _resp = await _client.SendAsync(_requestMessage);
                    debug = await _resp.Content.ReadAsStringAsync();
                   _resp.EnsureSuccessStatusCode();
                    return _resp;
                }
                catch (HttpRequestException rex)
                {
                    ErrorRoot er = JsonSerializer.Deserialize<ErrorRoot>(debug);
                    throw new Exception($": Application Error in FireRemoteRequestAsync {er.Error.Message.Value}", rex);
                }
                catch (Exception ex)
                {
                   
                    throw new Exception($": Protocol Error sending request to {_requestMessage.RequestUri} - {debug}", ex);
                }
            }
        }

        private static JsonSerializerOptions GetDeserializerOptions()
        {
            var opt = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                AllowTrailingCommas = true,
                IncludeFields = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
            opt.Converters.Add(new CustomODataDateConverter());
            opt.Converters.Add(new CustomODataByteConverter());
            // opt.Converters.Add(new DeferredEnumerableConverter<BaseDTOWithIDAndETag>());
            return opt;
        }
        private static void ETagMe<V,T>(HttpResponseMessage _resp, V ret) where T : IBaseDTOWithIDAndETag where V : IBaseDTOWithIDAndETag
        {
            // Strip the etag header etc from the resource and set the ETAG property on the wrapped object
            if (_resp.Headers.Contains("ETag"))
            {
                ret.eTag = _resp.Headers.ETag.Tag;
            }
        }

        public virtual async ValueTask FireRemoteRequestAsyncWithEtagWithNoReturn<T>(T Payload, string tailUri = "/", HttpMethod method = null, 
        Dictionary<string, object> KeyParams = null, Dictionary<string, string> CustomHeaders = null) where T : IBaseDTOWithIDAndETag
        {
            HttpRequestMessage _requestMessage;
            // Let's think about if this is a patch request, if so then we only want to send back the changes to the object
            switch (method?.Method ?? "NULL") 
            {
                case "PATCH":
                    // Set the true flag for SerializePayload to only send the changes to the client 
                    _requestMessage = await BuildHttpRequestMessageAsync(SerializePayload(Payload, true), tailUri, method, KeyParams, CustomHeaders, Payload.eTag);
                    break;
                
                default: 
                    _requestMessage = await BuildHttpRequestMessageAsync(NullWrapSerializePayload(Payload), tailUri, method, KeyParams, CustomHeaders, Payload.eTag);
                    break;
            }
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            // If there is no response body, then this is probably an update or create request, so capture the etag
            if (_resp.Headers.Contains("ETag")){
                Payload.eTag = _resp.Headers.ETag.Tag;
            }
        }
        
        public virtual async ValueTask FireRemoteRequestAsyncWithEtagWithNoReturnOrPayload<T>(string etag, string tailUri = "/", HttpMethod method = null, 
        Dictionary<string, object> KeyParams = null, Dictionary<string, string> CustomHeaders = null) where T : IBaseDTOWithIDAndETag
        {
            HttpRequestMessage _requestMessage = await BuildHttpRequestMessageAsync("", tailUri, method, KeyParams, CustomHeaders, etag);
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            // The only type of operation that is likely to require this activity is a DELETE request, so don't bother to capture the etag
        }
        public virtual async ValueTask<T> FireRemoteRequestAsyncWithNoPayloadAndETagOut<T>(string tailUri = "/", HttpMethod method = null, Dictionary<string, object> KeyParams = null, Dictionary<string, string> CustomHeaders = null, string etag = "")
            where T : IBaseDTOWithIDAndETag
        {
            HttpRequestMessage _requestMessage = await BuildHttpRequestMessageAsync(NullWrapSerializePayload<T>(default(T)), tailUri, method, KeyParams, CustomHeaders, "");
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            // If there is no response body, then this is probably an update or create request, so capture the etag
            var Payload = (await DeserializeAsyncResponse<RootSingle<T>>(_resp)).d;   
            if (_resp.Headers.Contains("ETag")){
                Payload.eTag = _resp.Headers.ETag.Tag;
            }
            return Payload;
        }

        public virtual async ValueTask<T> FireRemoteRequestAsyncWithEtag<T>(T payload, string tailUri = "/", HttpMethod method = null, Dictionary<string, object> KeyParams = null, Dictionary<string, string> CustomHeaders = null) where T : IBaseDTOWithIDAndETag
        {
       
            HttpRequestMessage _requestMessage = await BuildHttpRequestMessageAsync(NullWrapSerializePayload(payload), tailUri, method, KeyParams, CustomHeaders);
            HttpResponseMessage _resp = await ExecuteHttpRequest(_requestMessage);
            return await DeserializeAsyncResponse<T>(_resp);
        
        }
    }
}
