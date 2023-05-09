using SFA.DAS.Authorization.Context;

namespace SFA.DAS.EAS.Web.Authorization;

public class AuthorizationContextProvider : IAuthorizationContextProvider
{
    public IAuthorizationContext GetAuthorizationContext()
    {
        return new AuthorizationContext();
    }
}