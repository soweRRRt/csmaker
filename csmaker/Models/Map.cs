using System.Collections.Generic;

namespace csmaker.Models;

/// <summary>
/// Карта CS2
/// </summary>
public class Map
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public MapType Type { get; set; }

    /// <summary>
    /// Стандартные карты CS2 активного пула
    /// </summary>
    public static readonly List<Map> ActiveDutyMaps = new()
    {
        new Map { Name = "de_ancient", DisplayName = "Ancient", Type = MapType.Defuse },
        new Map { Name = "de_anubis", DisplayName = "Anubis", Type = MapType.Defuse },
        new Map { Name = "de_dust2", DisplayName = "Dust II", Type = MapType.Defuse },
        new Map { Name = "de_inferno", DisplayName = "Inferno", Type = MapType.Defuse },
        new Map { Name = "de_mirage", DisplayName = "Mirage", Type = MapType.Defuse },
        new Map { Name = "de_nuke", DisplayName = "Nuke", Type = MapType.Defuse },
        new Map { Name = "de_vertigo", DisplayName = "Vertigo", Type = MapType.Defuse }
    };

    /// <summary>
    /// Получить карту по имени
    /// </summary>
    public static Map? GetMapByName(string name)
    {
        return ActiveDutyMaps.Find(m => m.Name == name || m.DisplayName == name);
    }
}

public enum MapType
{
    Defuse,    // de_ карты (обезвреживание бомбы)
    Hostage    // cs_ карты (заложники)
}

/// <summary>
/// Результат одной карты в матче
/// </summary>
public class MapResult
{
    public Map Map { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public Team Winner { get; set; }
    public Team Loser { get; set; }

    /// <summary>
    /// Детальная информация по раундам
    /// </summary>
    public List<RoundResult> Rounds { get; set; } = new();

    /// <summary>
    /// Счет в первом тайме (Team1 CT)
    /// </summary>
    public int Team1FirstHalfScore { get; set; }
    public int Team2FirstHalfScore { get; set; }

    /// <summary>
    /// Счет во втором тайме (Team1 T)
    /// </summary>
    public int Team1SecondHalfScore { get; set; }
    public int Team2SecondHalfScore { get; set; }

    /// <summary>
    /// Был ли овертайм
    /// </summary>
    public bool WentToOvertime { get; set; }

    /// <summary>
    /// Количество овертаймов
    /// </summary>
    public int OvertimeCount { get; set; }
}

/// <summary>
/// Результат одного раунда
/// </summary>
public class RoundResult
{
    public int RoundNumber { get; set; }
    public Team WinnerTeam { get; set; }
    public RoundWinReason WinReason { get; set; }

    /// <summary>
    /// Какая сторона выиграла раунд
    /// </summary>
    public Side WinningSide { get; set; }
}

public enum RoundWinReason
{
    Elimination,        // Все противники убиты
    BombDefused,        // Бомба обезврежена
    BombExploded,       // Бомба взорвана
    TimeExpired         // Время вышло (победа CT)
}

public enum Side
{
    CT,  // Counter-Terrorists (защита)
    T    // Terrorists (нападение)
}