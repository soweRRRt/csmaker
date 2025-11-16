using csmaker.Models;
using System.Linq;

namespace csmaker.Services;

/// <summary>
/// Сервис для валидации данных игроков и команд
/// (ОПЦИОНАЛЬНО - можно не использовать, если валидация не нужна)
/// </summary>
public static class ValidationService
{
    /// <summary>
    /// Проверяет, уникален ли никнейм игрока
    /// </summary>
    public static bool IsNicknameUnique(string nickname, Player? excludePlayer = null)
    {
        return !DataService.Players.Any(p =>
            p != excludePlayer &&
            p.Nickname.Equals(nickname, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Проверяет, уникально ли название команды
    /// </summary>
    public static bool IsTeamNameUnique(string teamName, Team? excludeTeam = null)
    {
        return !DataService.Teams.Any(t =>
            t != excludeTeam &&
            t.Name.Equals(teamName, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Проверяет, можно ли добавить игрока в команду
    /// </summary>
    public static ValidationResult CanAddPlayerToTeam(Team team, Player player)
    {
        if (team.Players.Count >= 5)
            return ValidationResult.Error("В команде уже 5 игроков!");

        if (team.Players.Contains(player))
            return ValidationResult.Error("Игрок уже в этой команде!");

        if (player.Team != null && player.Team != team)
            return ValidationResult.Warning($"Игрок будет перемещен из команды '{player.Team.Name}'");

        return ValidationResult.Success();
    }

    /// <summary>
    /// Проверяет корректность данных игрока
    /// </summary>
    public static ValidationResult ValidatePlayer(string nickname, string country)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            return ValidationResult.Error("Никнейм не может быть пустым!");

        if (nickname.Length < 2)
            return ValidationResult.Error("Никнейм должен содержать минимум 2 символа!");

        if (nickname.Length > 20)
            return ValidationResult.Error("Никнейм не должен превышать 20 символов!");

        if (string.IsNullOrWhiteSpace(country))
            return ValidationResult.Error("Не выбрана страна!");

        if (!CountryProvider.IsValidCountryCode(country))
            return ValidationResult.Warning($"Страна '{country}' не имеет флага в ресурсах");

        return ValidationResult.Success();
    }

    /// <summary>
    /// Проверяет корректность названия команды
    /// </summary>
    public static ValidationResult ValidateTeamName(string teamName)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            return ValidationResult.Error("Название команды не может быть пустым!");

        if (teamName.Length < 2)
            return ValidationResult.Error("Название должно содержать минимум 2 символа!");

        if (teamName.Length > 50)
            return ValidationResult.Error("Название не должно превышать 50 символов!");

        return ValidationResult.Success();
    }
}

/// <summary>
/// Результат валидации
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = "";
    public ValidationLevel Level { get; set; }

    public static ValidationResult Success() => new ValidationResult
    {
        IsValid = true,
        Level = ValidationLevel.Success
    };

    public static ValidationResult Error(string message) => new ValidationResult
    {
        IsValid = false,
        Message = message,
        Level = ValidationLevel.Error
    };

    public static ValidationResult Warning(string message) => new ValidationResult
    {
        IsValid = true,
        Message = message,
        Level = ValidationLevel.Warning
    };
}

public enum ValidationLevel
{
    Success,
    Warning,
    Error
}