using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TvShowScrubber.Models
{
    public class EmbeddedCast
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [JsonPropertyName("cast")]
        public List<CastOverview> CastOverviews { get; set; }
    }
}