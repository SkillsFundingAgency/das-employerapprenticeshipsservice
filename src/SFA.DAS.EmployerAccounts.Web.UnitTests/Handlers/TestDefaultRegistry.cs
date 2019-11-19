using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.HashingService;
using StructureMap;
using System.Web;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Handlers
{
    public class DefaultRegistryResolved
    {
        public IContainer Container { get; set; }

        [SetUp]
        public void Arrange()
        {
            Container = new Container(c =>
            {
                c.AddRegistry<EmployerAccounts.Web.DependencyResolution.DefaultRegistry>();
                c.AddRegistry<TestDefaultRegistry>();
            });
        }

        [Test]
        public void ThenTheAuditApiClientForSupportUserTypeIsReturned()
        {
            //Act
            var instance = Container.GetInstance<IAuthorizationContextProvider>();

            //Assert
            Assert.IsInstanceOf<ImpersonationAuthorizationContext>(instance);
        }


        private class TestDefaultRegistry : Registry
        {
            private Mock<HttpContextBase> MockHttpContextBase;
            private Mock<IHashingService> MockHashingService;
            private Mock<IEmployerAccountTeamRepository> MockEmployerAccountTeamRepository;
            private Mock<IAuthenticationService> MockAuthenticationService;

            public TestDefaultRegistry()
            {
                MockHttpContextBase = new Mock<HttpContextBase>();
                MockHashingService = new Mock<IHashingService>();
                MockEmployerAccountTeamRepository = new Mock<IEmployerAccountTeamRepository>();
                MockAuthenticationService = new Mock<IAuthenticationService>();

                For<HttpContextBase>().Use(() => MockHttpContextBase.Object);
                For<IHashingService>().Use(() => MockHashingService.Object);
                For<IEmployerAccountTeamRepository>().Use(() => MockEmployerAccountTeamRepository.Object);
                For<IAuthenticationService>().Use(() => MockAuthenticationService.Object);
            }
        }
    }
}
