using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class FeatureAgreementService : IFeatureAgreementService
    {
        private readonly IAccountAgreementService _accountAgreementService;
        private readonly IFeatureService _featureService;

        public FeatureAgreementService(
            IAccountAgreementService accountAgreementService, 
            IFeatureService featureService)
        {
            _accountAgreementService = accountAgreementService;
            _featureService = featureService;
        }

        public async Task<bool> IsFeatureEnabled(OperationContext context)
        {
            var feature = await _featureService.GetFeatureThatAllowsAccessToOperationAsync(context);

            if (feature == null)
            {
                return true;
            }

            var latestAgreementForAccount =
                await _accountAgreementService.GetLatestAgreementSignedByAccountAsync(context.MembershipContext.AccountId);

            return latestAgreementForAccount >= feature.EnabledByAgreementVersion;
        }
    }
}
