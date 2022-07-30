using AutoMapper;
using Castle.Core.Internal;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetAnAccount
    {
        private AccountsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _log;
        private Mock<IHashingService> _hashingService;
        private Mock<IEmployerAccountsApiService> _apiService;
        private Mock<IEmployerFinanceApiService> _financeApiService;
        private IMapper _mapper;
        private TransferAllowance _transferAllowance;

        private const decimal AccountBalance = 678.90M;
        private const string HashedAgreementId = "ABC123";

        private AccountDetailViewModel _accountDetailViewModel;
        private AccountBalance _accountBalanceResult;


        [SetUp]
        public void Arrange()
        {
            _transferAllowance = new TransferAllowance { RemainingTransferAllowance = 123.45M, StartingTransferAllowance = 234.56M };
            _mediator = new Mock<IMediator>();
            _mapper = ConfigureMapper();
            _log = new Mock<ILog>();
            _hashingService = new Mock<IHashingService>();
            _apiService = new Mock<IEmployerAccountsApiService>();
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            _orchestrator = new AccountsOrchestrator(_mediator.Object, _log.Object, _mapper, _hashingService.Object, _apiService.Object, _financeApiService.Object);
        
            _accountDetailViewModel = new AccountDetailViewModel { AccountId = 1, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString() };

            _apiService
                .Setup(x => x.GetAccount("ABC123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountDetailViewModel)
                .Verifiable("Get account was not called"); 

            _mediator
                .Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>()))
                .ReturnsAsync(new GetTransferAllowanceResponse { TransferAllowance = _transferAllowance })
                .Verifiable("Get transfer balance was not called");

            _accountBalanceResult = new AccountBalance { Balance = AccountBalance };

            _mediator
                .Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>()))
                .ReturnsAsync(new GetAccountBalancesResponse
                {
                    Accounts = new List<AccountBalance> { _accountBalanceResult }
                })
                .Verifiable("Get account balance was not called");
        }

        [Test]
        public async Task ThenARequestShouldMakeCallsToGetBalances()
        {
            //Act
            await _orchestrator.GetAccount(HashedAgreementId);

            //Assert
            _mediator.VerifyAll();
        }

        [Test]
        public async Task ThenResponseShouldHaveBalanceSet()
        {
            //Act
            var result = await _orchestrator.GetAccount(HashedAgreementId);

            //Assert
            Assert.AreEqual(AccountBalance, result.Data.Balance);
        }

        [Test]
        public async Task ThenResponseShouldHaveTransferAllowanceSet()
        {
            //Act
            var result = await _orchestrator.GetAccount(HashedAgreementId);

            //Assert
            Assert.AreEqual(_transferAllowance.RemainingTransferAllowance, result.Data.RemainingTransferAllowance);
        }

        [Test]
        public async Task AndTheAccountHasALevyOverrideThenTheyShouldNotBeAllowedPaymentOnTheService()
        {
            _accountBalanceResult.LevyOverride = false;

            var response = await _orchestrator.GetAccount(HashedAgreementId);

            Assert.IsFalse(response.Data.IsAllowedPaymentOnService);
        }

        [Test]
        public async Task AndTheAccountHasAnEoIAgreementThenTheyShouldBeAllowedPaymentOnTheService()
        {
            _accountDetailViewModel.AccountAgreementType = AccountAgreementType.NonLevyExpressionOfInterest;

            var response = await _orchestrator.GetAccount(HashedAgreementId);

            Assert.IsTrue(response.Data.IsAllowedPaymentOnService);
        }

        [Test]
        public async Task AndTheEmployerTypeIsLevyThenTheyShouldBeAllowedPaymentOnTheService()
        {
            _accountDetailViewModel.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString();

            var response = await _orchestrator.GetAccount(HashedAgreementId);

            Assert.IsTrue(response.Data.IsAllowedPaymentOnService);
        }

        [Test]
        public async Task AndTheEmployerTypeIsNoneLevyThenTheyShouldNotBeAllowedPaymentOnTheService()
        {
            _accountDetailViewModel.ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy.ToString();

            var response = await _orchestrator.GetAccount(HashedAgreementId);

            Assert.IsFalse(response.Data.IsAllowedPaymentOnService);
        }

        private IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile) Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}
