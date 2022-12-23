namespace DataOperations
{
    public interface IClientContext
    {
        // called from executefunctionimportasync<TV>,
        ValueTask<V> FireRemoteRequestAsync<T,V>(
            T payload,
            string tailUri = "/", 
            HttpMethod method = null, 
            Dictionary<string, object> KeyParams = null, 
            Dictionary<string, string> CustomHeaders = null
        ) where T : IBaseDTOWithIDAndETag where V : IBaseDTOWithIDAndETag;
        //CreateAsync,
        ValueTask<T> FireRemoteRequestAsyncWithEtag<T>(
            T payload,
            string tailUri = "/", 
            HttpMethod method = null, 
            Dictionary<string, object> KeyParams = null, 
            Dictionary<string, string> CustomHeaders = null
        ) where T : IBaseDTOWithIDAndETag;

        // Executefunctionimportasync<V,
        ValueTask<object> FireRemoteRequestAsyncWithSimpleReturn<T>(
            T payload,
            string tailUri = "/", 
            HttpMethod method = null, 
            Dictionary<string, object> KeyParams = null, 
            Dictionary<string, string> CustomHeaders = null
        ) where T : IBaseDTOWithIDAndETag;

        // GetAsync
        ValueTask<T> FireRemoteRequestAsyncWithNoPayloadAndETagOut<T>(
            string tailUri = "/", HttpMethod method = null, Dictionary<string, object> KeyParams = null, Dictionary<string,string> CustomHeaders = null, string etag = ""
        ) where T : IBaseDTOWithIDAndETag;

        // GetList
        ValueTask<IEnumerable<V>> FireRemoteRequestAsyncWithNoPayloadAndEnumerable<T,V>(
            string tailUri = "/", HttpMethod method = null,  Dictionary<string, object> KeyParams = null, Dictionary<string,string> CustomHeaders = null
        ) where T : IBaseDTOWithIDAndETag where V : IBaseDTOWithIDAndETag;

        // Update        
        ValueTask FireRemoteRequestAsyncWithEtagWithNoReturn<T>(
            T Payload, 
            string tailUri = "/", 
            HttpMethod method = null, 
            Dictionary<string, object> KeyParams = null, 
            Dictionary<string,string> CustomHeaders = null
        ) 
        where T : IBaseDTOWithIDAndETag; 

        // called to delete
        ValueTask FireRemoteRequestAsyncWithEtagWithNoReturnOrPayload<T>(
            string Etag, 
            string tailUri = "/", 
            HttpMethod method = null, 
            Dictionary<string, object> KeyParams = null, 
            Dictionary<string,string> CustomHeaders = null
        ) 
        where T : IBaseDTOWithIDAndETag; 
        // DispatchThroughDeferredURL
        ValueTask<T> FireRemoteRequestAsyncDirectUri<T>(string tailUri = "/", HttpMethod method = null) where T : IBaseDTOWithIDAndETag; 
        // DispatchThroughDeferredUrlIEnumerable
        ValueTask<IEnumerable<T>> FireRemoteRequestAsyncDirectUriIEnumerable<T>(string tailUri = "/", HttpMethod method = null) where T : IBaseDTOWithIDAndETag;
    }
}
