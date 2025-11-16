using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace csmaker.Models;

public class GameData
{
    public List<Player> Players { get; set; } = new();
    public List<Team> Teams { get; set; } = new();

    public void Save(string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        foreach (var player in Players)
        {
            player.TeamName = player.Team?.Name;
        }

        File.WriteAllText(path, JsonSerializer.Serialize(this, options));
    }

    public static GameData Load(string path)
    {
        if (!File.Exists(path))
            return new GameData();

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<GameData>(json) ?? new GameData();

        var teamDict = data.Teams.ToDictionary(t => t.Name);

        foreach (var player in data.Players)
        {
            if (!string.IsNullOrEmpty(player.TeamName) &&
                teamDict.TryGetValue(player.TeamName, out var team))
            {
                player.Team = team;
                team.Players.Add(player);
            }
        }

        return data;
    }
}
