using csmaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csmaker.Services;

/// <summary>
/// Вспомогательные методы для работы с рейтингом команд
/// </summary>
public static class RatingHelpers
{
    /// <summary>
    /// Получить ранг команды на основе VRS рейтинга
    /// </summary>
    public static string GetTeamRank(int vrsRating)
    {
        return vrsRating switch
        {
            >= 2000 => "Легендарная",
            >= 1800 => "Элитная",
            >= 1600 => "Профессиональная",
            >= 1400 => "Продвинутая",
            >= 1200 => "Средняя",
            >= 1000 => "Начинающая",
            _ => "Новичок"
        };
    }

    /// <summary>
    /// Получить топ команд по рейтингу
    /// </summary>
    public static List<Team> GetTopTeams(int count = 10)
    {
        return DataService.Teams
            .OrderByDescending(t => t.VrsRating)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Получить позицию команды в общем рейтинге
    /// </summary>
    public static int GetTeamPosition(Team team)
    {
        var sortedTeams = DataService.Teams
            .OrderByDescending(t => t.VrsRating)
            .ToList();

        return sortedTeams.IndexOf(team) + 1;
    }

    /// <summary>
    /// Получить статистику команды в виде строки
    /// </summary>
    public static string GetTeamStats(Team team)
    {
        var position = GetTeamPosition(team);
        var rank = GetTeamRank(team.VrsRating);

        return $"Место: #{position} | Ранг: {rank} | " +
               $"VRS: {team.VrsRating} | " +
               $"Матчей: {team.MatchesPlayed} | " +
               $"Побед: {team.Wins} ({team.WinRate:F1}%)";
    }

    /// <summary>
    /// Получить среднюю силу команд в регионе/группе
    /// </summary>
    public static double GetAverageRating(IEnumerable<Team> teams)
    {
        var teamsList = teams.ToList();
        if (!teamsList.Any())
            return 0;

        return teamsList.Average(t => t.VrsRating);
    }

    /// <summary>
    /// Получить цвет для отображения рейтинга
    /// </summary>
    public static System.Drawing.Color GetRatingColor(int vrsRating)
    {
        return vrsRating switch
        {
            >= 2000 => System.Drawing.Color.FromArgb(255, 215, 0),   // Золотой
            >= 1800 => System.Drawing.Color.FromArgb(192, 192, 192), // Серебряный
            >= 1600 => System.Drawing.Color.FromArgb(205, 127, 50),  // Бронзовый
            >= 1400 => System.Drawing.Color.FromArgb(138, 43, 226),  // Фиолетовый
            >= 1200 => System.Drawing.Color.FromArgb(30, 144, 255),  // Синий
            >= 1000 => System.Drawing.Color.FromArgb(50, 205, 50),   // Зеленый
            _ => System.Drawing.Color.Gray                            // Серый
        };
    }

    /// <summary>
    /// Проверить, является ли матч "апсетом" (неожиданной победой)
    /// </summary>
    public static bool IsUpset(Team winner, Team loser)
    {
        // Апсет - когда побеждает команда с рейтингом на 200+ очков ниже
        return winner.VrsRating < loser.VrsRating - 200;
    }

    /// <summary>
    /// Получить предполагаемый счет матча на основе рейтингов
    /// </summary>
    public static (int team1Score, int team2Score) GetPredictedScore(Team team1, Team team2, int maxRounds = 16)
    {
        double winProb = VrsRatingService.GetWinProbability(team1, team2) / 100.0;

        // Предполагаемый счет на основе вероятности
        int team1Score = (int)Math.Round(maxRounds * winProb);
        int team2Score = maxRounds - team1Score;

        return (team1Score, team2Score);
    }

    /// <summary>
    /// Рассчитать "силу расписания" команды (средний рейтинг соперников)
    /// Будет использоваться позже, когда добавим историю матчей
    /// </summary>
    public static double CalculateStrengthOfSchedule(Team team, List<Team> opponents)
    {
        if (!opponents.Any())
            return 0;

        return opponents.Average(t => t.VrsRating);
    }

    /// <summary>
    /// Получить эмодзи для формы команды
    /// </summary>
    public static string GetFormEmoji(string result)
    {
        return result switch
        {
            "W" => "✅",
            "L" => "❌",
            "D" => "➖",
            _ => "❓"
        };
    }

    /// <summary>
    /// Форматировать форму команды с эмодзи
    /// </summary>
    public static string FormatTeamForm(Team team)
    {
        if (!team.RecentForm.Any())
            return "Нет истории";

        return string.Join(" ", team.RecentForm.Select(GetFormEmoji));
    }
}