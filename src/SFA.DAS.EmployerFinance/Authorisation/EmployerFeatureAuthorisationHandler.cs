using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.EmployerFeatures.Context;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Options;
using SFA.DAS.Authorization.Results;
using SFA.DAS.EmployerFinance.Authorisation;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Accounts;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Accounts;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.AuthorisationExtensions
{
    public class EmployerFeatureAuthorizationHandler : IAuthorizationHandler
    {
        public string Prefix => "EmployerFeature.";

        private readonly IFeatureTogglesService<EmployerFeatureToggle> _featureTogglesService;
        private readonly IOuterApiClient _outerApiClient;

        public EmployerFeatureAuthorizationHandler(IFeatureTogglesService<EmployerFeatureToggle> featureTogglesService, IOuterApiClient outerApiClient)
        {
            _featureTogglesService = featureTogglesService;
            _outerApiClient = outerApiClient;
        }

        public async Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            var authorizationResult = new AuthorizationResult();

            if (options.Count > 0)
            {
                options.EnsureNoAndOptions();
                options.EnsureNoOrOptions();

                var feature = options.Single();
                var featureToggle = _featureTogglesService.GetFeatureToggle(feature);

                if (featureToggle.EnabledByAgreementVersion.GetValueOrDefault(0) > 0)
                {
                    var (accountId, _) = authorizationContext.GetEmployerFeatureValues();
                    var response = await _outerApiClient
                        .Get<GetMinimumSignedAgreementVersionResponse>(new GetMinimumSignedAgreementVersionRequest(accountId.GetValueOrDefault(0)))
                        .ConfigureAwait(false);

                    if (response.MinimumSignedAgreementVersion < featureToggle.EnabledByAgreementVersion)
                    {
                        authorizationResult.AddError(new EmployerFeatureAgreementNotSigned());
                    }
                }
            }

            return authorizationResult;
        }
    }
}
