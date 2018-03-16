using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    /// <summary>
    ///     Exposes the <see cref="IFeatureToggleService"/> as a <see cref="IOperationAuthorisationHandler"/>.
    /// </summary>
    public class FeatureToggleAuthorisationHandler : IOperationAuthorisationHandler
    {
        private readonly IFeatureToggleService _featureToggleService;

        public FeatureToggleAuthorisationHandler(IFeatureToggleService featureToggleService)
        {
            _featureToggleService = featureToggleService;
        }

        public Task<bool> CanAccessAsync(OperationContext context)
        {
            return _featureToggleService.IsFeatureEnabled(context);
        }
    }
}