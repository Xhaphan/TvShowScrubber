using System.Text.Json.Serialization;

namespace TvShowScrubber.Models
{
    public class ShowResponse : Show
    {
        [JsonPropertyOrder(50)]
        public List<Cast> Cast { get; set; }
    }
}
