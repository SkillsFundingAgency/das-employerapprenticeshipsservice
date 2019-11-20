using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.HashingService;
using StructureMap;
using System.Web;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Authorization
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
                c.AddRegistry<DefaultRegistryTests>();
            });
        }        

        [Test]
        public void AuthorizationContext_WhenResolvingIAuthorizationContextProvider_ThenReturnInstnaceOfImpersonationAuthorizationContext()
        {
            //Act
            var instance = Container.GetInstance<IAuthorizationContextProvider>();

            //Assert
            Assert.IsInstanceOf<ImpersonationAuthorizationContext>(instance);
        }

        private class DefaultRegistryTests : Registry
        {
            private readonly Mock<HttpContextBase> MockHttpContextBase;
            private readonly Mock<IHashingService> MockHashingService;
            private readonly Mock<IEmployerAccountTeamRepository> MockEmployerAccountTeamRepository;
            private readonly Mock<IAuthenticationService> MockAuthenticationService;

            public DefaultRegistryTests()
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
