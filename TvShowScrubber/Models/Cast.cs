using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TvShowScrubber.Converters;

namespace TvShowScrubber.Models
{
    public class Cast
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [JsonPropertyName("id")]
        public int PersonId { get; set; }

        public int ShowId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("birthday")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTime Birthday { get; set; }
    }
}
