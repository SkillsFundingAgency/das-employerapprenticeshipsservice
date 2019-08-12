using SFA.DAS.EmployerAccounts.Features;
using StructureMap;
using System.Collections.Generic;
using SFA.DAS.Authorization;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Services;
using AgreementService = SFA.DAS.EmployerAccounts.Features.AgreementService;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class AuthorizationRegistry : Registry
    {
        public AuthorizationRegistry()
        {
            For<FeaturesConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<FeaturesConfiguration>(ConfigurationKeys.Features)).Singleton();
            For<IAgreementService>().Use<AgreementService>();
            For<IDasRecruitService>().Use<DasRecruitService>();
            For<IAuthorizationService>().Use<AuthorizationService>();

            For<IEnumerable<IAuthorizationHandler>>().Use(c => new List<IAuthorizationHandler>
            {
                c.GetInstance<FeatureEnabledAuthorisationHandler>(),
                c.GetInstance<FeatureWhitelistAuthorizationHandler>(),
                c.GetInstance<FeatureAgreementAuthorisationHandler>()
            });

            For<IFeatureService>().Use<FeatureService>().Singleton();
        }
    }
}