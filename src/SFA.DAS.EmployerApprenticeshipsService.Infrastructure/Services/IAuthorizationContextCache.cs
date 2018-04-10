using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public interface IAuthorizationContextCache
    {
        IAuthorizationContext GetAuthorizationContext();
        void SetAuthorizationContext(IAuthorizationContext authorizationContext);
    }
}