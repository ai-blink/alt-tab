using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;

namespace Switchboard.App;

internal static class SwitchboardIconFactory
{
    public static Drawing.Icon CreateTrayIcon()
    {
        var resourceUri = new Uri("pack://application:,,,/Assets/switchboard-icon.ico", UriKind.Absolute);
        using var resourceStream = System.Windows.Application.GetResourceStream(resourceUri)?.Stream
            ?? throw new InvalidOperationException("Switchboard icon resource could not be loaded.");
        using var icon = new Drawing.Icon(resourceStream);
        return (Drawing.Icon)icon.Clone();
    }

    public static ImageSource CreateWindowIcon()
    {
        using var icon = CreateTrayIcon();
        var image = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        image.Freeze();
        return image;
    }
}
