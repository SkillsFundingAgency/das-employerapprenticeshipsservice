using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetStatistics;
using SFA.DAS.EAS.Domain.Data.Entities.Statistics;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetStatisticsTests
{
    [TestFixture]
    public class WhenIGetTheStatistics
    {
        private GetStatisticsHandler _handler;
        private Mock<IStatisticsAccountsRepository> _repositoryAccounts;
        private Mock<IStatisticsFinancialRepository> _repositoryFinancial;

        [SetUp]
        public void Setup()
        {
            _repositoryAccounts = new Mock<IStatisticsAccountsRepository>();
            _repositoryFinancial = new Mock<IStatisticsFinancialRepository>();
            _handler = new GetStatisticsHandler(_repositoryAccounts.Object, _repositoryFinancial.Object);
        }

        [Test]
        public async Task ThenTheRepositoryMethodGetStatisticsIsCalled()
        {
            SetupTheRepositoryToReturnData(true);
            await _handler.Handle(new GetStatisticsRequest());

            _repositoryAccounts.Verify(o => o.GetStatistics(), Times.Once);
            _repositoryFinancial.Verify(rf => rf.GetStatistics(), Times.Once);
        }

        [Test]
        public async Task ThenIfTheRepositoryMethodGetStatisticsIsCalledAndReturnsNoDataAndEmptyObjectIsReturned()
        {
            SetupTheRepositoryToReturnData(false);
            var actual = await _handler.Handle(new GetStatisticsRequest());

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Statistics.TotalPaymentsThisYear);
            Assert.AreEqual(0, actual.Statistics.TotalAccounts);
            Assert.AreEqual(0, actual.Statistics.TotalSignedAgreements);
            Assert.AreEqual(0, actual.Statistics.TotalActiveLegalEntities);
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
            Assert.AreEqual(2, actual.Statistics.TotalPaymentsThisYear);
            Assert.AreEqual(3, actual.Statistics.TotalActiveLegalEntities);
            Assert.AreEqual(4, actual.Statistics.TotalSignedAgreements);
            Assert.AreEqual(5, actual.Statistics.TotalPAYESchemes);
        }

        private void SetupTheRepositoryToReturnData(bool returnInstantiatedObject)
        {
            _repositoryAccounts.Setup(o => o.GetStatistics())
                .ReturnsAsync(returnInstantiatedObject ? new StatisticsAccounts()
                {
                    TotalAccounts = 1,
                    TotalLegalEntities = 3,
                    TotalAgreements = 4,
                    TotalPAYESchemes = 5
                } : null);

            _repositoryFinancial.Setup(rf => rf.GetStatistics()).ReturnsAsync(returnInstantiatedObject
                ? new StatisticsFinancial()
                {
                    TotalPayments = 2
                }
                : null);
        }
    }
}
