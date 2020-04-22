using MediatR;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.EmployerFeatures.Context;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Options;
using SFA.DAS.Authorization.Results;
using SFA.DAS.EmployerAccounts.Authorisation;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.AuthorisationExtensions
{
    public class EmployerFeatureAuthorizationHandler : IAuthorizationHandler
    {
        public string Prefix => "EmployerFeature.";

        private readonly IFeatureTogglesService<EmployerFeatureToggle> _featureTogglesService;
        private readonly IMediator _mediator;

        public EmployerFeatureAuthorizationHandler(IFeatureTogglesService<EmployerFeatureToggle> featureTogglesService, IMediator mediator)
        {
            _featureTogglesService = featureTogglesService;
            _mediator = mediator;
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

                var featureToggleEnabledByAgreementVersion = featureToggle.EnabledByAgreementVersion;
                if (featureToggleEnabledByAgreementVersion.GetValueOrDefault(0) > 0)
                {
                    var (accountId, _) = authorizationContext.GetEmployerFeatureValues();

                    var agreements = await _mediator.SendAsync(new GetEmployerAgreementsByAccountIdRequest { AccountId = accountId.GetValueOrDefault(0) }).ConfigureAwait(false);

                    var minAgreementVersion = agreements.EmployerAgreements.
                        Where(a => a.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Signed &&
                        a.AccountLegalEntity.SignedAgreementVersion.GetValueOrDefault(0) > 0).Select(b => b.AccountLegalEntity.SignedAgreementVersion ?? 0)
                        .DefaultIfEmpty()
                        .Min();

                    if (minAgreementVersion < featureToggleEnabledByAgreementVersion || 
                        agreements.EmployerAgreements.Any(a => a.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Pending &&
                                                               a.TemplateId == featureToggleEnabledByAgreementVersion))
                    {
                        authorizationResult.AddError(new EmployerFeatureAgreementNotSigned());
                    }
                }
            }

            return authorizationResult;
        }
    }
}
