using SFA.DAS.AutoConfiguration;
using SFA.DAS.Authentication;
using StructureMap;
using Moq;

namespace SFA.DAS.EmployerAccounts.UnitTests.DependencyResolution.AuditRegistry
{
    public class TestRegistry : Registry
    {
        private Mock<IEnvironmentService> _mockEnvironmentService;
        private Mock<IAuthenticationService> _mockAuthenticationService;

        public TestRegistry()
        {
            _mockEnvironmentService = new Mock<IEnvironmentService>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();

            _mockEnvironmentService.Setup(m => m.IsCurrent(It.IsAny<DasEnv>())).Returns(true);

            For<IEnvironmentService>().Use(_mockEnvironmentService.Object);
            For<IAuthenticationService>().Use(_mockAuthenticationService.Object);
        }
    }
}
