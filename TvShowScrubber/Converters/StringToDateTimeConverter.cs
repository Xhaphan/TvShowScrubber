using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TvShowScrubber.Converters
{
    public class StringToDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            if (dateString == null)
                return DateTime.MinValue;

            return DateTime.ParseExact(reader.GetString() ?? string.Empty, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
