using System.Text.Json;

namespace HandheldLauncher.App.Services;

public sealed class LauncherSettings
{
    public string SteamExe { get; set; }
    public string BattleNetExe { get; set; }
    public string XboxExe { get; set; }
}

public sealed class LauncherSettingsService
{
    private readonly string _dir;
    private readonly string _file;
    public LauncherSettings Settings { get; private set; } = new();

    public LauncherSettingsService(string appName = null)
    {
        var name = string.IsNullOrWhiteSpace(appName) ? "LegionLauncher" : appName!;
        _dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name);
        _file = Path.Combine(_dir, "settings.json");
        Directory.CreateDirectory(_dir);
    }

    public async Task LoadAsync()
    {
        if (!File.Exists(_file)) { Settings = new(); return; }
        var json = await File.ReadAllTextAsync(_file);
        Settings = JsonSerializer.Deserialize<LauncherSettings>(json) ?? new();
    }

    public async Task SaveAsync()
    {
        Directory.CreateDirectory(_dir);
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_file, json);
    }
}
