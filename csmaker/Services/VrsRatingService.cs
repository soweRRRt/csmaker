using csmaker.Models;
using System;

namespace csmaker.Services;

/// <summary>
/// Сервис для расчета VRS рейтинга команд
/// VRS (Victory Rating System) - система рейтинга похожая на ELO
/// </summary>
public static class VrsRatingService
{
    // Базовый K-фактор (чувствительность изменения рейтинга)
    private const int BaseKFactor = 32;

    // K-фактор для новых команд (менее 30 матчей)
    private const int NewTeamKFactor = 40;

    // Множитель за разгром (разница раундов >= 10)
    private const double BlowoutMultiplier = 1.3;

    // Множитель за близкий матч (разница раундов <= 3)
    private const double CloseMatchMultiplier = 0.9;

    /// <summary>
    /// Рассчитывает изменение рейтинга после матча
    /// </summary>
    /// <param name="winnerTeam">Команда-победитель</param>
    /// <param name="loserTeam">Команда-проигравший</param>
    /// <param name="winnerScore">Счет победителя</param>
    /// <param name="loserScore">Счет проигравшего</param>
    /// <returns>Кортеж: (изменение рейтинга победителя, изменение рейтинга проигравшего)</returns>
    public static (int winnerChange, int loserChange) CalculateRatingChange(
        Team winnerTeam,
        Team loserTeam,
        int winnerScore,
        int loserScore)
    {
        // K-фактор зависит от количества сыгранных матчей
        int winnerK = GetKFactor(winnerTeam.MatchesPlayed);
        int loserK = GetKFactor(loserTeam.MatchesPlayed);

        // Ожидаемая вероятность победы для каждой команды
        double expectedWinner = GetExpectedScore(winnerTeam.VrsRating, loserTeam.VrsRating);
        double expectedLoser = GetExpectedScore(loserTeam.VrsRating, winnerTeam.VrsRating);

        // Множитель в зависимости от разницы счета
        double scoreMultiplier = GetScoreMultiplier(winnerScore, loserScore);

        // Расчет изменения рейтинга
        int winnerChange = (int)Math.Round(winnerK * (1 - expectedWinner) * scoreMultiplier);
        int loserChange = -(int)Math.Round(loserK * (0 - expectedLoser) * scoreMultiplier);

        // Минимальное изменение - 1 очко
        winnerChange = Math.Max(1, winnerChange);
        loserChange = Math.Min(-1, loserChange);

        return (winnerChange, loserChange);
    }

    /// <summary>
    /// Применяет изменение рейтинга к командам после матча
    /// </summary>
    public static void ApplyMatchResult(
        Team winnerTeam,
        Team loserTeam,
        int winnerScore,
        int loserScore)
    {
        var (winnerChange, loserChange) = CalculateRatingChange(
            winnerTeam, loserTeam, winnerScore, loserScore);

        // Обновляем рейтинг
        winnerTeam.VrsRating += winnerChange;
        loserTeam.VrsRating += loserChange;

        // Обновляем статистику
        winnerTeam.MatchesPlayed++;
        winnerTeam.Wins++;
        winnerTeam.AddMatchResult("W");

        loserTeam.MatchesPlayed++;
        loserTeam.Losses++;
        loserTeam.AddMatchResult("L");
    }

    /// <summary>
    /// Применяет результат ничьей (если будут)
    /// </summary>
    public static void ApplyDrawResult(Team team1, Team team2)
    {
        int k1 = GetKFactor(team1.MatchesPlayed);
        int k2 = GetKFactor(team2.MatchesPlayed);

        double expected1 = GetExpectedScore(team1.VrsRating, team2.VrsRating);
        double expected2 = GetExpectedScore(team2.VrsRating, team1.VrsRating);

        int change1 = (int)Math.Round(k1 * (0.5 - expected1));
        int change2 = (int)Math.Round(k2 * (0.5 - expected2));

        team1.VrsRating += change1;
        team2.VrsRating += change2;

        team1.MatchesPlayed++;
        team1.Draws++;
        team1.AddMatchResult("D");

        team2.MatchesPlayed++;
        team2.Draws++;
        team2.AddMatchResult("D");
    }

    /// <summary>
    /// Получить K-фактор в зависимости от опыта команды
    /// </summary>
    private static int GetKFactor(int matchesPlayed)
    {
        return matchesPlayed < 30 ? NewTeamKFactor : BaseKFactor;
    }

    /// <summary>
    /// Рассчитать ожидаемый результат (вероятность победы)
    /// </summary>
    private static double GetExpectedScore(int ratingA, int ratingB)
    {
        return 1.0 / (1.0 + Math.Pow(10, (ratingB - ratingA) / 400.0));
    }

    /// <summary>
    /// Получить множитель в зависимости от разницы счета
    /// </summary>
    private static double GetScoreMultiplier(int winnerScore, int loserScore)
    {
        int difference = Math.Abs(winnerScore - loserScore);

        if (difference >= 10)
            return BlowoutMultiplier; // Разгром
        else if (difference <= 3)
            return CloseMatchMultiplier; // Близкий матч
        else
            return 1.0; // Обычный матч
    }

    /// <summary>
    /// Рассчитать вероятность победы в процентах
    /// </summary>
    public static double GetWinProbability(Team team1, Team team2)
    {
        return GetExpectedScore(team1.VrsRating, team2.VrsRating) * 100;
    }

    /// <summary>
    /// Получить текстовое описание разницы в силе команд
    /// </summary>
    public static string GetMatchupDescription(Team team1, Team team2)
    {
        int diff = Math.Abs(team1.VrsRating - team2.VrsRating);

        if (diff < 50)
            return "Равные соперники";
        else if (diff < 100)
            return "Небольшое преимущество";
        else if (diff < 200)
            return "Заметное преимущество";
        else if (diff < 300)
            return "Сильное преимущество";
        else
            return "Подавляющее преимущество";
    }

    /// <summary>
    /// Симуляция результата матча для превью (не изменяет реальные данные)
    /// </summary>
    public static RatingChangePreview PreviewRatingChange(
        Team winnerTeam,
        Team loserTeam,
        int winnerScore,
        int loserScore)
    {
        var (winnerChange, loserChange) = CalculateRatingChange(
            winnerTeam, loserTeam, winnerScore, loserScore);

        return new RatingChangePreview
        {
            WinnerOldRating = winnerTeam.VrsRating,
            WinnerNewRating = winnerTeam.VrsRating + winnerChange,
            WinnerChange = winnerChange,
            LoserOldRating = loserTeam.VrsRating,
            LoserNewRating = loserTeam.VrsRating + loserChange,
            LoserChange = loserChange
        };
    }
}

/// <summary>
/// Предварительный просмотр изменения рейтинга
/// </summary>
public class RatingChangePreview
{
    public int WinnerOldRating { get; set; }
    public int WinnerNewRating { get; set; }
    public int WinnerChange { get; set; }

    public int LoserOldRating { get; set; }
    public int LoserNewRating { get; set; }
    public int LoserChange { get; set; }
}