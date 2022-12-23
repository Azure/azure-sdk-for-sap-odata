// ODataSAPDto Class
//------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace DataOperations.OData.Serialization
{
    public class RootList<T> where T : IBaseDTOWithIDAndETag
    {
        [JsonPropertyName("d")]
        public DList<T> d;
    }
 
}