using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IAuthorizationContextCache
    {
        IAuthorizationContext GetAuthorizationContext();
        void SetAuthorizationContext(IAuthorizationContext authorizationContext);
    }
}