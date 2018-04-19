using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureEnabledAuthorisationHandler : IAuthorizationHandler
    {
        public Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            return feature.Enabled
                ? Task.FromResult(AuthorizationResult.Ok)
                : Task.FromResult(AuthorizationResult.FeatureDisabled);
        }
    }
}