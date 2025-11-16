using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace csmaker.Models;

public class Team
{
    public string Name { get; set; }

    /// <summary>
    /// VRS рейтинг команды (по умолчанию 1000)
    /// </summary>
    public int VrsRating { get; set; } = 1000;

    /// <summary>
    /// Количество сыгранных матчей
    /// </summary>
    public int MatchesPlayed { get; set; } = 0;

    /// <summary>
    /// Количество побед
    /// </summary>
    public int Wins { get; set; } = 0;

    /// <summary>
    /// Количество поражений
    /// </summary>
    public int Losses { get; set; } = 0;

    /// <summary>
    /// Количество ничьих (если будут)
    /// </summary>
    public int Draws { get; set; } = 0;

    [JsonIgnore]
    public List<Player> Players { get; set; } = new();

    /// <summary>
    /// Винрейт команды в процентах
    /// </summary>
    [JsonIgnore]
    public double WinRate => MatchesPlayed > 0
        ? (double)Wins / MatchesPlayed * 100
        : 0;

    /// <summary>
    /// Форма команды (последние 5 матчей: W/L/D)
    /// </summary>
    public List<string> RecentForm { get; set; } = new();

    public bool AddPlayer(Player player)
    {
        if (Players.Count >= 5)
            return false;

        if (!Players.Contains(player))
        {
            Players.Add(player);
            player.Team = this;
            player.TeamName = this.Name;
        }
        return true;
    }

    public void RemovePlayer(Player player)
    {
        if (Players.Remove(player))
        {
            player.Team = null;
            player.TeamName = null;
        }
    }

    /// <summary>
    /// Добавить результат матча в форму команды
    /// </summary>
    public void AddMatchResult(string result)
    {
        RecentForm.Add(result);

        // Храним только последние 5 результатов
        if (RecentForm.Count > 5)
            RecentForm.RemoveAt(0);
    }
}