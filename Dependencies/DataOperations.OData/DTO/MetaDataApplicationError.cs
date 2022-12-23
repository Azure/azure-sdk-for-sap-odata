using System.Text.Json.Serialization;
namespace DataOperations.OData.Deserialization
{
    public class Application
    {
        [JsonPropertyName("component_id")]
        public string ComponentId { get; set; }

        [JsonPropertyName("service_namespace")]
        public string ServiceNamespace { get; set; }

        [JsonPropertyName("service_id")]
        public string ServiceId { get; set; }

        [JsonPropertyName("service_version")]
        public string ServiceVersion { get; set; }
    }
    public class Error
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("innererror")]
        public Innererror Innererror { get; set; }
    }
    public class Errordetail
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("propertyref")]
        public string Propertyref { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }
    }
    public class ErrorResolution
    {
        [JsonPropertyName("SAP_Transaction")]
        public string SAPTransaction { get; set; }

        [JsonPropertyName("SAP_Note")]
        public string SAPNote { get; set; }
    }
    public class Innererror
    {
        [JsonPropertyName("application")]
        public Application Application { get; set; }

        [JsonPropertyName("transactionid")]
        public string Transactionid { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("Error_Resolution")]
        public ErrorResolution ErrorResolution { get; set; }

        [JsonPropertyName("errordetails")]
        public List<Errordetail> Errordetails { get; set; }
    }
    public class Message
    {
        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
    public class ErrorRoot
    {
        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }
}