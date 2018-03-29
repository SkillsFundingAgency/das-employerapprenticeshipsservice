using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetStatistics;
using SFA.DAS.EAS.Domain.Data.Entities;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetStatisticsTests
{
    [TestFixture]
    public class WhenIGetTheStatistics
    {
        private GetStatisticsHandler _handler;
        private Mock<IStatisticsRepository> _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IStatisticsRepository>();
            _handler = new GetStatisticsHandler(_repository.Object);
        }

        [Test]
        public async Task ThenTheRepositoryMethodGetStatisticsIsCalled()
        {
            SetupTheRepositoryToReturnData(true);
            await _handler.Handle(new GetStatisticsRequest());

            _repository.Verify(o => o.GetStatistics(), Times.Once);
        }

        [Test]
        public async Task ThenIfTheRepositoryMethodGetStatisticsIsCalledAndReturnsNoDataAndEmptyObjectIsReturned()
        {
            SetupTheRepositoryToReturnData(false);
            var actual = await _handler.Handle(new GetStatisticsRequest());

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Statistics.TotalPayments);
            Assert.AreEqual(0, actual.Statistics.TotalAccounts);
            Assert.AreEqual(0, actual.Statistics.TotalAgreements);
            Assert.AreEqual(0, actual.Statistics.TotalLegalEntities);
            Assert.AreEqual(0, actual.Statistics.TotalPAYESchemes);
        }

        [Test]
        public async Task ThenTheResponseIsReturned()
        {
            SetupTheRepositoryToReturnData(true);

            var actual = await _handler.Handle(new GetStatisticsRequest());

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Statistics);
            Assert.AreEqual(1, actual.Statistics.TotalAccounts);
            Assert.AreEqual(2, actual.Statistics.TotalPayments);
            Assert.AreEqual(3, actual.Statistics.TotalLegalEntities);
            Assert.AreEqual(4, actual.Statistics.TotalAgreements);
            Assert.AreEqual(5, actual.Statistics.TotalPAYESchemes);
        }

        private void SetupTheRepositoryToReturnData(bool returnInstantiatedObject)
        {
            _repository.Setup(o => o.GetStatistics())
                .ReturnsAsync(returnInstantiatedObject ? new Statistics()
                {
                    TotalAccounts = 1,
                    TotalPayments = 2,
                    TotalLegalEntities = 3,
                    TotalAgreements = 4,
                    TotalPAYESchemes = 5
                } : null);
        }
    }
}
