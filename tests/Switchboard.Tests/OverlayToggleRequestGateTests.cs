using Switchboard.App;

namespace Switchboard.Tests;

public sealed class OverlayToggleRequestGateTests
{
    [Fact]
    public void Repeated_shortcuts_while_dispatcher_is_stalled_toggle_only_once()
    {
        var gate = new OverlayToggleRequestGate();
        var queued = new List<long>();
        for (var index = 0; index < 10; index++)
        {
            if (gate.TryQueue(out var ticket))
            {
                queued.Add(ticket);
            }
        }

        Assert.Single(queued);
        var visible = false;
        foreach (var ticket in queued)
        {
            if (gate.TryConsume(ticket))
            {
                visible = !visible;
            }
        }
        Assert.True(visible);
    }

    [Fact]
    public void Explicit_show_prevents_an_older_shortcut_from_hiding_the_window()
    {
        var gate = new OverlayToggleRequestGate();
        Assert.True(gate.TryQueue(out var oldTicket));
        gate.CancelPending(); // Tray/secondary shortcut shows the window.
        Assert.False(gate.TryConsume(oldTicket));
        Assert.True(gate.TryQueue(out var nextTicket));
        Assert.True(gate.TryConsume(nextTicket));
    }

    [Fact]
    public void Old_callback_cannot_consume_a_new_gesture_after_cancellation()
    {
        var gate = new OverlayToggleRequestGate();
        Assert.True(gate.TryQueue(out var oldTicket));
        gate.CancelPending();
        Assert.True(gate.TryQueue(out var nextTicket));
        Assert.False(gate.TryConsume(oldTicket));
        Assert.True(gate.TryConsume(nextTicket));
        Assert.False(gate.TryConsume(nextTicket));
    }

    [Fact]
    public void Independent_gestures_still_open_and_close_the_overlay()
    {
        var gate = new OverlayToggleRequestGate();
        var visible = false;
        for (var index = 0; index < 2; index++)
        {
            Assert.True(gate.TryQueue(out var ticket));
            Assert.True(gate.TryConsume(ticket));
            visible = !visible;
            Assert.Equal(index == 0, visible);
        }
    }

    [Fact]
    public void Concurrent_hook_requests_have_only_one_pending_gesture()
    {
        var gate = new OverlayToggleRequestGate();
        var accepted = 0;
        Parallel.For(0, 100, _ =>
        {
            if (gate.TryQueue(out _))
            {
                Interlocked.Increment(ref accepted);
            }
        });
        Assert.Equal(1, accepted);
    }
}
