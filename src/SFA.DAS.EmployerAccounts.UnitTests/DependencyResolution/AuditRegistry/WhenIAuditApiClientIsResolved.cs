using NUnit.Framework;
using SFA.DAS.Audit.Client;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.UnitTests.DependencyResolution.AuditRegistry
{
    public class WhenIAuditApiClientIsResolved
    {
        private IContainer _container;

        [SetUp]
        public void Arrange()
        {
            _container = new Container(c =>
            {
                c.AddRegistry<EmployerAccounts.DependencyResolution.AuditRegistry>();
                c.AddRegistry<TestRegistry>();
            });
        }

        [Test]
        public void ThenTheAuditApiClientForSupportUserTypeIsReturned()
        {
            //Act
            var instance = _container.GetInstance<IAuditApiClient>();

            //Assert
            Assert.IsInstanceOf<EmployerAccounts.Services.AuditApiClientForSupportUser>(instance);
        }
    }
}
