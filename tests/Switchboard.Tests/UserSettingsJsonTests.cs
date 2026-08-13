using Switchboard.App;
using Switchboard.Core.Models;

namespace Switchboard.Tests;

public sealed class UserSettingsJsonTests
{
    [Theory]
    [InlineData(OverlayScalePreset.Sixty)]
    [InlineData(OverlayScalePreset.Seventy)]
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

    [Fact]
    public void Saved_overlay_position_round_trips_through_json()
    {
        var settingsPath = Path.Combine(Path.GetTempPath(), $"switchboard-settings-{Guid.NewGuid():N}.json");
        var savedPosition = new OverlayPositionPreference
        {
            MonitorDeviceName = "DISPLAY2",
            Anchor = OverlayAnchor.MiddleRight,
            OffsetX = -24,
            OffsetY = 16
        };

        try
        {
            var store = new JsonUserSettingsStore(settingsPath);
            store.Save(new UserSettings { SavedOverlayPosition = savedPosition });

            var loaded = store.Load();

            Assert.Equal(savedPosition, loaded.SavedOverlayPosition);
        }
        finally
        {
            File.Delete(settingsPath);
        }
    }
}
