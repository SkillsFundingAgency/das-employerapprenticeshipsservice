using StructureMap;
using System.Collections.Generic;
using SFA.DAS.Authorization;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerFinance.Authorization;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Features;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class AuthorizationRegistry : Registry
    {
        public AuthorizationRegistry()
        {
            For<FeaturesConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<FeaturesConfiguration>(ConfigurationKeys.Features)).Singleton();
            For<IAgreementService>().Use<AgreementService>();
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