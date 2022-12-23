// ODataSAPDto Class
//------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace DataOperations.OData.Serialization
{

    public class RootSingle<T> where T : IBaseDTOWithIDAndETag
    {
        [JsonPropertyName("d")]
        public T d;
    }
 
}