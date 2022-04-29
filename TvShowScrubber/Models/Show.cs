using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TvShowScrubber.Models;

public class Show
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}