using System.Text.Json.Serialization;

namespace csmaker.Models;

public class Player
{
    public string Nickname { get; set; }
    public string Country { get; set; }

    [JsonIgnore]
    public Team Team { get; set; }

    public string? TeamName { get; set; }
}

