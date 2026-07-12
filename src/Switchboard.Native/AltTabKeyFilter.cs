namespace Switchboard.Native;

public sealed class AltTabKeyFilter
{
    private bool isTabPressed;

    public AltTabKeyAction Process(bool isTabKeyDown, bool isTabKeyUp, bool isAltDown)
    {
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
