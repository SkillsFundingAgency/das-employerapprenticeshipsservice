using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    /// <summary>
    ///     Exposes the <see cref="IFeatureAgreementService"/> as a <see cref="IOperationAuthorisationHandler"/>.
    /// </summary>
    public class AgreementFeatureAuthorisationHandler : IOperationAuthorisationHandler
    {
        private readonly IFeatureAgreementService _featureAgreementService;

        public AgreementFeatureAuthorisationHandler(IFeatureAgreementService featureAgreementService)
        {
            _featureAgreementService = featureAgreementService;
        }

        public Task<bool> CanAccessAsync(OperationContext context)
        {
            return _featureAgreementService.IsFeatureEnabled(context);
        }
    }
}