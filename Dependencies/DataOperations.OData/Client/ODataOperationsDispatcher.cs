using Microsoft.Extensions.Options;
namespace DataOperations.OData.Client
{
    public class ODataOperationsDispatcher : IOperationsDispatcher 
    {
        private IClientContext _context;
        private readonly ODataOperationsDispatcherOptions options;
        public ODataOperationsDispatcher(IClientContext context, IOptions<ODataOperationsDispatcherOptions> options)
        {
            _context = context;
            this.options = options.Value;
        }

        public async Task<T> CreateAsync<T>(T entity) where T : IBaseDTOWithIDAndETag
        {
            // Fire a POST request
            // The payload is the entity to create
            // We expect a 201 Created response with the new entity in the body
            string keyString = $"/{options.ServiceRootPrefix}/{typeof(T).Name}Set";
            return (await _context.FireRemoteRequestAsyncWithEtag<T>(entity, keyString, HttpMethod.Post, null, null));
        }
        public async Task DeleteAsync<T>(string ID, string eTag) where T : IBaseDTOWithIDAndETag
        {
            // Fire a DELETE request, passing in the etag in an If-Match header.
            // We expect a 204 No Content response. 
            string keyString = $"/{options.ServiceRootPrefix}/{typeof(T).Name}Set('{ID}')";
            await _context.FireRemoteRequestAsyncWithEtagWithNoReturnOrPayload<T>(eTag, keyString, HttpMethod.Delete, null, new Dictionary<string,string>(){});
        }
        public async Task<V> ExecuteFunctionImportAsync<T, V>(T payload, string functionImportPath, Dictionary<string, object> parameters) where V : IBaseDTOWithIDAndETag where T : IBaseDTOWithIDAndETag
        {
            string keyString = $"/{options.ServiceRootPrefix}/{functionImportPath}";
            keyString = BuildParameterUrlTail(parameters, keyString);
            return await _context.FireRemoteRequestAsync<T,V>(payload, keyString, HttpMethod.Post, null, null);
        }
        
        public async Task<object> ExecuteFunctionImportAsync<T>(string functionImportPath, Dictionary<string, object> parameters) where T : IBaseDTOWithIDAndETag
        {
            string keyString = $"/{options.ServiceRootPrefix}/{functionImportPath}";
            keyString = BuildParameterUrlTail(parameters, keyString);
            return await _context.FireRemoteRequestAsyncWithSimpleReturn<T>(default(T), keyString, HttpMethod.Post, null, null);
        }
        public static string BuildParameterUrlTail(Dictionary<string, object> parameters, string keyString)
        {
            bool multiples = false;
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                keyString = keyString + (multiples ? "&" : "?") + parameter.Key + "='" + parameter.Value + "'"; 
                multiples = true;
            }
            return keyString;
        }
        public async Task<T> GetAsync<T>(string Key = null, QueryExpand expand = null, QuerySelect select = null)  where T : IBaseDTOWithIDAndETag
        {
            // Generate the OData URL, then Fire a get request
            // grab the D parameter from the wire response and return it.
            // Return a Task<T> with the result.
        
            string keyString = $"/{options.ServiceRootPrefix}/{typeof(T).Name}Set('{Key}')" + BuildUriQueryClauses(null, null, null, null, expand, select);
            return (await _context.FireRemoteRequestAsyncWithNoPayloadAndETagOut<T>(keyString, HttpMethod.Get, null, null));
        }
        public async Task<T> GetAsync<T>(Dictionary<string, object> keyValues) where T : IBaseDTOWithIDAndETag
        {
            return (await GetListAsync<T>(keyValues)).FirstOrDefault();
        }
        public async Task<IEnumerable<T>> GetListAsync<T>(Dictionary<string, object> KeyValues) where T : IBaseDTOWithIDAndETag
        {
            // Build the filters from the provided dictionary
            // Generate the OData URL, then Fire a get request
            // Return a Task<IEnumerable<T>> with the result.
            var qf1 = new List<QueryFilterExpression>();
            foreach (KeyValuePair<string, object> keyValue in KeyValues)
            {
                qf1.Add(new QueryFilterExpression(keyValue.Key , FilterOperator.eq, keyValue.Value.ToString()));
            }
            var args = QueryFilter.FilterFactory(new Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>>(){{FilterConjunctionOperator.root, qf1}});

            string keyString = $"/{options.ServiceRootPrefix}/{typeof(T).Name}Set" + BuildUriQueryClauses(null,null,null, args, null, null);
            return (await _context.FireRemoteRequestAsyncWithNoPayloadAndEnumerable<T, T>( keyString, HttpMethod.Get, null, null));
        }
        public async Task<IEnumerable<T>> GetListAsync<T>(QueryTop top = null, QuerySkip skip = null, QueryOrderBy orderBy = null, QueryFilter filter = null, QueryExpand expand = null, QuerySelect select = null)  where T : IBaseDTOWithIDAndETag 
        {
            // Return a Task<T> with the result.
            // Fire a get request
            string keyString = $"/{options.ServiceRootPrefix}/{typeof(T).Name}Set" + BuildUriQueryClauses(top, skip, orderBy, filter, expand, select);
            return (await _context.FireRemoteRequestAsyncWithNoPayloadAndEnumerable<T, T>(keyString, HttpMethod.Get, null, null));
        }
        public virtual async Task UpdateAsync<T>(T entity) where T : IBaseDTOWithIDAndETag
        {
            // Fire a PATCH request, passing in the etag in an If-Match header.
            // We expect this to return a 204 No Content response, hence the lack of a return value.
            string keyString = $"/{options.ServiceRootPrefix}/{typeof(T).Name}Set('{string.Join(",",entity.GetPrimaryKeyValues().Values)}')" ;
            await _context.FireRemoteRequestAsyncWithEtagWithNoReturn<T>(entity, keyString, HttpMethod.Patch, null, new Dictionary<string,string>() {});
        }
        public virtual string BuildUriQueryClauses(QueryTop top = null, QuerySkip skip = null, QueryOrderBy orderBy = null, QueryFilter filter = null, QueryExpand expand = null, QuerySelect select = null)
        {
            string expandString = "", selectString = "", topString = "", skipString = "", orderByString = "", filterString = "";
            bool multiples = false; 
            if (expand != null)
            {
                expandString = "$expand=" + string.Join(",", expand.Expand);
                multiples = true;
            }
            if (select != null)
            {
                selectString = (multiples ? "&" : "?") + "$select=" + string.Join(",", select.Select);
                multiples = true;
            }
            if (top != null)
            {
                topString = (multiples ? "&" : "?") + "$top=" + top.Top;
                multiples = true;
            }
            if (skip != null)
            {
                skipString = (multiples ? "&" : "?") + "$skip=" + skip.Skip;
                multiples = true;
            }
            if (orderBy != null)
            {
                orderByString = (multiples ? "&" : "?") + "$orderby=" + string.Join(",", orderBy.OrderBy);
                multiples = true;
            }

            if (filter?.IsValid() ?? false)
            {
                filterString = (multiples ? "&" : "?") + filter.Filter.RenderOutputAsFilterClause();
                multiples = true;
            }
            return expandString + selectString + topString + skipString + orderByString + filterString;
        }
        public virtual async Task<T> DispatchThroughDeferredURL<T>(string uri) where T : IBaseDTOWithIDAndETag
        {
            return (await _context.FireRemoteRequestAsyncDirectUri<T>(uri, HttpMethod.Get));
        }
        public virtual async Task<List<T>> DispatchThroughDeferredUrlIEnumerable<T>(string uri) where T : IBaseDTOWithIDAndETag
        {
            return (await _context.FireRemoteRequestAsyncDirectUriIEnumerable<T>(uri, HttpMethod.Get)).ToList<T>();
        }
        public virtual async Task<BatchResult> ExecuteBatchAsync(IEnumerable<BatchChangeItemSet> requests)
        {
            throw new NotImplementedException();
        }


    }

}
