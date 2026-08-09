using Switchboard.App;
using Switchboard.Core.Models;

namespace Switchboard.Tests;

public sealed class UserSettingsJsonTests
{
    [Theory]
    [InlineData(OverlayScalePreset.Eighty)]
    [InlineData(OverlayScalePreset.Hundred)]
    [InlineData(OverlayScalePreset.OneTwentyFive)]
    [InlineData(OverlayScalePreset.OneFifty)]
    [InlineData(OverlayScalePreset.TwoHundred)]
    public void Overlay_scale_round_trips_through_json(OverlayScalePreset preset)
    {
        var settingsPath = Path.Combine(Path.GetTempPath(), $"switchboard-settings-{Guid.NewGuid():N}.json");

        try
        {
            var store = new JsonUserSettingsStore(settingsPath);
            store.Save(new UserSettings { SelectedOverlayScalePreset = preset });

            var loaded = store.Load();

            Assert.Equal(preset, loaded.SelectedOverlayScalePreset);
        }
        finally
        {
            File.Delete(settingsPath);
        }
    }
}
