using csmaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csmaker.Services;

/// <summary>
/// Симулятор матчей CS2
/// </summary>
public class MatchSimulator
{
    private static readonly Random Random = new Random();

    // Вероятность победы CT/T в зависимости от карты
    private static readonly Dictionary<string, double> MapCTAdvantage = new()
    {
        { "de_ancient", 0.52 },   // CT склонная карта
        { "de_anubis", 0.50 },    // Сбалансированная
        { "de_dust2", 0.48 },     // T склонная
        { "de_inferno", 0.51 },   // Слегка CT
        { "de_mirage", 0.49 },    // Почти сбалансированная
        { "de_nuke", 0.55 },      // Сильно CT
        { "de_vertigo", 0.53 }    // CT склонная
    };

    /// <summary>
    /// Симулировать весь матч
    /// </summary>
    public static Match SimulateMatch(Team team1, Team team2, BestOfFormat format, List<Map>? maps = null)
    {
        var match = new Match
        {
            Team1 = team1,
            Team2 = team2,
            Format = format,
            Status = MatchStatus.InProgress
        };

        // Если карты не указаны, выбираем случайные
        if (maps == null || maps.Count == 0)
        {
            match.SelectedMaps = PickRandomMaps(format);
        }
        else
        {
            match.SelectedMaps = maps;
        }

        // Симулируем каждую карту до победы
        int mapsToWin = GetMapsToWin(format);
        int team1Wins = 0;
        int team2Wins = 0;

        foreach (var map in match.SelectedMaps)
        {
            // Проверяем, не закончился ли матч
            if (team1Wins >= mapsToWin || team2Wins >= mapsToWin)
                break;

            var mapResult = SimulateMap(map, team1, team2);
            match.MapResults.Add(mapResult);

            if (mapResult.Winner == team1)
                team1Wins++;
            else
                team2Wins++;
        }

        // Определяем победителя
        if (team1Wins > team2Wins)
        {
            match.Winner = team1;
            match.Loser = team2;
        }
        else
        {
            match.Winner = team2;
            match.Loser = team1;
        }

        match.Status = MatchStatus.Finished;
        return match;
    }

    /// <summary>
    /// Симулировать одну карту
    /// </summary>
    public static MapResult SimulateMap(Map map, Team team1, Team team2)
    {
        var result = new MapResult
        {
            Map = map,
            Winner = team1,
            Loser = team2
        };

        // Определяем силу команд с учетом рейтинга
        double team1Strength = CalculateTeamStrength(team1);
        double team2Strength = CalculateTeamStrength(team2);

        // Нормализуем вероятности
        double totalStrength = team1Strength + team2Strength;
        double team1WinProb = team1Strength / totalStrength;

        // Получаем преимущество CT для карты
        double ctAdvantage = 0.50; // По умолчанию
        if (MapCTAdvantage.ContainsKey(map.Name))
            ctAdvantage = MapCTAdvantage[map.Name];

        // Симулируем первый тайм (до 12 раундов или до 13 побед)
        // Team1 начинает на CT
        var (team1First, team2First) = SimulateHalf(team1WinProb, ctAdvantage, true, 0, 0);
        result.Team1FirstHalfScore = team1First;
        result.Team2FirstHalfScore = team2First;

        // Проверяем, не выиграл ли кто-то уже 13 раундов
        if (team1First >= 13)
        {
            result.Team1Score = team1First;
            result.Team2Score = team2First;
            result.Winner = team1;
            result.Loser = team2;
            return result;
        }
        else if (team2First >= 13)
        {
            result.Team1Score = team1First;
            result.Team2Score = team2First;
            result.Winner = team2;
            result.Loser = team1;
            return result;
        }

        // Симулируем второй тайм (раунды 13-24 или до 13 побед)
        // Team1 переходит на T
        var (team1Second, team2Second) = SimulateHalf(team1WinProb, ctAdvantage, false, team1First, team2First);
        result.Team1SecondHalfScore = team1Second;
        result.Team2SecondHalfScore = team2Second;

        // Общий счет
        result.Team1Score = result.Team1FirstHalfScore + result.Team1SecondHalfScore;
        result.Team2Score = result.Team2FirstHalfScore + result.Team2SecondHalfScore;

        // Овертайм (если счет 12:12, 15:15 и т.д.)
        int overtimeCount = 0;
        while (result.Team1Score == result.Team2Score && result.Team1Score >= 12)
        {
            overtimeCount++;
            result.WentToOvertime = true;

            // Овертайм: MR12 (до 4 раундов в каждой стороне или до победы)
            int targetScore = result.Team1Score + 4; // 16, 19, 22 и т.д.

            // Первая половина OT (Team1 на CT)
            var (ot1First, ot2First) = SimulateOvertimeHalf(team1WinProb, ctAdvantage, true, result.Team1Score, result.Team2Score, targetScore);
            result.Team1Score += ot1First;
            result.Team2Score += ot2First;

            // Проверяем победу
            if (result.Team1Score >= targetScore || result.Team2Score >= targetScore)
                break;

            // Вторая половина OT (Team1 на T)
            var (ot1Second, ot2Second) = SimulateOvertimeHalf(team1WinProb, ctAdvantage, false, result.Team1Score, result.Team2Score, targetScore);
            result.Team1Score += ot1Second;
            result.Team2Score += ot2Second;
        }
        result.OvertimeCount = overtimeCount;

        // Определяем победителя
        if (result.Team1Score > result.Team2Score)
        {
            result.Winner = team1;
            result.Loser = team2;
        }
        else
        {
            result.Winner = team2;
            result.Loser = team1;
        }

        return result;
    }

    /// <summary>
    /// Симулировать половину (тайм) - до 12 раундов или до 13 побед
    /// </summary>
    private static (int team1Rounds, int team2Rounds) SimulateHalf(
        double team1WinProb,
        double ctAdvantage,
        bool team1IsCT,
        int currentTeam1Score,
        int currentTeam2Score)
    {
        int team1Rounds = 0;
        int team2Rounds = 0;

        // Максимум 12 раундов в тайме
        for (int round = 0; round < 12; round++)
        {
            // КРИТИЧЕСКАЯ ПРОВЕРКА: если кто-то уже набрал 13 раундов - останавливаемся
            int totalTeam1 = currentTeam1Score + team1Rounds;
            int totalTeam2 = currentTeam2Score + team2Rounds;

            if (totalTeam1 >= 13 || totalTeam2 >= 13)
                break;

            // Определяем вероятность победы Team1 в раунде
            double team1RoundWinChance = CalculateRoundWinChance(team1WinProb, ctAdvantage, team1IsCT);

            if (Random.NextDouble() < team1RoundWinChance)
                team1Rounds++;
            else
                team2Rounds++;

            // ПРОВЕРКА СРАЗУ ПОСЛЕ РАУНДА: если кто-то достиг 13 - останавливаемся
            totalTeam1 = currentTeam1Score + team1Rounds;
            totalTeam2 = currentTeam2Score + team2Rounds;

            if (totalTeam1 >= 13 || totalTeam2 >= 13)
                break;
        }

        return (team1Rounds, team2Rounds);
    }

    /// <summary>
    /// Симулировать овертайм половину - до 4 раундов или до победы
    /// </summary>
    private static (int team1Rounds, int team2Rounds) SimulateOvertimeHalf(
        double team1WinProb,
        double ctAdvantage,
        bool team1IsCT,
        int currentTeam1Score,
        int currentTeam2Score,
        int targetScore)
    {
        int team1Rounds = 0;
        int team2Rounds = 0;

        for (int round = 0; round < 4; round++)
        {
            // Проверяем, не достиг ли кто-то целевого счета
            if (currentTeam1Score + team1Rounds >= targetScore ||
                currentTeam2Score + team2Rounds >= targetScore)
                break;

            // Определяем вероятность победы Team1 в раунде
            double team1RoundWinChance = CalculateRoundWinChance(team1WinProb, ctAdvantage, team1IsCT);

            if (Random.NextDouble() < team1RoundWinChance)
                team1Rounds++;
            else
                team2Rounds++;

            // ПРОВЕРКА СРАЗУ ПОСЛЕ РАУНДА: если кто-то достиг целевого счета - останавливаемся
            if (currentTeam1Score + team1Rounds >= targetScore ||
                currentTeam2Score + team2Rounds >= targetScore)
                break;
        }

        return (team1Rounds, team2Rounds);
    }

    /// <summary>
    /// Рассчитать вероятность победы Team1 в раунде с учетом всех факторов
    /// </summary>
    private static double CalculateRoundWinChance(double team1WinProb, double ctAdvantage, bool team1IsCT)
    {
        double team1RoundWinChance;

        if (team1IsCT)
        {
            // Team1 играет на CT стороне
            // Базовая вероятность увеличивается если карта CT склонная
            double ctBonus = (ctAdvantage - 0.5) * 0.4; // ±0.02 (для карты 0.55 = +0.02, для 0.48 = -0.008)
            team1RoundWinChance = team1WinProb + ctBonus;
        }
        else
        {
            // Team1 играет на T стороне
            // Базовая вероятность уменьшается если карта CT склонная
            double ctPenalty = (ctAdvantage - 0.5) * 0.4;
            team1RoundWinChance = team1WinProb - ctPenalty;
        }

        // Добавляем случайность в каждом раунде (ПОСЛЕ базового расчета)
        double randomFactor = (Random.NextDouble() - 0.5) * 0.15; // ±7.5%
        team1RoundWinChance += randomFactor;

        // Ограничиваем в диапазоне 15%-85% (более реалистично)
        return Clamp(team1RoundWinChance, 0.15, 0.85);
    }

    /// <summary>
    /// Рассчитать силу команды на основе рейтинга и формы
    /// </summary>
    private static double CalculateTeamStrength(Team team)
    {
        // Базовая сила - VRS рейтинг
        double baseStrength = team.VrsRating;

        // Бонус за форму (последние 5 матчей)
        if (team.RecentForm.Count > 0)
        {
            int wins = team.RecentForm.Count(f => f == "W");
            double formBonus = (wins / (double)team.RecentForm.Count - 0.5) * 100; // ±50 очков
            baseStrength += formBonus;
        }

        // Учитываем опыт команды
        if (team.MatchesPlayed < 10)
        {
            // Новые команды менее стабильны
            baseStrength *= 0.9;
        }

        return Math.Max(baseStrength, 100); // Минимум 100
    }

    /// <summary>
    /// Ограничить значение в диапазоне (аналог Math.Clamp для .NET 4.8)
    /// </summary>
    private static double Clamp(double value, double min, double max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Выбрать случайные карты для матча
    /// </summary>
    private static List<Map> PickRandomMaps(BestOfFormat format)
    {
        var allMaps = new List<Map>(Map.ActiveDutyMaps);
        var selectedMaps = new List<Map>();

        int mapCount = (int)format;

        for (int i = 0; i < mapCount; i++)
        {
            int index = Random.Next(allMaps.Count);
            selectedMaps.Add(allMaps[index]);
            allMaps.RemoveAt(index);
        }

        return selectedMaps;
    }

    /// <summary>
    /// Получить количество карт для победы
    /// </summary>
    private static int GetMapsToWin(BestOfFormat format)
    {
        return ((int)format / 2) + 1; // BO1=1, BO3=2, BO5=3
    }

    /// <summary>
    /// Симулировать быстрый матч (только счет, без деталей)
    /// </summary>
    public static Match SimulateQuickMatch(Team team1, Team team2, BestOfFormat format)
    {
        var match = SimulateMatch(team1, team2, format);

        // Применяем результат к рейтингам команд
        if (match.Winner != null && match.Loser != null)
        {
            // Считаем общий счет по всем картам
            int totalWinnerScore = 0;
            int totalLoserScore = 0;

            foreach (var mapResult in match.MapResults)
            {
                if (mapResult.Winner == match.Winner)
                {
                    totalWinnerScore += mapResult.Team1Score;
                    totalLoserScore += mapResult.Team2Score;
                }
                else
                {
                    totalWinnerScore += mapResult.Team2Score;
                    totalLoserScore += mapResult.Team1Score;
                }
            }

            // Применяем изменение рейтинга
            VrsRatingService.ApplyMatchResult(match.Winner, match.Loser, totalWinnerScore, totalLoserScore);
        }

        return match;
    }
}