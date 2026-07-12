using Switchboard.Native;

namespace Switchboard.Tests;

public sealed class AltTabKeyFilterTests
{
    [Fact]
    public void Process_toggles_only_on_first_alt_tab_keydown()
    {
        var filter = new AltTabKeyFilter();

        var first = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true);
        var repeat = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true);

        Assert.Equal(AltTabKeyAction.ToggleAndSuppress, first);
        Assert.Equal(AltTabKeyAction.Suppress, repeat);
    }

    [Fact]
    public void Process_allows_next_toggle_after_consumed_keyup()
    {
        var filter = new AltTabKeyFilter();
        _ = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true);

        var keyUp = filter.Process(isTabKeyDown: false, isTabKeyUp: true, isAltDown: false);
        var nextPress = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true);

        Assert.Equal(AltTabKeyAction.Suppress, keyUp);
        Assert.Equal(AltTabKeyAction.ToggleAndSuppress, nextPress);
    }

    [Fact]
    public void Process_passes_plain_tab_through()
    {
        var filter = new AltTabKeyFilter();

        var action = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: false);

        Assert.Equal(AltTabKeyAction.PassThrough, action);
    }
}
