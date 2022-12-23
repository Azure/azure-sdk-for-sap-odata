
using System.Net.Http;

namespace DataOperations
{
    public interface IBatchChangeItem
    {
        object Entity { get; set; }
        HttpMethod Method { get; set; }
        string Uri { get; set; }
    }

    public class BatchChangeItem : IBatchChangeItem
    {
        // object entity, HttpMethod method, string uri
        public object Entity { get; set; }
        public HttpMethod Method { get; set; }
        public string Uri { get; set; }
        public BatchChangeItem(object entity, HttpMethod method, string uri)
        {
            Entity = entity;
            Method = method;
            Uri = uri;
        }
    }
}