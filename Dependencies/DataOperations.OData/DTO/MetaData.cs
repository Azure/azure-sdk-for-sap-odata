using System.Text.Json.Serialization;

namespace DataOperations.OData.Serialization
{
    public class Metadata
    {
        [JsonPropertyName("id")]
        public string Id;

        [JsonPropertyName("uri")]
        public string Uri;

        [JsonPropertyName("type")]
        public string Type;
    }
}
