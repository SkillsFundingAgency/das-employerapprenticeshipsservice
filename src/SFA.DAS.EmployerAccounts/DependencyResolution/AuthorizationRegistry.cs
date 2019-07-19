using SFA.DAS.EmployerAccounts.Features;
using StructureMap;
using System.Collections.Generic;
using SFA.DAS.Authorization;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Authorization;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class AuthorizationRegistry : Registry
    {
        public AuthorizationRegistry()
        {
            For<FeaturesConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<FeaturesConfiguration>($"SFA.DAS.EmployerApprenticeshipsService.FeaturesV2")).Singleton();
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