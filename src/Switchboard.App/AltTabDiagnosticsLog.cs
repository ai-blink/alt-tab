using System.Diagnostics;
using System.IO;

namespace Switchboard.App;

// Appends Alt+Tab miss and late-delivery records to %AppData%\Switchboard\logs so intermittent
// fallbacks to the Windows switcher can be attributed after the fact. Writes run on the thread pool
// because reports arrive on the low-level hook thread, which must return immediately.
public static class AltTabDiagnosticsLog
{
    private const long MaxLogBytes = 512 * 1024;
    private static readonly object WriteLock = new();

    public static string LogPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Switchboard",
        "logs",
        "alttab-diagnostics.log");

    public static void Write(string message)
    {
        var line = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff} {message}; {DescribeRuntime()}";
        _ = ThreadPool.UnsafeQueueUserWorkItem(static state => Append((string)state!), line);
    }

    private static string DescribeRuntime()
    {
        using var process = Process.GetCurrentProcess();
        return $"gcPauseTotal={GC.GetTotalPauseDuration().TotalMilliseconds:0}ms gen2={GC.CollectionCount(2)} " +
               $"private={process.PrivateMemorySize64 / (1024 * 1024)}MB";
    }

    private static void Append(string line)
    {
        try
        {
            lock (WriteLock)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);

                if (File.Exists(LogPath) && new FileInfo(LogPath).Length > MaxLogBytes)
                {
                    File.Move(LogPath, LogPath + ".old", overwrite: true);
                }

                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
