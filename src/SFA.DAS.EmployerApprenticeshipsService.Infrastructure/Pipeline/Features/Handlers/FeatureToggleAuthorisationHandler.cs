using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    /// <summary>
    ///     Exposes the <see cref="IFeatureService"/> as a <see cref="IOperationAuthorisationHandler"/>.
    /// </summary>
    public class FeatureToggleAuthorisationHandler : IOperationAuthorisationHandler
    {
        private readonly IFeatureWhiteListingService _featureWhiteListingService;

        public FeatureToggleAuthorisationHandler(IFeatureWhiteListingService featureWhiteListingService)
        {
            _featureWhiteListingService = featureWhiteListingService;
        }

        public Task<bool> CanAccessAsync(OperationContext context)
        {
            return _featureWhiteListingService.IsFeatureEnabledForContextAsync(context);
        }
    }
}