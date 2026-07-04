using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Switchboard.App;

public sealed class JsonUserSettingsStore : IUserSettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string settingsPath;

    public JsonUserSettingsStore()
        : this(CreateDefaultSettingsPath())
    {
    }

    public JsonUserSettingsStore(string settingsPath)
    {
        this.settingsPath = settingsPath;
    }

    public UserSettings Load()
    {
        try
        {
            if (!File.Exists(settingsPath))
            {
                return new UserSettings();
            }

            var json = File.ReadAllText(settingsPath);
            return JsonSerializer.Deserialize<UserSettings>(json, SerializerOptions) ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    public void Save(UserSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(settingsPath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, SerializerOptions);
            File.WriteAllText(settingsPath, json);
        }
        catch
        {
        }
    }

    private static string CreateDefaultSettingsPath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, "Switchboard", "settings.json");
    }
}
