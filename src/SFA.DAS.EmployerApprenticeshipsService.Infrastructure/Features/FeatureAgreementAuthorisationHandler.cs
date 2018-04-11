using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureAgreementAuthorisationHandler : IAuthorizationHandler
    {
        private readonly IAccountAgreementService _accountAgreementService;

        public FeatureAgreementAuthorisationHandler(IAccountAgreementService accountAgreementService)
        {
            _accountAgreementService = accountAgreementService;
        }

        public async Task<bool> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            if (authorizationContext.AccountContext == null || feature.EnabledByAgreementVersion == null)
            {
                return true;
            }

            var latestAgreementForAccount = await _accountAgreementService.GetLatestAgreementSignedByAccountAsync(authorizationContext.AccountContext.Id);

            return latestAgreementForAccount >= feature.EnabledByAgreementVersion.Value;
        }
    }
}