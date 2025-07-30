using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GIL_Agent_Portal.Healper
{
    public class DdMmYyyyDateConverter : JsonConverter<DateTime?>
    {
        private static readonly string[] formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return null;

            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;

            throw new JsonException($"Invalid date format. Expected dd/MM/yyyy or yyyy-MM-dd, got '{s}'");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("dd/MM/yyyy"));
            else
                writer.WriteNullValue();
        }
    }
}
