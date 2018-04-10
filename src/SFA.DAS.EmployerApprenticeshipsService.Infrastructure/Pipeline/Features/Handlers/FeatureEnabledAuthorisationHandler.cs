using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    public class FeatureEnabledAuthorisationHandler : IAuthorizationHandler
    {
        public Task<bool> CanAccessAsync(IAuthorizationContext authorizationContext)
        {
            return authorizationContext.CurrentFeature == null || authorizationContext.CurrentFeature.Enabled ? 
                FeatureHandlerResults.FeatureEnabledTask : FeatureHandlerResults.FeatureDisabledTask;
        }
    }
}