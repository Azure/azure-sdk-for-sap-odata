// ODataSAPDto Class
//------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace DataOperations.OData.Serialization
{
    public class DList<T> where T : IBaseDTOWithIDAndETag
    {
        [JsonPropertyName("results")]
        public List<T> results;
    }
 
}