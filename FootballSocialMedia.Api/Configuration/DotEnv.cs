using System.Text.RegularExpressions;

namespace FootballSocialMedia.Api.Configuration;

/// <summary>
/// Repo kökündeki .env dosyasını okuyup ortam değişkeni olarak yükler (ek NuGet paketi gerektirmez).
/// - Anahtarlar ASP.NET Core formatında: iç içe ayarlar için "__" (ör. CloudinarySettings__ApiSecret).
/// - Zaten tanımlı ortam değişkenleri ezilmez (sunucuda gerçek env değişkenleri önceliklidir).
/// - Değer içinde ${DIGER_ANAHTAR} kullanılabilir (docker compose ile aynı söz dizimi).
/// - .env; çalışma dizininden ve uygulamanın bin klasöründen yukarı doğru aranır.
/// </summary>
public static partial class DotEnv
{
    private const int MaxParentLevels = 6;

    public static string? Load(string fileName = ".env")
    {
        var path = Find(Directory.GetCurrentDirectory(), fileName) ?? Find(AppContext.BaseDirectory, fileName);
        if (path is null) return null;

        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            if (line.StartsWith("export ")) line = line["export ".Length..].TrimStart();

            var separator = line.IndexOf('=');
            if (separator <= 0) continue;

            var key = line[..separator].Trim();
            var value = Unquote(line[(separator + 1)..].Trim());
            value = VariablePattern().Replace(value, m => Environment.GetEnvironmentVariable(m.Groups[1].Value) ?? string.Empty);

            if (Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, value);
        }

        return path;
    }

    private static string? Find(string startDirectory, string fileName)
    {
        var dir = new DirectoryInfo(startDirectory);
        for (var level = 0; dir is not null && level <= MaxParentLevels; level++, dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, fileName);
            if (File.Exists(candidate)) return candidate;
        }
        return null;
    }

    private static string Unquote(string value) =>
        value.Length >= 2 && (value[0] == '"' && value[^1] == '"' || value[0] == '\'' && value[^1] == '\'')
            ? value[1..^1]
            : value;

    [GeneratedRegex(@"\$\{([A-Za-z_][A-Za-z0-9_]*)\}")]
    private static partial Regex VariablePattern();
}
