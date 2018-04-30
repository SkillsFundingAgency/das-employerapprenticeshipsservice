using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureAgreementAuthorisationHandler : IAuthorizationHandler
    {
        private static readonly Type Type = typeof(FeatureAgreementAuthorisationHandler);
        
        private readonly IAccountAgreementService _accountAgreementService;
        private readonly ILog _logger;

        public FeatureAgreementAuthorisationHandler(IAccountAgreementService accountAgreementService, ILog logger)
        {
            _accountAgreementService = accountAgreementService;
            _logger = logger;
        }

        public async Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            _logger.Debug($"Started running '{Type.Name}' for feature '{feature.FeatureType}'");

            if (authorizationContext.AccountContext == null || feature.EnabledByAgreementVersion == null)
            {
                return AuthorizationResult.Ok;
            }
            
            var latestSignedAgreementVersion = await _accountAgreementService.GetLatestSignedAgreementVersionAsync(authorizationContext.AccountContext.Id).ConfigureAwait(false);
			var isFeatureAgreementSigned = latestSignedAgreementVersion >= feature.EnabledByAgreementVersion.Value;
            var result = isFeatureAgreementSigned ? AuthorizationResult.Ok : AuthorizationResult.FeatureAgreementNotSigned;

            _logger.Debug($"Finished running '{Type.Name}' for feature '{feature.FeatureType}' with result '{result}'");

            return result;
        }
    }
}