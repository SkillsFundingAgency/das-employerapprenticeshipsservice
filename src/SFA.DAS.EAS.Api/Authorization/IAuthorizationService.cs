using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Account.Api.Authorization
{
    public interface IAuthorizationService
    {
        IAuthorizationContext GetAuthorizationContext();
    }
}