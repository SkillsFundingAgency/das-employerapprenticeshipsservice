using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview;
using SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetAccountFinanceOverviewTests
{
    public class WhenIGetAccountBalance
    {
        private const long ExpectedAccountId = 20;
        private const decimal ExpectedCurrentFunds = 2345.67M;

        private GetAccountFinanceOverviewQueryHandler _handler;
        private Mock<IDasForecastingService> _forecastingService;
        private Mock<ILog> _logger;
        private Mock<IDasLevyService> _levyService;
        private GetAccountFinanceOverviewQuery _query;
        private ExpiringAccountFunds _expiringFunds;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILog>();
            _forecastingService = new Mock<IDasForecastingService>();
            _levyService = new Mock<IDasLevyService>();

            _query = new GetAccountFinanceOverviewQuery{AccountId = ExpectedAccountId};
            _expiringFunds = new ExpiringAccountFunds
            {
                AccountId = ExpectedAccountId,
                ExpiryAmounts = new List<ExpiringFunds>
                {
                    new ExpiringFunds {PayrollDate = new DateTime(2019, 4, 6), Amount = 3000},
                    new ExpiringFunds {PayrollDate = new DateTime(2019, 5, 6), Amount = 4000},
                    new ExpiringFunds {PayrollDate = new DateTime(2019, 3, 6), Amount = 2000}
                }
            };

            _handler = new GetAccountFinanceOverviewQueryHandler(_forecastingService.Object, _levyService.Object, _logger.Object);
            _forecastingService.Setup(s => s.GetExpiringAccountFunds(ExpectedAccountId)).ReturnsAsync(_expiringFunds);
            _levyService.Setup(s => s.GetAccountBalance(ExpectedAccountId)).ReturnsAsync(ExpectedCurrentFunds);
        }

        [Test]
        public async Task ThenTheCurrentFundsShouldBeReturned()
        {
            var response = await _handler.Handle(_query);

            response.CurrentFunds.Should().Be(ExpectedCurrentFunds);
        }

        [Test]
        public void ThenIfExceptionOccursGettingBalanceItShouldBeLogged()
        {
            var expectedException = new Exception("Test error");

            _levyService.Setup(s => s.GetAccountBalance(ExpectedAccountId)).Throws(expectedException);

            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_query));

            _logger.Verify(l => l.Error(expectedException, It.IsAny<string>()), Times.Once);
        }
    }
}
