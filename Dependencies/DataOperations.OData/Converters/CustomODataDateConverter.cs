using System.Text.Json;
using System.Text.Json.Serialization;
namespace DataOperations.OData.Serialization
{
    public class CustomODataDateConverter : JsonConverter<DateTime>
    {
        public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
        {
            // Write as a JSON Date object, the value is the date in milliseconds since Unix Epoch.
            // Unix EPOCH time = Date(0) = Thu Jan 01 1970 00:00:00 GMT, 
            // so we need to find the number of milliseconds since Unix Epoch to get the value to output.
            writer.WriteStringValue("/Date(" + date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString() + ")/");
            
         }
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // is represented as a JSON Date object, the value is the date in milliseconds since Unix Epoch.
            // Unix EPOCH time = Date(0) = Thu Jan 01 1970 00:00:00 GMT, 
            // so we need to add the number of milliseconds since Unix Epoch to get the actual date.
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(double.Parse(reader.GetString().Replace("/Date(","").Replace(")/","")));

        }
    }
    
}
