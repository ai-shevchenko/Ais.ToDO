using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ais.ToDo.Api.Converters;

internal sealed class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetDateTimeOffset();
        return value.ToUniversalTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        var localTime = value.ToLocalTime();
        writer.WriteStringValue(localTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
    }
}