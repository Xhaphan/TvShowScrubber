using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TvShowScrubber.Models;

public class Show
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("id")]
    public int ShowId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}