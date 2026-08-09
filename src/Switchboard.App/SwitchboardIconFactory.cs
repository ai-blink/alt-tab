using System.Runtime.InteropServices;
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
        using var bitmap = new Drawing.Bitmap(32, 32);
        using var graphics = Drawing.Graphics.FromImage(bitmap);
        using var backgroundBrush = new Drawing.SolidBrush(Drawing.Color.FromArgb(255, 13, 16, 22));
        using var borderBrush = new Drawing.SolidBrush(Drawing.Color.FromArgb(255, 84, 163, 255));
        using var primaryBrush = new Drawing.SolidBrush(Drawing.Color.FromArgb(255, 104, 182, 255));
        using var secondaryBrush = new Drawing.SolidBrush(Drawing.Color.FromArgb(255, 112, 131, 245));
        using var mutedBrush = new Drawing.SolidBrush(Drawing.Color.FromArgb(255, 87, 100, 122));
        using var connectorPen = new Drawing.Pen(Drawing.Color.White, 2);

        graphics.Clear(Drawing.Color.Transparent);
        graphics.FillRectangle(backgroundBrush, 2, 2, 28, 28);
        graphics.FillRectangle(borderBrush, 3, 3, 26, 1);
        graphics.FillRectangle(primaryBrush, 5, 6, 10, 8);
        graphics.FillRectangle(mutedBrush, 17, 6, 10, 8);
        graphics.FillRectangle(mutedBrush, 5, 18, 10, 8);
        graphics.FillRectangle(secondaryBrush, 17, 18, 10, 8);
        graphics.DrawLine(connectorPen, 12, 16, 20, 16);
        graphics.DrawLine(connectorPen, 17, 13, 20, 16);
        graphics.DrawLine(connectorPen, 17, 19, 20, 16);

        var iconHandle = bitmap.GetHicon();

        try
        {
            return (Drawing.Icon)Drawing.Icon.FromHandle(iconHandle).Clone();
        }
        finally
        {
            _ = DestroyIcon(iconHandle);
        }
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

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(nint hIcon);
}
