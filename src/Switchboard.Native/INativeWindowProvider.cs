using Switchboard.Core.Models;

namespace Switchboard.Native;

public interface INativeWindowProvider
{
    IReadOnlyList<WindowSnapshot> GetTopLevelWindows();
}
