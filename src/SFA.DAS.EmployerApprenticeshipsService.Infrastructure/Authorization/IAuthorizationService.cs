using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IAuthorizationService
    {
        IAuthorizationContext GetAuthorizationContext();
        Task<IAuthorizationContext> GetAuthorizationContextAsync();
        AuthorizationResult GetAuthorizationResult(FeatureType featureType);
        Task<AuthorizationResult> GetAuthorizationResultAsync(FeatureType featureType);
        bool IsAuthorized(FeatureType featureType);
        Task<bool> IsAuthorizedAsync(FeatureType featureType);
        void ValidateMembership();
    }
}