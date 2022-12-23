namespace DataOperations
{
    public interface IOperationsDispatcher
    {
        // GET /T('ID')
        Task<T> GetAsync<T>(string ID, QueryExpand expand = null, QuerySelect select = null) where T : IBaseDTOWithIDAndETag;

        // GET /T?$Filter=key eq values
        Task<T> GetAsync<T>(Dictionary<string, object> keyValues)  where T : IBaseDTOWithIDAndETag;

        // GET /List<T>?$top=10&$skip=10&$orderby=key desc&$filter=key eq values&$expand=key
        Task<IEnumerable<T>> GetListAsync<T>(QueryTop top = null, QuerySkip skip = null, QueryOrderBy orderBy = null, QueryFilter filter = null, QueryExpand expand = null, QuerySelect select = null) where T : IBaseDTOWithIDAndETag;

        // GET List<T>?$Filter=key eq value
        Task<IEnumerable<T>> GetListAsync<T>(Dictionary<string, object> KeyValues) where T : IBaseDTOWithIDAndETag;

        // POST /T
        Task<T> CreateAsync<T>(T entity) where T : IBaseDTOWithIDAndETag;

        // PATCH /T('ID')
        Task UpdateAsync<T>(T entity) where T : IBaseDTOWithIDAndETag;

        // DELETE /T('ID')
        Task DeleteAsync<T>(string ID, string eTag)  where T : IBaseDTOWithIDAndETag; 

        // This lets us build an RPC-Type interface and execute function imports. 
        Task<V> ExecuteFunctionImportAsync<T, V>(T payload, string functionImportPath, Dictionary<string, object> parameters) where T : IBaseDTOWithIDAndETag where V : IBaseDTOWithIDAndETag;
                
        // This lets us build an RPC-Type interface and execute function imports. 
        Task<object> ExecuteFunctionImportAsync<T>(string functionImportPath, Dictionary<string, object> parameters) where T : IBaseDTOWithIDAndETag;

        // Batching config and setup
        Task<BatchResult> ExecuteBatchAsync(IEnumerable<BatchChangeItemSet> requests);

        // Builds the String to send to the client
        string BuildUriQueryClauses(QueryTop top = null, QuerySkip skip = null, QueryOrderBy orderBy = null, QueryFilter filter = null, QueryExpand expand = null, QuerySelect select = null);
        Task<T> DispatchThroughDeferredURL<T>(string uri) where T : IBaseDTOWithIDAndETag;
        Task<List<T>> DispatchThroughDeferredUrlIEnumerable<T>(string uri) where T : IBaseDTOWithIDAndETag;

    }
}