using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Models.ProjectedCalculations;
using SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetAccountFinanceOverviewTests
{
    public class WhenIGetExpiringFunds
    {
        private const long ExpectedAccountId = 20;
        private const long ExpectedBalance = 2000;
        private const decimal ExpectedFundsIn = 1234.56M;
        private const decimal ExpectedFundsOut = 789.01M;

        private DateTime _now;
        private GetAccountFinanceOverviewQueryHandler _handler;
        private Mock<ICurrentDateTime> _currentDateTime;
        private Mock<IDasForecastingService> _dasForecastingService;
        private Mock<IDasLevyService> _levyService;
        private Mock<ILog> _logger;
        private Mock<IValidator<GetAccountFinanceOverviewQuery>> _validator;
        private GetAccountFinanceOverviewQuery _query;
        private AccountProjectionSummary _accountProjectionSummary;

        [SetUp]
        public void Setup()
        {
            _now = DateTime.UtcNow;
            _logger = new Mock<ILog>();
            _currentDateTime = new Mock<ICurrentDateTime>();
            _dasForecastingService = new Mock<IDasForecastingService>();
            _levyService = new Mock<IDasLevyService>();
            _validator = new Mock<IValidator<GetAccountFinanceOverviewQuery>>();

            _query = new GetAccountFinanceOverviewQuery { AccountId = ExpectedAccountId };

            _accountProjectionSummary = new AccountProjectionSummary
            {
                AccountId = ExpectedAccountId,
                ProjectionGenerationDate = DateTime.UtcNow,
                ExpiringAccountFunds = new ExpiringAccountFunds
                {
                    ExpiryAmounts = new List<ExpiringFunds>
                {
                    new ExpiringFunds { PayrollDate = _now.AddMonths(1), Amount = 3000 },
                    new ExpiringFunds { PayrollDate = _now.AddMonths(2), Amount = 4000 },
                    new ExpiringFunds { PayrollDate = _now, Amount = 2000 }
                }
                },
                ProjectionCalulation = new ProjectedCalculation
                {
                    FundsIn = ExpectedFundsIn,
                    FundsOut = ExpectedFundsOut,
                    NumberOfMonths = 12
                }
            };

            _handler = new GetAccountFinanceOverviewQueryHandler(_currentDateTime.Object, _dasForecastingService.Object, _levyService.Object, _validator.Object, _logger.Object);
            _currentDateTime.Setup(d => d.Now).Returns(_now);
            _dasForecastingService.Setup(s => s.GetAccountProjectionSummary(ExpectedAccountId)).ReturnsAsync(_accountProjectionSummary);
            _levyService.Setup(s => s.GetAccountBalance(ExpectedAccountId)).ReturnsAsync(ExpectedBalance);
            _validator.Setup(v => v.ValidateAsync(_query))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
        }

        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectAccountId()
        {
            var response = await _handler.Handle(_query);

            response.AccountId.Should().Be(ExpectedAccountId);
            response.CurrentFunds.Should().Be(ExpectedBalance);
        }

        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectAmount()
        {
            var response = await _handler.Handle(_query);

            response.ExpiringFundsAmount.Should().Be(ExpectedBalance);
        }

        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectExpiryDate()
        {
            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().BeSameDateAs(_now);
        }

        [Test]
        public async Task WhenExpiriesExistBeforeToday_ThenTheExpiringFundsShouldHaveTheCorrectExpiryDate()
        {
            var expiryAmountsBeforeToday = new List<ExpiringFunds>
            {
                new ExpiringFunds { PayrollDate = _now.AddMonths(-4), Amount = 3000 },
                    new ExpiringFunds { PayrollDate = _now.AddMonths(-5), Amount = 4000 },
                    new ExpiringFunds { PayrollDate = _now.AddMonths(-6), Amount = 2000 }
            };

            _accountProjectionSummary.ExpiringAccountFunds.ExpiryAmounts.AddRange(expiryAmountsBeforeToday);

            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().BeSameDateAs(_now);
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheAccountIdAndBalanceShouldStillBePopulated()
        {
            _dasForecastingService.Setup(s => s.GetAccountProjectionSummary(ExpectedAccountId)).ReturnsAsync((AccountProjectionSummary)null);

            var response = await _handler.Handle(_query);

            response.AccountId.Should().Be(ExpectedAccountId);
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheExpiryDateShouldBeNull()
        {
            _dasForecastingService.Setup(s => s.GetAccountProjectionSummary(ExpectedAccountId)).ReturnsAsync((AccountProjectionSummary)null);

            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheAmountShouldBeNull()
        {
            _dasForecastingService.Setup(s => s.GetAccountProjectionSummary(ExpectedAccountId)).ReturnsAsync((AccountProjectionSummary)null);

            var response = await _handler.Handle(_query);

            response.ExpiringFundsAmount.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheAccountIdShouldStillBePopulated()
        {
            _accountProjectionSummary.ExpiringAccountFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);

            response.AccountId.Should().Be(ExpectedAccountId);
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheExpiryDateShouldBeNull()
        {
            _accountProjectionSummary.ExpiringAccountFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheAmountShouldBeNull()
        {
            _accountProjectionSummary.ExpiringAccountFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);

            response.ExpiringFundsAmount.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringInNext12MonthsTheExpiryDateShouldBeNull()
        {
            _accountProjectionSummary.ExpiringAccountFunds.ExpiryAmounts.ForEach(a => a.PayrollDate = a.PayrollDate.AddMonths(13));

            var response = await _handler.Handle(_query);

            response.ExpiringFundsExpiryDate.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringInNext12MonthsTheAmountShouldBeNull()
        {
            _accountProjectionSummary.ExpiringAccountFunds.ExpiryAmounts.ForEach(a => a.PayrollDate = a.PayrollDate.AddMonths(13));

            var response = await _handler.Handle(_query);

            response.ExpiringFundsAmount.Should().NotHaveValue();
        }

        [Test]
        public void ThenIfValidationFailsAnExceptionIsThrown()
        {
            _validator.Setup(v => v.ValidateAsync(_query)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string> { { "Test Error", "Error" } }
            });

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_query));
        }
    }
}
