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

        public async Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            if (authorizationContext.AccountContext == null || feature.EnabledByAgreementVersion == null)
            {
                return AuthorizationResult.Ok;
            }

            var latestSignedAgreementVersion = await _accountAgreementService.GetLatestSignedAgreementVersionAsync(authorizationContext.AccountContext.Id);
			var isFeatureAgreementSigned = latestSignedAgreementVersion >= feature.EnabledByAgreementVersion.Value;

            return isFeatureAgreementSigned
                ? AuthorizationResult.Ok
                : AuthorizationResult.FeatureAgreementNotSigned;
        }
    }
}