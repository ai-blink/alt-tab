using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public interface IWindowActivator
{
    bool TryActivate(WindowSnapshot window);
}
