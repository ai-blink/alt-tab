using Switchboard.App.ViewModels;
using Switchboard.Core.Models;

namespace Switchboard.App;

public sealed record UserSettings
{
    public SwitcherViewMode SelectedViewMode { get; init; } = SwitcherViewMode.Grid;

    public WindowSortMode SelectedSortMode { get; init; } = WindowSortMode.Recent;

    public OverlayThemeMode SelectedAppearanceMode { get; init; } = OverlayThemeMode.Transparent;

    public OverlayOpacityPreset SelectedOverlayOpacityPreset { get; init; } = OverlayOpacityPreset.Ninety;

    public OverlayScalePreset SelectedOverlayScalePreset { get; init; } = OverlayScalePreset.Hundred;

    public ThumbnailScalePreset SelectedThumbnailScalePreset { get; init; } = ThumbnailScalePreset.Normal;

    public SwitcherSizingPolicy SelectedSizingPolicy { get; init; } = SwitcherSizingPolicy.Auto;

    public SwitcherViewMode DefaultViewMode { get; init; } = SwitcherViewMode.Grid;

    public SwitcherHotkeyModifier SelectedFirstHotkeyModifier { get; init; } = SwitcherHotkeyModifier.Ctrl;

    public SwitcherHotkeyModifier SelectedSecondHotkeyModifier { get; init; } = SwitcherHotkeyModifier.Alt;

    public SwitcherHotkeyKey SelectedHotkeyKey { get; init; } = SwitcherHotkeyKey.Space;

    public bool IsCompactOverlayEnabled { get; init; }

    public OverlayPlacement CompactOverlayPlacement { get; init; } = OverlayPlacement.BottomLeft;

    public OverlayPositionPreference? SavedOverlayPosition { get; init; }

    public bool IsAlwaysOnTop { get; init; } = true;
}
