using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetAccountFinanceOverviewTests
{
    public class WhenIGetExpiringFunds
    {
        private const long ExpectedAccountId = 20;

        private GetAccountFinanceOverviewQueryHandler _handler;
        private Mock<IDasForecastingService> _dasForecastingService;
        private Mock<IDasLevyService> _levyService;
        private Mock<ILog> _logger;
        private GetAccountFinanceOverviewQuery _query;
        private ExpiringAccountFunds _expiringFunds;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILog>();
            _dasForecastingService = new Mock<IDasForecastingService>();
            _levyService = new Mock<IDasLevyService>();

            _query = new GetAccountFinanceOverviewQuery { AccountId = ExpectedAccountId };
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

            _handler = new GetAccountFinanceOverviewQueryHandler(_dasForecastingService.Object,_levyService.Object, _logger.Object);
            _dasForecastingService.Setup(s => s.GetExpiringAccountFunds(ExpectedAccountId)).ReturnsAsync(_expiringFunds);
            _levyService.Setup(s => s.GetAccountBalance(ExpectedAccountId)).ReturnsAsync(2000);
        }

        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectAccountId()
        {
            var response = await _handler.Handle(_query);

            response.AccountId.Should().Be(ExpectedAccountId);
        }

        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectAmount()
        {
            var response = await _handler.Handle(_query);

            response.ExpiringFundsAmount.Should().Be(2000);
        }

        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectExpiryDate()
        {
            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().BeSameDateAs(new DateTime(2019, 3, 6));
        }

        [Test]
        [Ignore("This logic is likely to change to will be updated when it does change")]
        public async Task ThenIfQueryHasNullAccountIdAnEmptyResponseIsReturned()
        {
            _query.AccountId = null;

            var response = await _handler.Handle(_query);
            
            response.ExpiringFundsExpiryDate.Should().NotHaveValue();
            response.ExpiringFundsAmount.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheAccountIdShouldStillBePopulated()
        {
            _dasForecastingService.Setup(s => s.GetExpiringAccountFunds(ExpectedAccountId)).ReturnsAsync((ExpiringAccountFunds)null);

            var response = await _handler.Handle(_query);

            response.AccountId.Should().Be(ExpectedAccountId);
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheExpiryDateShouldBeNull()
        {
            _dasForecastingService.Setup(s => s.GetExpiringAccountFunds(ExpectedAccountId)).ReturnsAsync((ExpiringAccountFunds)null);

            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheAmountShouldBeNull()
        {
            _dasForecastingService.Setup(s => s.GetExpiringAccountFunds(ExpectedAccountId)).ReturnsAsync((ExpiringAccountFunds)null);

            var response = await _handler.Handle(_query);

            response.ExpiringFundsAmount.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheAccountIdShouldStillBePopulated()
        {
            _expiringFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);

            response.AccountId.Should().Be(ExpectedAccountId);
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheExpiryDateShouldBeNull()
        {
            _expiringFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheAmountShouldBeNull()
        {
            _expiringFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);

            response.ExpiringFundsAmount.Should().NotHaveValue();
        }
    }
}
