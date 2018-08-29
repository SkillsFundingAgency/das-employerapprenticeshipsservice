using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Features;
using StructureMap;
using System.Collections.Generic;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class AuthorizationRegistry : Registry
    {
        public AuthorizationRegistry()
        {
            For<FeaturesConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<FeaturesConfiguration>($"{Constants.ServiceName}.FeaturesV2")).Singleton();
            For<IAgreementService>().Use<AgreementService>();

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