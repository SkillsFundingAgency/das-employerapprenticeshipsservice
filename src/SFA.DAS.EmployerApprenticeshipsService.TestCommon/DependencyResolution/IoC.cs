using Moq;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.TestCommon.MockPolicy;
using SFA.DAS.EAS.Web;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Messaging;
using StructureMap;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public static class IoC
    {
        public static Container CreateContainer(Mock<IMessagePublisher> messagePublisher, Mock<IOwinWrapper> owinWrapper, Mock<ICookieService> cookieService)
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                c.Policies.Add(new ConfigurationPolicy<AuditApiClientConfiguration>("SFA.DAS.AuditApiClient"));
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MockMessagePolicy(messagePublisher));
                c.AddRegistry(new DefaultRegistry(owinWrapper, cookieService));
            });
        }
    }
}
