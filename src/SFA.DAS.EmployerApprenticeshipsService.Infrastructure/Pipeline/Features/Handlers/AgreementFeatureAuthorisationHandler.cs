using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    public class AgreementFeatureAuthorisationHandler : IAuthorizationHandler
    {
        private readonly IAccountAgreementService _accountAgreementService;

        public AgreementFeatureAuthorisationHandler(IAccountAgreementService accountAgreementService)
        {
            _accountAgreementService = accountAgreementService;
        }

        public async Task<bool> CanAccessAsync(IAuthorizationContext authorizationContext)
        {
            if (authorizationContext.AccountContext == null)
            {
                return true;
            }

            var latestAgreementForAccount = await _accountAgreementService.GetLatestAgreementSignedByAccountAsync(authorizationContext.AccountContext.Id);

            return latestAgreementForAccount >= authorizationContext.CurrentFeature.EnabledByAgreementVersion;
        }
    }
}