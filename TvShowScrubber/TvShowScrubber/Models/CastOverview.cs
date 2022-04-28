namespace TvShowScrubber.Models
{
    public class CastOverview
    {
        [System.Text.Json.Serialization.JsonPropertyName("person")]
        public Cast Person { get; set; }
    }
}
