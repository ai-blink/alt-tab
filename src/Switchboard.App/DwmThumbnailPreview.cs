using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Switchboard.App;

public sealed class DwmThumbnailPreview : FrameworkElement
{
    public static readonly DependencyProperty SourceHandleProperty =
        DependencyProperty.Register(
            nameof(SourceHandle),
            typeof(nint),
            typeof(DwmThumbnailPreview),
            new FrameworkPropertyMetadata((nint)0, OnSourceHandleChanged));

    private const int DwmTnpRectDestination = 0x00000001;
    private const int DwmTnpOpacity = 0x00000004;
    private const int DwmTnpVisible = 0x00000008;
    private const int DwmTnpSourceClientAreaOnly = 0x00000010;

    private nint thumbnail;
    private nint destinationWindow;

    public DwmThumbnailPreview()
    {
        IsHitTestVisible = false;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        LayoutUpdated += OnLayoutUpdated;
        SizeChanged += OnSizeChanged;
    }

    public nint SourceHandle
    {
        get => (nint)GetValue(SourceHandleProperty);
        set => SetValue(SourceHandleProperty, value);
    }

    private static void OnSourceHandleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is DwmThumbnailPreview preview)
        {
            preview.RegisterThumbnail();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => RegisterThumbnail();

    private void OnUnloaded(object sender, RoutedEventArgs e) => UnregisterThumbnail();

    private void OnLayoutUpdated(object? sender, EventArgs e) => UpdateThumbnail();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => UpdateThumbnail();

    private void RegisterThumbnail()
    {
        UnregisterThumbnail();

        if (!IsLoaded || SourceHandle == 0)
        {
            return;
        }

        var window = Window.GetWindow(this);

        if (window is null)
        {
            return;
        }

        destinationWindow = new WindowInteropHelper(window).Handle;

        if (destinationWindow == 0 || destinationWindow == SourceHandle)
        {
            return;
        }

        var result = DwmRegisterThumbnail(destinationWindow, SourceHandle, out thumbnail);

        if (result != 0)
        {
            thumbnail = 0;
            return;
        }

        UpdateThumbnail();
    }

    private void UnregisterThumbnail()
    {
        if (thumbnail == 0)
        {
            return;
        }

        _ = DwmUnregisterThumbnail(thumbnail);
        thumbnail = 0;
    }

    private void UpdateThumbnail()
    {
        if (thumbnail == 0 || !IsLoaded || ActualWidth <= 0 || ActualHeight <= 0)
        {
            return;
        }

        var window = Window.GetWindow(this);

        if (window is null)
        {
            return;
        }

        var source = PresentationSource.FromVisual(window);
        var transform = source?.CompositionTarget?.TransformToDevice ?? Matrix.Identity;
        var point = TransformToAncestor(window).Transform(new Point(0, 0));
        var left = (int)Math.Round(point.X * transform.M11);
        var top = (int)Math.Round(point.Y * transform.M22);
        var width = (int)Math.Round(ActualWidth * transform.M11);
        var height = (int)Math.Round(ActualHeight * transform.M22);

        if (width <= 0 || height <= 0)
        {
            return;
        }

        var destination = FitDestinationRect(left, top, width, height);
        var properties = new DwmThumbnailProperties
        {
            Flags = DwmTnpRectDestination | DwmTnpVisible | DwmTnpOpacity | DwmTnpSourceClientAreaOnly,
            Destination = destination,
            Opacity = 255,
            Visible = true,
            SourceClientAreaOnly = false
        };

        _ = DwmUpdateThumbnailProperties(thumbnail, ref properties);
    }

    private DwmRect FitDestinationRect(int left, int top, int width, int height)
    {
        if (DwmQueryThumbnailSourceSize(thumbnail, out var sourceSize) != 0 ||
            sourceSize.Width <= 0 ||
            sourceSize.Height <= 0)
        {
            return new DwmRect(left, top, left + width, top + height);
        }

        var sourceRatio = (double)sourceSize.Width / sourceSize.Height;
        var targetRatio = (double)width / height;
        var renderWidth = width;
        var renderHeight = height;

        if (sourceRatio > targetRatio)
        {
            renderHeight = Math.Max(1, (int)Math.Round(width / sourceRatio));
        }
        else
        {
            renderWidth = Math.Max(1, (int)Math.Round(height * sourceRatio));
        }

        var renderLeft = left + (width - renderWidth) / 2;
        var renderTop = top + (height - renderHeight) / 2;
        return new DwmRect(renderLeft, renderTop, renderLeft + renderWidth, renderTop + renderHeight);
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmRegisterThumbnail(nint hwndDestination, nint hwndSource, out nint phThumbnailId);

    [DllImport("dwmapi.dll")]
    private static extern int DwmUnregisterThumbnail(nint hThumbnailId);

    [DllImport("dwmapi.dll")]
    private static extern int DwmUpdateThumbnailProperties(nint hThumbnailId, ref DwmThumbnailProperties ptnProperties);

    [DllImport("dwmapi.dll")]
    private static extern int DwmQueryThumbnailSourceSize(nint hThumbnail, out DwmSize pSize);

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct DwmRect
    {
        public DwmRect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct DwmSize
    {
        public readonly int Width;
        public readonly int Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DwmThumbnailProperties
    {
        public int Flags;
        public DwmRect Destination;
        public DwmRect Source;
        public byte Opacity;

        [MarshalAs(UnmanagedType.Bool)]
        public bool Visible;

        [MarshalAs(UnmanagedType.Bool)]
        public bool SourceClientAreaOnly;
    }
}
