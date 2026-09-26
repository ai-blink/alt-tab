using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Switchboard.Core.Models;

namespace Switchboard.App;

// Draws a translucent app icon on the lower-left corner of every live thumbnail. DWM composites
// thumbnails above the overlay's own WPF content, so the icons live in this separate click-through
// window; being owned by the overlay keeps it stacked right above it. Positions come from
// DwmThumbnailPreview.DisplayedBounds so icons appear and disappear exactly with their thumbnails.
public sealed class ThumbnailIconLayer : Window
{
    private const double IconOpacity = 0.85;
    private const double PlatePadding = 5;
    private const double IconSizeRatio = 0.34;
    private const double MaxIconSize = 56;
    private const double MinIconSize = 20;
    private const double Inset = 8;
    private const int GwlExStyle = -20;
    private const int WsExTransparent = 0x00000020;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;

    private readonly Window overlay;
    private readonly System.Windows.Controls.ListBox list;
    private readonly Func<bool> isEnabled;
    private readonly Canvas canvas = new() { IsHitTestVisible = false };
    private List<IconPlacement> placements = [];
    private bool isRefreshQueued;

    public ThumbnailIconLayer(Window overlay, System.Windows.Controls.ListBox list, Func<bool> isEnabled)
    {
        this.overlay = overlay;
        this.list = list;
        this.isEnabled = isEnabled;

        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = System.Windows.Media.Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        ShowActivated = false;
        Focusable = false;
        IsHitTestVisible = false;
        Content = canvas;
        Owner = overlay;

        SourceInitialized += (_, _) => MakeClickThrough();

        // LayoutUpdated fires for every layout pass on the dispatcher (including this window's),
        // so refreshes are coalesced and only rebuild when the placements actually change.
        overlay.LayoutUpdated += (_, _) => QueueRefresh();
        overlay.LocationChanged += (_, _) => QueueRefresh();
        overlay.IsVisibleChanged += (_, _) => QueueRefresh();
    }

    public void QueueRefresh()
    {
        if (isRefreshQueued)
        {
            return;
        }

        isRefreshQueued = true;
        _ = Dispatcher.BeginInvoke(Refresh, System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void Refresh()
    {
        isRefreshQueued = false;

        if (!overlay.IsVisible || !isEnabled())
        {
            if (IsVisible)
            {
                Hide();
            }

            return;
        }

        SetIfChanged(LeftProperty, overlay.Left);
        SetIfChanged(TopProperty, overlay.Top);
        SetIfChanged(WidthProperty, overlay.ActualWidth);
        SetIfChanged(HeightProperty, overlay.ActualHeight);

        var next = CollectPlacements();

        if (!next.SequenceEqual(placements))
        {
            placements = next;
            Rebuild();
        }

        if (!IsVisible)
        {
            Show();
        }
    }

    private List<IconPlacement> CollectPlacements()
    {
        var result = new List<IconPlacement>();

        for (var index = 0; index < list.Items.Count; index++)
        {
            if (list.Items[index] is not WindowSnapshot window ||
                list.ItemContainerGenerator.ContainerFromIndex(index) is not DependencyObject container ||
                FindDescendant<DwmThumbnailPreview>(container)?.DisplayedBounds is not Rect bounds)
            {
                continue;
            }

            var size = Math.Clamp(bounds.Height * IconSizeRatio, MinIconSize, MaxIconSize);
            var plateSize = size + (PlatePadding * 2);

            if (bounds.Width < plateSize + (Inset * 2) || bounds.Height < plateSize + (Inset * 2))
            {
                continue;
            }

            result.Add(new IconPlacement(window.Handle, bounds.Left + Inset, bounds.Bottom - plateSize - Inset, size));
        }

        return result;
    }

    private void Rebuild()
    {
        canvas.Children.Clear();

        foreach (var placement in placements)
        {
            if (WindowIconConverter.GetIcon(placement.Handle) is not { } icon)
            {
                continue;
            }

            var image = new System.Windows.Controls.Image
            {
                Source = icon,
                Width = placement.Size,
                Height = placement.Size
            };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            // A dark translucent plate keeps the icon legible over bright thumbnails.
            var plate = new Border
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x99, 0x0B, 0x0F, 0x16)),
                CornerRadius = new CornerRadius(placement.Size * 0.28),
                Padding = new Thickness(PlatePadding),
                Opacity = IconOpacity,
                Child = image
            };
            Canvas.SetLeft(plate, placement.Left);
            Canvas.SetTop(plate, placement.Top);
            canvas.Children.Add(plate);
        }
    }

    private void SetIfChanged(DependencyProperty property, double value)
    {
        if (!double.IsNaN(value) && Math.Abs((double)GetValue(property) - value) > 0.1)
        {
            SetValue(property, value);
        }
    }

    private void MakeClickThrough()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        var style = GetWindowLong(hwnd, GwlExStyle);
        _ = SetWindowLong(hwnd, GwlExStyle, style | WsExTransparent | WsExToolWindow | WsExNoActivate);
    }

    private static T? FindDescendant<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var child = VisualTreeHelper.GetChild(parent, index);

            if (child is T match)
            {
                return match;
            }

            if (FindDescendant<T>(child) is { } nested)
            {
                return nested;
            }
        }

        return null;
    }

    private readonly record struct IconPlacement(nint Handle, double Left, double Top, double Size);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static extern int GetWindowLong(nint hwnd, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
    private static extern int SetWindowLong(nint hwnd, int index, int newValue);
}
