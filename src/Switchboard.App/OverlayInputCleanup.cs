using System.Windows;
using System.Windows.Input;
using ContextMenu = System.Windows.Controls.ContextMenu;

namespace Switchboard.App;

public static class OverlayInputCleanup
{
    public static void BeforeHide(Window overlay, ContextMenu? sortMenu, Window? iconLayer)
    {
        var captured = Mouse.Captured as DependencyObject;
        var captureWindow = captured is null ? null : Window.GetWindow(captured);
        var ownsCapture = captured is not null &&
            (ReferenceEquals(captureWindow, overlay) ||
             iconLayer is not null && ReferenceEquals(captureWindow, iconLayer) ||
             sortMenu?.IsMouseCaptureWithin == true);

        // ContextMenu is a separate popup. Hiding its placement target does not
        // close it; end menu mode while its owner and focus tree still exist.
        if (sortMenu is not null)
        {
            sortMenu.IsOpen = false;
        }

        if (ownsCapture && ReferenceEquals(Mouse.Captured, captured))
        {
            Mouse.Capture(null);
        }

        // Do not leave the clickable icon window waiting for a Loaded refresh.
        iconLayer?.Hide();
    }
}
