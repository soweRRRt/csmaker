using csmaker.Models;
using csmaker.Properties;
using System.Drawing;
using System.Reflection;

namespace csmaker.Utilities;

public static class Extension
{
    public static Image? GetCountryImage(this Player player)
    {
        if (string.IsNullOrWhiteSpace(player.Country))
            return null;

        var property = typeof(Resources).GetProperty(player.Country, BindingFlags.Static | BindingFlags.Public);

        if (property == null)
            return null;

        return property.GetValue(null) as Image;
    }
}
