using csmaker.Properties;
using System;
using System.Linq;
using System.Reflection;

namespace csmaker.Services;

public static class CountryProvider
{
    private static string[]? _countries;

    public static string[] GetAllCountries()
    {
        if (_countries != null)
            return _countries;

        var properties = typeof(Resources)
            .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(p => p.PropertyType == typeof(System.Drawing.Bitmap) ||
                       p.PropertyType == typeof(System.Drawing.Image))
            .Select(p => p.Name)
            .Where(name => name.Length == 2 &&
                          name.All(char.IsUpper))
            .OrderBy(name => name)
            .ToArray();

        _countries = properties;
        return _countries;
    }

    public static bool IsValidCountryCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 2)
            return false;

        return GetAllCountries().Contains(code, StringComparer.OrdinalIgnoreCase);
    }
}
