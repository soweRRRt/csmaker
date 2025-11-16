using csmaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csmaker.Services;

/// <summary>
/// Сервис для управления матчами
/// </summary>
public static class MatchService
{
    private static List<Match> _matches = new();

    /// <summary>
    /// Все сыгранные матчи
    /// </summary>
    public static List<Match> Matches => _matches;

    /// <summary>
    /// Создать новый матч
    /// </summary>
    public static Match CreateMatch(Team team1, Team team2, BestOfFormat format, List<Map>? maps = null)
    {
        var match = new Match
        {
            Team1 = team1,
            Team2 = team2,
            Format = format,
            Status = MatchStatus.NotStarted
        };

        if (maps != null && maps.Count > 0)
        {
            match.SelectedMaps = maps;
        }
        else
        {
            // Пик случайных карт
            match.SelectedMaps = PickMapsForMatch(format);
        }

        _matches.Add(match);
        return match;
    }

    /// <summary>
    /// Сыграть матч (симуляция)
    /// </summary>
    public static void PlayMatch(Match match)
    {
        if (match.Status != MatchStatus.NotStarted)
            throw new InvalidOperationException("Матч уже начался или завершен");

        match.Status = MatchStatus.InProgress;

        // Симулируем каждую карту
        int mapsToWin = GetMapsToWin(match.Format);
        int team1Wins = 0;
        int team2Wins = 0;

        foreach (var map in match.SelectedMaps)
        {
            if (team1Wins >= mapsToWin || team2Wins >= mapsToWin)
                break;

            var mapResult = MatchSimulator.SimulateMap(map, match.Team1, match.Team2);
            match.MapResults.Add(mapResult);

            if (mapResult.Winner == match.Team1)
                team1Wins++;
            else
                team2Wins++;
        }

        // Определяем победителя
        if (team1Wins > team2Wins)
        {
            match.Winner = match.Team1;
            match.Loser = match.Team2;
        }
        else
        {
            match.Winner = match.Team2;
            match.Loser = match.Team1;
        }

        match.Status = MatchStatus.Finished;

        // Обновляем рейтинги команд
        UpdateTeamRatings(match);
    }

    /// <summary>
    /// Обновить рейтинги команд после матча
    /// </summary>
    private static void UpdateTeamRatings(Match match)
    {
        if (match.Winner == null || match.Loser == null)
            return;

        // Считаем общий счет по раундам
        int winnerTotalRounds = 0;
        int loserTotalRounds = 0;

        foreach (var mapResult in match.MapResults)
        {
            if (mapResult.Winner == match.Winner)
            {
                winnerTotalRounds += mapResult.Winner == match.Team1 ? mapResult.Team1Score : mapResult.Team2Score;
                loserTotalRounds += mapResult.Loser == match.Team1 ? mapResult.Team1Score : mapResult.Team2Score;
            }
        }

        // Применяем изменение VRS рейтинга
        VrsRatingService.ApplyMatchResult(match.Winner, match.Loser, winnerTotalRounds, loserTotalRounds);
    }

    /// <summary>
    /// Получить матчи команды
    /// </summary>
    public static List<Match> GetTeamMatches(Team team)
    {
        return _matches.Where(m => m.Team1 == team || m.Team2 == team).ToList();
    }

    /// <summary>
    /// Получить последние матчи команды
    /// </summary>
    public static List<Match> GetRecentTeamMatches(Team team, int count = 5)
    {
        return _matches
            .Where(m => (m.Team1 == team || m.Team2 == team) && m.IsFinished)
            .OrderByDescending(m => m.Date)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Получить статистику команды по всем матчам
    /// </summary>
    public static MatchStatistics GetTeamStatistics(Team team)
    {
        var teamMatches = _matches.Where(m =>
            (m.Team1 == team || m.Team2 == team) && m.IsFinished).ToList();

        var stats = new MatchStatistics
        {
            Team = team,
            Match = null
        };

        foreach (var match in teamMatches)
        {
            bool isTeam1 = match.Team1 == team;

            foreach (var mapResult in match.MapResults)
            {
                int teamScore = isTeam1 ? mapResult.Team1Score : mapResult.Team2Score;
                int opponentScore = isTeam1 ? mapResult.Team2Score : mapResult.Team1Score;

                stats.TotalRoundsWon += teamScore;
                stats.TotalRoundsLost += opponentScore;

                if (mapResult.Winner == team)
                    stats.MapsWon++;
                else
                    stats.MapsLost++;
            }
        }

        return stats;
    }

    /// <summary>
    /// Получить H2H статистику между двумя командами
    /// </summary>
    public static (int team1Wins, int team2Wins, int totalMatches) GetHeadToHead(Team team1, Team team2)
    {
        var h2hMatches = _matches.Where(m =>
            ((m.Team1 == team1 && m.Team2 == team2) || (m.Team1 == team2 && m.Team2 == team1))
            && m.IsFinished).ToList();

        int team1Wins = h2hMatches.Count(m => m.Winner == team1);
        int team2Wins = h2hMatches.Count(m => m.Winner == team2);

        return (team1Wins, team2Wins, h2hMatches.Count);
    }

    /// <summary>
    /// Выбрать карты для матча (пик/бан симуляция)
    /// </summary>
    private static List<Map> PickMapsForMatch(BestOfFormat format)
    {
        var random = new Random();
        var availableMaps = new List<Map>(Map.ActiveDutyMaps);
        var selectedMaps = new List<Map>();

        int mapsNeeded = (int)format;

        for (int i = 0; i < mapsNeeded; i++)
        {
            if (availableMaps.Count == 0)
                break;

            int index = random.Next(availableMaps.Count);
            selectedMaps.Add(availableMaps[index]);
            availableMaps.RemoveAt(index);
        }

        return selectedMaps;
    }

    /// <summary>
    /// Получить количество карт для победы
    /// </summary>
    private static int GetMapsToWin(BestOfFormat format)
    {
        return ((int)format / 2) + 1;
    }

    /// <summary>
    /// Удалить все матчи (для сброса)
    /// </summary>
    public static void ClearMatches()
    {
        _matches.Clear();
    }

    /// <summary>
    /// Получить форматированный результат матча
    /// </summary>
    public static string GetMatchSummary(Match match)
    {
        if (!match.IsFinished)
            return "Матч не завершен";

        var summary = $"{match.Team1.Name} vs {match.Team2.Name}\n";
        summary += $"Формат: BO{(int)match.Format}\n";
        summary += $"Счет по картам: {match.Team1MapScore}:{match.Team2MapScore}\n";
        summary += $"Победитель: {match.Winner?.Name}\n\n";
        summary += "Результаты по картам:\n";

        foreach (var mapResult in match.MapResults)
        {
            summary += $"  {mapResult.Map.DisplayName}: {mapResult.Team1Score}:{mapResult.Team2Score}";
            if (mapResult.WentToOvertime)
                summary += $" (OT x{mapResult.OvertimeCount})";
            summary += "\n";
        }

        return summary;
    }
}