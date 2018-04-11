using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IAuthorizationService
    {
        IAuthorizationContext GetAuthorizationContext();
        bool IsAuthorized(FeatureType featureType);
        void ValidateMembership();
    }
}