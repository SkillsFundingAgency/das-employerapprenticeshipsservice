using System.Collections.Generic;
using SFA.DAS.Authorization;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Features;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
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