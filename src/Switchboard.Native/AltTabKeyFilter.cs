namespace Switchboard.Native;

public sealed class AltTabKeyFilter
{
    // Windows' longest keyboard repeat delay is 1000 ms and repeats then arrive at most ~400 ms apart,
    // so a Tab key-down this long after the previous Tab event is a new press whose key-up was lost
    // (for example to an elevated window that UIPI hides from the low-level hook).
    private const int LostKeyUpThresholdMs = 1100;

    private bool isTabPressed;
    private int lastTabEventTimeMs;

    public AltTabKeyAction Process(bool isTabKeyDown, bool isTabKeyUp, bool isAltDown, int eventTimeMs)
    {
        if (isTabKeyDown && isTabPressed && unchecked(eventTimeMs - lastTabEventTimeMs) > LostKeyUpThresholdMs)
        {
            isTabPressed = false;
        }

        if (isTabKeyDown || isTabKeyUp)
        {
            lastTabEventTimeMs = eventTimeMs;
        }

        if (isTabKeyUp && isTabPressed)
        {
            isTabPressed = false;
            return AltTabKeyAction.Suppress;
        }

        if (!isTabKeyDown || !isAltDown)
        {
            return AltTabKeyAction.PassThrough;
        }

        if (isTabPressed)
        {
            return AltTabKeyAction.Suppress;
        }

        isTabPressed = true;
        return AltTabKeyAction.ToggleAndSuppress;
    }
}

public enum AltTabKeyAction
{
    PassThrough,
    Suppress,
    ToggleAndSuppress
}
