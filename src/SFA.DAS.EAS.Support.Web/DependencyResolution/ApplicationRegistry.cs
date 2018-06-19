using System.Diagnostics.CodeAnalysis;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Web.Services;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    [ExcludeFromCodeCoverage]
    public class ApplicationRegistry : Registry
    {
        public ApplicationRegistry()
        {
            For<IAccountHandler>().Use<AccountHandler>();
            For<IChallengeRepository>().Use<ChallengeRepository>();
            For<IChallengeService>().Use<ChallengeService>();
            For<IDatetimeService>().Use<DatetimeService>();
            For<IChallengeHandler>().Use<ChallengeHandler>();
            For<ILevySubmissionsRepository>().Use<LevySubmissionsRepository>();
            For<IPayeLevySubmissionsHandler>().Use<PayeLevySubmissionsHandler>();
            For<IPayeLevyMapper>().Use<PayeLevyMapper>();
        }
    }
}