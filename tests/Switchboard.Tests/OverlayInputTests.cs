using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Switchboard.App;

namespace Switchboard.Tests;

public sealed class OverlayInputTests
{
    [Fact]
    public void Hiding_overlay_closes_its_sort_menu_before_returning()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            ContextMenu? menu = null;
            try
            {
                var overlay = new Window();
                menu = new ContextMenu();

                // The owner is never shown and its dispatcher is not pumped, so
                // the popup cannot take desktop focus or capture during this test.
                menu.PlacementTarget = overlay;
                menu.IsOpen = true;
                Assert.True(menu.IsOpen);

                OverlayInputCleanup.BeforeHide(overlay, menu, iconLayer: null);
                overlay.Hide();

                Assert.False(menu.IsOpen);
                Assert.False(overlay.IsVisible);
                OverlayInputCleanup.BeforeHide(overlay, menu, iconLayer: null);
                Assert.False(menu.IsOpen);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                if (menu is not null)
                {
                    menu.IsOpen = false;
                }
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "Overlay input test did not finish.");
        if (failure is not null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }

}
