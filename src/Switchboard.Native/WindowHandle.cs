namespace Switchboard.Native;

public readonly record struct WindowHandle(nint Value)
{
    public bool IsEmpty => Value == 0;
}
