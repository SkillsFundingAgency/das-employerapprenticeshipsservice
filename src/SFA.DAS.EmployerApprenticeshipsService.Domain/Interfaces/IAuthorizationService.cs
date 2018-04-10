using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAuthorizationService
    {
        IAuthorizationContext GetAuthorizationContext();
        bool IsOperationAuthorized();
        void ValidateMembership();
    }
}