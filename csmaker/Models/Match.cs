using System;
using System.Collections.Generic;
using System.Linq;

namespace csmaker.Models;

/// <summary>
/// Матч между двумя командами
/// </summary>
public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;

    public Team Team1 { get; set; }
    public Team Team2 { get; set; }

    /// <summary>
    /// Формат матча (BO1, BO3, BO5)
    /// </summary>
    public BestOfFormat Format { get; set; }

    /// <summary>
    /// Результаты по картам
    /// </summary>
    public List<MapResult> MapResults { get; set; } = new();

    /// <summary>
    /// Победитель матча
    /// </summary>
    public Team? Winner { get; set; }

    /// <summary>
    /// Проигравший
    /// </summary>
    public Team? Loser { get; set; }

    /// <summary>
    /// Счет по картам (например, 2:1)
    /// </summary>
    public int Team1MapScore => MapResults.Count(mr => mr.Winner == Team1);
    public int Team2MapScore => MapResults.Count(mr => mr.Winner == Team2);

    /// <summary>
    /// Статус матча
    /// </summary>
    public MatchStatus Status { get; set; } = MatchStatus.NotStarted;

    /// <summary>
    /// Название турнира/события (опционально)
    /// </summary>
    public string? TournamentName { get; set; }

    /// <summary>
    /// Карты, которые будут сыграны (пик/бан)
    /// </summary>
    public List<Map> SelectedMaps { get; set; } = new();

    /// <summary>
    /// Проверка, завершен ли матч
    /// </summary>
    public bool IsFinished => Status == MatchStatus.Finished;

    /// <summary>
    /// Получить текущую карту (которая играется сейчас)
    /// </summary>
    public Map? GetCurrentMap()
    {
        if (MapResults.Count < SelectedMaps.Count)
            return SelectedMaps[MapResults.Count];
        return null;
    }

    /// <summary>
    /// Получить номер текущей карты
    /// </summary>
    public int GetCurrentMapNumber()
    {
        return MapResults.Count + 1;
    }
}

public enum BestOfFormat
{
    BO1 = 1,  // Best of 1 (1 карта)
    BO3 = 3,  // Best of 3 (до 2 побед)
    BO5 = 5   // Best of 5 (до 3 побед)
}

public enum MatchStatus
{
    NotStarted,   // Матч не начался
    InProgress,   // Матч идет
    Finished,     // Матч завершен
    Cancelled     // Матч отменен
}

/// <summary>
/// Статистика команды в матче
/// </summary>
public class MatchStatistics
{
    public Team Team { get; set; }
    public Match Match { get; set; }

    /// <summary>
    /// Общее количество раундов выиграно
    /// </summary>
    public int TotalRoundsWon { get; set; }

    /// <summary>
    /// Общее количество раундов проиграно
    /// </summary>
    public int TotalRoundsLost { get; set; }

    /// <summary>
    /// Раунды выиграны как CT
    /// </summary>
    public int CTRoundsWon { get; set; }

    /// <summary>
    /// Раунды выиграны как T
    /// </summary>
    public int TRoundsWon { get; set; }

    /// <summary>
    /// Карты выиграно
    /// </summary>
    public int MapsWon { get; set; }

    /// <summary>
    /// Карты проиграно
    /// </summary>
    public int MapsLost { get; set; }

    /// <summary>
    /// Винрейт по раундам в процентах
    /// </summary>
    public double RoundWinRate
    {
        get
        {
            var total = TotalRoundsWon + TotalRoundsLost;
            return total > 0 ? (double)TotalRoundsWon / total * 100 : 0;
        }
    }
}