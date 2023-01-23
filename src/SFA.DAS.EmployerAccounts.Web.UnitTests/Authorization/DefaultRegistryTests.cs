using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.HashingService;
using StructureMap;

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
        public void AuthorizationContext_WhenResolvingIAuthorizationContextProvider_ThenReturnInstanceOfImpersonationAuthorizationContext()
        {
            //Act
            var instance = Container.GetInstance<IAuthorizationContextProvider>();

            //Assert
            Assert.IsInstanceOf<ImpersonationAuthorizationContext>(instance);
        }

        [Test]
        public void AuthorizationContext_WhenResolvingIAuthorisationResourceRepository_ThenReturnInstanceOfAuthorisationResourceRepository()
        {
            //Act
            var instance = Container.GetInstance<IAuthorisationResourceRepository>();

            //Assert
            Assert.IsInstanceOf<AuthorisationResourceRepository>(instance);
        }

        private class DefaultRegistryTests : Registry
        {
            public DefaultRegistryTests()
            {
                var mockHttpContextBase = new Mock<HttpContextBase>();
                var mockHashingService = new Mock<IHashingService>();
                var mockEmployerAccountTeamRepository = new Mock<IEmployerAccountTeamRepository>();
                var mockAuthenticationService = new Mock<IAuthenticationService>();

                For<HttpContextBase>().Use(() => mockHttpContextBase.Object);
                For<IHashingService>().Use(() => mockHashingService.Object);
                For<IEmployerAccountTeamRepository>().Use(() => mockEmployerAccountTeamRepository.Object);
                For<IAuthenticationService>().Use(() => mockAuthenticationService.Object);
            }
        }

    }
}
