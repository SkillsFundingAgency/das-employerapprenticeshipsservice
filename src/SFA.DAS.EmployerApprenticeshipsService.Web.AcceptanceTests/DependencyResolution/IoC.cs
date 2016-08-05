using Moq;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.DepedencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.MockPolicy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.Messaging;
using StructureMap;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution
{
    public static class IoC
    {
        public static Container CreateContainer(Mock<IMessagePublisher> messagePublisher, Mock<IOwinWrapper> owinWrapper)
        {
            return new Container(c =>
            {
                c.Policies.Add<ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>>();
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MockMessagePolicy(messagePublisher));
                c.AddRegistry(new DefaultRegistry(owinWrapper));
            });
        }
    }
}
