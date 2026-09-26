using Switchboard.Native;

namespace Switchboard.Tests;

public sealed class AltTabKeyFilterTests
{
    [Fact]
    public void Process_toggles_only_on_first_alt_tab_keydown()
    {
        var filter = new AltTabKeyFilter();

        var first = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: 1000);
        var repeat = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: 1500);

        Assert.Equal(AltTabKeyAction.ToggleAndSuppress, first);
        Assert.Equal(AltTabKeyAction.Suppress, repeat);
    }

    [Fact]
    public void Process_keeps_suppressing_a_held_tab_through_repeats()
    {
        var filter = new AltTabKeyFilter();
        _ = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: 0);

        // Longest repeat delay (1000 ms), then repeats every 400 ms: all still one held press.
        var actions = new[] { 1000, 1400, 1800, 2200 }
            .Select(time => filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: time));

        Assert.All(actions, action => Assert.Equal(AltTabKeyAction.Suppress, action));
    }

    [Fact]
    public void Process_allows_next_toggle_after_consumed_keyup()
    {
        var filter = new AltTabKeyFilter();
        _ = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: 1000);

        var keyUp = filter.Process(isTabKeyDown: false, isTabKeyUp: true, isAltDown: false, eventTimeMs: 1100);
        var nextPress = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: 1300);

        Assert.Equal(AltTabKeyAction.Suppress, keyUp);
        Assert.Equal(AltTabKeyAction.ToggleAndSuppress, nextPress);
    }

    [Fact]
    public void Process_toggles_again_when_the_previous_keyup_was_lost()
    {
        var filter = new AltTabKeyFilter();
        _ = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: 1000);

        // The Tab key-up went to an elevated window and never reached the hook.
        var nextPress = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: 5000);

        Assert.Equal(AltTabKeyAction.ToggleAndSuppress, nextPress);
    }

    [Fact]
    public void Process_handles_tick_count_wraparound()
    {
        var filter = new AltTabKeyFilter();
        _ = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: int.MaxValue - 100);

        var repeat = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: true, eventTimeMs: unchecked(int.MaxValue + 300));

        Assert.Equal(AltTabKeyAction.Suppress, repeat);
    }

    [Fact]
    public void Process_passes_plain_tab_through()
    {
        var filter = new AltTabKeyFilter();

        var action = filter.Process(isTabKeyDown: true, isTabKeyUp: false, isAltDown: false, eventTimeMs: 1000);

        Assert.Equal(AltTabKeyAction.PassThrough, action);
    }
}
