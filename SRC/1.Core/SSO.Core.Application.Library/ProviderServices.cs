using SSO.Core.Application.Library.Interfaces;
using SSO.SharedKernel.Utilities.Library.Scrutor.Abstractions;

namespace SSO.Core.Application.Library;

public class ProviderServices : IScopedLifetime
{
    public IUser User;

    public ProviderServices(IUser user)
    {
        User = user;
    }
}
