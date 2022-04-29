using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TvShowScrubber.Models
{
    public class CastOverview
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [JsonPropertyName("person")]
        public Cast Person { get; set; }
    }
}
