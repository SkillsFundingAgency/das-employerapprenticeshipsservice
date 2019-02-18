using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetExpiringAccountFundsTests
{
    [TestFixture]
    public class WhenIGetExpiringFunds
    {
        private const long AccountId = 20;

        private GetExpiringAccountFundsQueryHandler _handler;
        private Mock<IForecastingService> _forecastingService;
        private Mock<ILog> _logger;
        private GetExpiringAccountFundsQuery _query;
        private ExpiringAccountFunds _expiringFunds;


        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILog>();
            _forecastingService = new Mock<IForecastingService>();

            _query = new GetExpiringAccountFundsQuery{AccountId = AccountId};
            _expiringFunds = new ExpiringAccountFunds
            {
                AccountId = AccountId,
                ExpiryAmounts = new List<ExpiringFunds>
                {
                    new ExpiringFunds {PayrollDate = new DateTime(2019, 4, 6), Amount = 3000},
                    new ExpiringFunds {PayrollDate = new DateTime(2019, 5, 6), Amount = 4000},
                    new ExpiringFunds {PayrollDate = new DateTime(2019, 3, 6), Amount = 2000}
                }
            };

            _handler = new GetExpiringAccountFundsQueryHandler(_forecastingService.Object, _logger.Object);

            _forecastingService.Setup(s => s.GetExpiringAccountFunds(AccountId)).ReturnsAsync(_expiringFunds);
        }


        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectAccountId()
        {
            var response = await _handler.Handle(_query);

            response.AccountId.Should().Be(AccountId);
        }
        
        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectAmount()
        {
            var response = await _handler.Handle(_query);
            
            response.Amount.Should().Be(2000);
        }
        
        [Test]
        public async Task ThenTheExpiringFundsShouldHaveTheCorrectExpiryDate()
        {
            var response = await _handler.Handle(_query);
          
            response.ExpiryDate.Should().BeSameDateAs(new DateTime(2019, 3, 6));
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheAccountIdShouldStillBePopulated()
        {
            _forecastingService.Setup(s => s.GetExpiringAccountFunds(AccountId)).ReturnsAsync((ExpiringAccountFunds) null);

            var response = await _handler.Handle(_query);
          
            response.AccountId.Should().Be(AccountId);
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheExpiryDateShouldBeNull()
        {
            _forecastingService.Setup(s => s.GetExpiringAccountFunds(AccountId)).ReturnsAsync((ExpiringAccountFunds) null);

            var response = await _handler.Handle(_query);

            response.ExpiryDate.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNullIsReturnedTheAmountShouldBeNull()
        {
            _forecastingService.Setup(s => s.GetExpiringAccountFunds(AccountId)).ReturnsAsync((ExpiringAccountFunds) null);

            var response = await _handler.Handle(_query);
          
            response.Amount.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheAccountIdShouldStillBePopulated()
        {
            _expiringFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);
          
            response.AccountId.Should().Be(AccountId);
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheExpiryDateShouldBeNull()
        {
            _expiringFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);

            response.ExpiryDate.Should().NotHaveValue();
        }

        [Test]
        public async Task ThenIfNoFundsExpiringTheAmountShouldBeNull()
        {
            _expiringFunds.ExpiryAmounts = new List<ExpiringFunds>();

            var response = await _handler.Handle(_query);
          
            response.Amount.Should().NotHaveValue();
        }
    }
}
