using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    public class FeatureEnabledAuthorisationHandler : IOperationAuthorisationHandler
    {
        public Task<bool> CanAccessAsync(IAuthorizationContext authorisationContext)
        {
            return authorisationContext.CurrentFeature == null || authorisationContext.CurrentFeature.Enabled ? 
                FeatureHandlerResults.FeatureEnabledTask : FeatureHandlerResults.FeatureDisabledTask;
        }
    }
}