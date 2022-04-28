using System.Text.Json.Serialization;

namespace TvShowScrubber.Models
{
    public class ShowResponse : Show
    {
        [JsonPropertyOrder(3)]
        public List<Cast> Cast { get; set; }
    }
}
