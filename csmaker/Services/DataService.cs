using csmaker.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace csmaker.Services;

public static class DataService
{
    private static readonly string DataFilePath = GetDataFilePath();

    private static string GetDataFilePath()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string projectDir = Directory.GetParent(baseDir).Parent.Parent.FullName;
        string dataDir = Path.Combine(projectDir, "Data");

        if (!Directory.Exists(dataDir))
            Directory.CreateDirectory(dataDir);

        return Path.Combine(dataDir, "gameData.json");
    }

    public static GameData GameData { get; private set; }
    public static List<Team> Teams => GameData.Teams;
    public static List<Player> Players => GameData.Players;

    static DataService()
    {
        Initialize();
    }

    private static void Initialize()
    {
        if (File.Exists(DataFilePath))
        {
            GameData = GameData.Load(DataFilePath);
        }
        else
        {
            GameData = CreateDefaultData();
            Save();
        }
    }

    private static GameData CreateDefaultData()
    {
        var data = new GameData();

        var team1 = new Team { Name = "Natus Vincere" };
        var team2 = new Team { Name = "Falcons" };

        var p1 = new Player { Nickname = "s1mple", Country = "UA" };
        var p2 = new Player { Nickname = "NiKo", Country = "BA" };

        team1.AddPlayer(p1);
        team2.AddPlayer(p2);

        data.Teams.AddRange(new[] { team1, team2 });
        data.Players.AddRange(new[] { p1, p2 });

        return data;
    }

    public static void Save()
    {
        try
        {
            GameData.Save(DataFilePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
            throw;
        }
    }

    public static void Reload()
    {
        if (File.Exists(DataFilePath))
        {
            GameData = GameData.Load(DataFilePath);
        }
    }

    public static void Reset()
    {
        GameData = CreateDefaultData();
        Save();
    }
}