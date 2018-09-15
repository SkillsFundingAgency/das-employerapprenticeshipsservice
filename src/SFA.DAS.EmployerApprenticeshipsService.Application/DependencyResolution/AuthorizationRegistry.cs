using System.Collections.Generic;
using SFA.DAS.Authorization;
using SFA.DAS.Configuration;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Features;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class AuthorizationRegistry : Registry
    {
        public AuthorizationRegistry()
        {
            For<FeaturesConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<FeaturesConfiguration>($"{Constants.ServiceName}.FeaturesV2")).Singleton();
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