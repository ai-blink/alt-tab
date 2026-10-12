using Switchboard.Native;

namespace Switchboard.Tests;

public sealed class ForegroundWindowPresenterTests
{
    [Fact]
    public void Invalid_windows_are_rejected_without_activation()
    {
        Assert.False(ForegroundWindowPresenter.TryPresent(0, keepTopmost: false));
        Assert.False(ForegroundWindowPresenter.TryRestore(0));
        Assert.False(ForegroundWindowPresenter.TryPresent(new nint(-12345), keepTopmost: true));
        Assert.False(ForegroundWindowPresenter.TryRestore(new nint(-12345)));
    }
}
