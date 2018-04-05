using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    public class AgreementFeatureAuthorisationHandler : IOperationAuthorisationHandler
    {
        private readonly IAccountAgreementService _accountAgreementService;

        public AgreementFeatureAuthorisationHandler(
            IAccountAgreementService accountAgreementService)
        {
            _accountAgreementService = accountAgreementService;
        }

        public Task<bool> CanAccessAsync(IAuthorizationContext authorisationContext)
        {
            return IsFeatureEnabled(authorisationContext);
        }

        public async Task<bool> IsFeatureEnabled(IAuthorizationContext authorisationContext)
        {
            if (authorisationContext?.AccountContext?.Id == null)
            {
                return true;
            }

            var latestAgreementForAccount =
                await _accountAgreementService.GetLatestAgreementSignedByAccountAsync(authorisationContext.AccountContext.Id);

            return latestAgreementForAccount >= authorisationContext.CurrentFeature.EnabledByAgreementVersion;
        }
    }
}