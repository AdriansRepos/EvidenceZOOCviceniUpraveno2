using System.Text.Json;
using System.Text.Json.Serialization;


namespace EvidenceZOOCviceniUpraveno2
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions o)
        => DateOnly.Parse(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions o)
            => writer.WriteStringValue(value.ToString("dd-MM-yyyy"));
    }
}