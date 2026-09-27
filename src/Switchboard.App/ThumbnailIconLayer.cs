using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Switchboard.Core.Models;

namespace Switchboard.App;

// Draws an app icon on the lower-left corner of every live thumbnail; clicking an icon switches to
// that window. DWM composites thumbnails above the overlay's own WPF content, so the icons live in this
// separate non-activating layered window (fully transparent pixels let clicks through to the overlay); being owned by the overlay keeps it stacked right above it. Icons sit on the card's preview
// slot (not the letterboxed thumbnail) so every card places them identically, and they are shown only
// while DwmThumbnailPreview.DisplayedBounds reports the thumbnail as visible.
public sealed class ThumbnailIconLayer : Window
{
    private const double IconOpacity = 0.85;
    private const double PlatePadding = 5;
    private const double PlateBorder = 1.5;
    private const double IconSizeRatio = 0.34;
    private const double MaxIconSize = 56;
    private const double MinIconSize = 20;
    private const double Inset = 8;
    private const int GwlExStyle = -20;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;

    private readonly Window overlay;
    private readonly System.Windows.Controls.ListBox list;
    private readonly Func<bool> isEnabled;
    private readonly Action<nint> onIconClick;
    private readonly Canvas canvas = new();
    private List<IconPlacement> placements = [];
    private bool isRefreshQueued;

    public ThumbnailIconLayer(Window overlay, System.Windows.Controls.ListBox list, Func<bool> isEnabled, Action<nint> onIconClick)
    {
        this.overlay = overlay;
        this.list = list;
        this.isEnabled = isEnabled;
        this.onIconClick = onIconClick;

        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = System.Windows.Media.Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        ShowActivated = false;
        Focusable = false;
        Content = canvas;
        Owner = overlay;

        SourceInitialized += (_, _) => MakeNonActivating();

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
                FindDescendant<DwmThumbnailPreview>(container) is not { DisplayedBounds: not null } preview)
            {
                continue;
            }

            var bounds = preview.TransformToAncestor(overlay).TransformBounds(new Rect(preview.RenderSize));

            var size = Math.Clamp(bounds.Height * IconSizeRatio, MinIconSize, MaxIconSize);
            var plateSize = size + ((PlatePadding + PlateBorder) * 2);

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

            // Light plate with a dark rim: the fill separates the icon from dark thumbnails,
            // the rim separates the plate from bright ones.
            var plate = new Border
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xE6, 0xF4, 0xF6, 0xFA)),
                BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x99, 0x0B, 0x0F, 0x16)),
                BorderThickness = new Thickness(PlateBorder),
                CornerRadius = new CornerRadius(placement.Size * 0.28),
                Padding = new Thickness(PlatePadding),
                Opacity = IconOpacity,
                Cursor = System.Windows.Input.Cursors.Hand,
                Child = image
            };
            var handle = placement.Handle;
            plate.MouseLeftButtonUp += (_, e) =>
            {
                e.Handled = true;
                onIconClick(handle);
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

    private void MakeNonActivating()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        var style = GetWindowLong(hwnd, GwlExStyle);
        _ = SetWindowLong(hwnd, GwlExStyle, style | WsExToolWindow | WsExNoActivate);
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
