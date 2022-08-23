using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Caches;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.DependencyResolution;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerFinance.UnitTests.DependencyResolution
{
    internal class WhenIResolveProviderService
    {
        private IContainer _container;
        private Mock<IDasLevyRepository> _mockDasLevyRepository;
        private Mock<ILog> _mockLog;
        private Mock<IInProcessCache> _mockInProcessCache;

        [SetUp]
        public void Arrange()
        {
            _mockDasLevyRepository = new Mock<IDasLevyRepository>();
            _mockLog = new Mock<ILog>();
            _mockInProcessCache = new Mock<IInProcessCache>();
            var config = new EmployerFinanceConfiguration{EmployerFinanceOuterApiConfiguration = new EmployerFinanceOuterApiConfiguration { BaseUrl = "http://BaseUrl.education.gov.uk" , Key="123" }};

            _container = new Container(c =>
            {
                c.AddRegistry<EmployerFinanceOuterApiRegistry>();
                c.AddRegistry<ProvidersRegistry>();
                c.For<IDasLevyRepository>().Use(_mockDasLevyRepository.Object);
                c.For<EmployerFinanceConfiguration>().Use(config);
                c.For<ILog>().Use(_mockLog.Object);
                c.For<IInProcessCache>().Use(_mockInProcessCache.Object);
            });
        }

        [Test]
        public void ThenTheDependencyTreeIsResolvedCorrectly()
        {
            var providerServiceCache = _container.GetInstance<IProviderService>();
            Assert.IsAssignableFrom<ProviderServiceCache>(providerServiceCache);

            var providerServiceRemote = GetChildService<ProviderServiceRemote>(providerServiceCache);
            Assert.IsNotNull(providerServiceRemote);

            var ProviderServiceFromDb = GetChildService<ProviderServiceFromDb>(providerServiceRemote);
            Assert.IsNotNull(ProviderServiceFromDb);
        }

        private static T GetChildService<T>(IProviderService parent)
        {
            var flags = BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic;

            var properties = parent.GetType().GetFields(flags);

            var temp = properties.FirstOrDefault(p => p.FieldType.Equals(typeof(IProviderService)));
            return (T)temp.GetValue(parent);
        }
    }
}