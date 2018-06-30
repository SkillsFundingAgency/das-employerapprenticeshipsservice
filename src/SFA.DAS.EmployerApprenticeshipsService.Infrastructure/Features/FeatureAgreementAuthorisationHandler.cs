using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureAgreementAuthorisationHandler : IAuthorizationHandler
    {
        private static readonly Type Type = typeof(FeatureAgreementAuthorisationHandler);
        
        private readonly IAgreementService _agreementService;
        private readonly ILog _logger;

        public FeatureAgreementAuthorisationHandler(IAgreementService agreementService, ILog logger)
        {
            _agreementService = agreementService;
            _logger = logger;
        }

        public async Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            AuthorizationResult result;

            _logger.Debug($"Started running '{Type.Name}' for feature '{feature.FeatureType}'");

            if (authorizationContext.AccountContext == null || feature.EnabledByAgreementVersion == null)
            {
                result = AuthorizationResult.Ok;
            }
            else
            {
                var agreementVersion = await _agreementService.GetAgreementVersionAsync(authorizationContext.AccountContext.Id).ConfigureAwait(false);
                var isFeatureAgreementSigned = agreementVersion >= feature.EnabledByAgreementVersion.Value;

                result = isFeatureAgreementSigned
                    ? AuthorizationResult.Ok
                    : AuthorizationResult.FeatureAgreementNotSigned;
            }

            _logger.Debug($"Finished running '{Type.Name}' for feature '{feature.FeatureType}' with result '{result}'");

            return result;
        }
    }
}