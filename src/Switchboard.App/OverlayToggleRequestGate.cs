namespace Switchboard.App;

// A hook can keep receiving shortcuts while the dispatcher cannot service them.
// Keep one pending gesture, and let an explicit show/hide supersede that gesture.
public sealed class OverlayToggleRequestGate
{
    private long nextTicket;
    private long pendingTicket;

    public bool TryQueue(out long ticket)
    {
        ticket = Interlocked.Increment(ref nextTicket);
        return Interlocked.CompareExchange(ref pendingTicket, ticket, 0) == 0;
    }

    public bool TryConsume(long ticket) =>
        ticket != 0 && Interlocked.CompareExchange(ref pendingTicket, 0, ticket) == ticket;

    public void CancelPending() => Interlocked.Exchange(ref pendingTicket, 0);
}
