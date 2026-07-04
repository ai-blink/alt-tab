namespace Switchboard.App;

public interface IUserSettingsStore
{
    UserSettings Load();

    void Save(UserSettings settings);
}
