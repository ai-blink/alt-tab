using System.Collections.Concurrent;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Switchboard.Native;

namespace Switchboard.App;

// Window handle -> frozen app icon. Results are cached per handle because cards re-bind on every
// catalog refresh; the cache is simply cleared when it grows past the number of windows anyone keeps.
public sealed class WindowIconConverter : IValueConverter
{
    private const int MaxCachedIcons = 256;
    private static readonly ConcurrentDictionary<nint, ImageSource?> Cache = new();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is nint hwnd ? GetIcon(hwnd) : null;

    public static ImageSource? GetIcon(nint hwnd)
    {
        if (hwnd == 0)
        {
            return null;
        }

        if (Cache.Count > MaxCachedIcons)
        {
            Cache.Clear();
        }

        return Cache.GetOrAdd(hwnd, LoadIcon);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    private static ImageSource? LoadIcon(nint hwnd)
    {
        var sharedIcon = WindowIconSource.TryGetSharedIcon(hwnd);

        if (sharedIcon != 0)
        {
            return ToImageSource(sharedIcon);
        }

        var imagePath = WindowIconSource.TryGetProcessImagePath(hwnd);

        if (imagePath is null)
        {
            return null;
        }

        try
        {
            using var icon = System.Drawing.Icon.ExtractAssociatedIcon(imagePath);
            return icon is null ? null : ToImageSource(icon.Handle);
        }
        catch (Exception exception) when (exception is ArgumentException or System.IO.IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static ImageSource? ToImageSource(nint iconHandle)
    {
        try
        {
            var image = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            image.Freeze();
            return image;
        }
        catch (Exception exception) when (exception is ArgumentException or System.Runtime.InteropServices.COMException)
        {
            return null;
        }
    }
}
