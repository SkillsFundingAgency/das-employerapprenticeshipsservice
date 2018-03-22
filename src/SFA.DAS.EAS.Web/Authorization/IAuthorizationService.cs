using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Web.Authorization
{
    public interface IAuthorizationService
    {
        IAuthorizationContext GetAuthorizationContext();
        void ValidateMembership();
    }
}