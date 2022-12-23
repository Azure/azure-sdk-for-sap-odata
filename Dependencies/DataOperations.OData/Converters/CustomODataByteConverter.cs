using System.Text.Json;
using System.Text.Json.Serialization;
namespace DataOperations.OData.Serialization
{
    public class CustomODataByteConverter : JsonConverter<byte>
    {
        public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetByte();
        }

        public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    
}
