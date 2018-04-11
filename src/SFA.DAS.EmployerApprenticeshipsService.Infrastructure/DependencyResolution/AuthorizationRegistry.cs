using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Features;
using StructureMap;

namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    public class AuthorizationRegistry : Registry
    {
        public AuthorizationRegistry()
        {
            For<IAccountAgreementService>().Use<AccountAgreementService>();

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