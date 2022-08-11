using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetAccounts
    {
        private AccountsOrchestrator _orchestrator;        
        private Mock<ILog> _log;
        private Mock<IHashingService> _hashingService;
        private Mock<IEmployerAccountsApiService> _apiService;
        private Mock<IEmployerFinanceApiService> _financeApiService;
        private Mock<IMapper> _mapper;
        private AccountWithBalanceViewModel _expectedAccount;
        private AccountBalance _expectedAccountBalance;

        [SetUp]
        public void Arrange()
        {
            _mapper = new Mock<IMapper>();
            _log = new Mock<ILog>();
            _hashingService = new Mock<IHashingService>();
            _apiService = new Mock<IEmployerAccountsApiService>();
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            _orchestrator = new AccountsOrchestrator(_log.Object, _mapper.Object, _hashingService.Object, _apiService.Object, _financeApiService.Object);

            _expectedAccount = new AccountWithBalanceViewModel {  AccountId = 124343 };
            _expectedAccountBalance = new AccountBalance { AccountId = _expectedAccount.AccountId };

            _apiService
                .Setup(x => x.GetAccounts(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedApiResponseViewModel<AccountWithBalanceViewModel>
                {
                    Data = new List<AccountWithBalanceViewModel> { _expectedAccount }
                });

            _financeApiService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>()))
                 .ReturnsAsync( new List<AccountBalance> { _expectedAccountBalance });

        }

        [Test]
        public async Task AndTheAccountHasALevyOverrideThenTheyShouldNotBeAllowedPaymentOnTheService()
        {
            //Arrange
            _expectedAccountBalance.LevyOverride = false;

            //Act
            var response = await _orchestrator.GetAllAccountsWithBalances(DateTime.Now.ToString(), 10, 1);

            //Assert
            Assert.IsFalse(response.Data.Data.First().IsAllowedPaymentOnService);
        }

        [Test]
        public async Task AndTheAccountHasAnEoIAgreementThenTheyShouldBeAllowedPaymentOnTheService()
        {
            //Arrange
            _expectedAccount.AccountAgreementType = AccountAgreementType.NonLevyExpressionOfInterest;

            //Act
            var response = await _orchestrator.GetAllAccountsWithBalances(DateTime.Now.ToString(), 10, 1);

            //Assert
            Assert.IsTrue(response.Data.Data.First().IsAllowedPaymentOnService);
        }

        [Test]
        public async Task AndTheEmployerTypeIsLevyThenTheyShouldBeAllowedPaymentOnTheService()
        {
            //Arrange
            _expectedAccount.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;

            //Act
            var response = await _orchestrator.GetAllAccountsWithBalances(DateTime.Now.ToString(), 10, 1);

            //Assert
            Assert.IsTrue(response.Data.Data.First().IsAllowedPaymentOnService);
        }

        [Test]
        public async Task AndTheEmployerTypeIsNoneLevyThenTheyShouldNotBeAllowedPaymentOnTheService()
        {
            //Arrange
            _expectedAccount.ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy;

            //Act
            var response = _orchestrator.GetAllAccountsWithBalances(DateTime.Now.ToString(), 10, 1);

            //Assert
            Assert.IsFalse(response.Result.Data.Data.First().IsAllowedPaymentOnService);
        }

        [Test]
        public async Task AndThereAreNoSignedAgreementsThenTheyShouldNotBeAllowedPaymentOnTheService()
        {
            //Arrange
            _expectedAccount.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;
            _expectedAccount.AccountAgreementType = AccountAgreementType.Unknown;

            //Act
            var response = _orchestrator.GetAllAccountsWithBalances(DateTime.Now.ToString(), 10, 1);

            //Assert
            Assert.IsFalse(response.Result.Data.Data.First().IsAllowedPaymentOnService);
        }
    }
}
