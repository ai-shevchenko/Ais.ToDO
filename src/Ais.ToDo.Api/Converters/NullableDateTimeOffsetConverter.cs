using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ais.ToDo.Api.Converters;

internal sealed class NullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        var value = reader.GetDateTimeOffset();
        return value.ToUniversalTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            var localTime = value.Value.ToLocalTime();
            writer.WriteStringValue(localTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
            
            return;
        }
        
        writer.WriteNullValue();
    }
}