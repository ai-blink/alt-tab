using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public interface IWindowCatalog
{
    IReadOnlyList<WindowSnapshot> GetOpenWindows();
}
