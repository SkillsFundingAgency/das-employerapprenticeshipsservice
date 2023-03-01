using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetAllAccountsWithBalances
    {
        private AccountsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _log;
        private Mock<IHashingService> _hashingService;
        private Mock<IEmployerAccountsApiService> _apiService;
        private Mock<IMapper> _mapper;


        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _log = new Mock<ILog>();
            _hashingService = new Mock<IHashingService>();
            _apiService = new Mock<IEmployerAccountsApiService>();
            _mapper = new Mock<IMapper>();
            _orchestrator = new AccountsOrchestrator(_mediator.Object, _log.Object, _mapper.Object, _hashingService.Object, _apiService.Object);

            var accountsResponse = new PagedApiResponseViewModel<AccountWithBalanceViewModel>()
            {
                Page = 123,
                TotalPages = 123,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel { AccountHashId = "ABC123", AccountId = 123, AccountName = "Test 1", IsLevyPayer = true },
                    new AccountWithBalanceViewModel { AccountHashId = "ABC999", AccountId = 987, AccountName = "Test 2", IsLevyPayer = true }
                }
            };

            var balanceResponse = new GetAccountBalancesResponse
            {
                Accounts = new List<AccountBalance>
                {
                    new AccountBalance { AccountId = 123, Balance = 20, RemainingTransferAllowance = 20, StartingTransferAllowance = 10 },
                    new AccountBalance { AccountId = 987, Balance = 200, RemainingTransferAllowance = 200, StartingTransferAllowance = 100 },
                }
            };

            _apiService.Setup(s => s.GetAccounts(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountsResponse);

            _mediator
                .Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>()))
                .ReturnsAsync(balanceResponse);
        }

        [Test]
        public async Task ThenShouldReturnAllAccountsWithBalances()
        {
            var result = await _orchestrator.GetAllAccountsWithBalances(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());

            result.Should().NotBeNull();
            result.Data.ShouldBeEquivalentTo(new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 123,
                Page = 123,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel { AccountHashId = "ABC123", AccountId = 123, AccountName = "Test 1", Balance = 20, IsLevyPayer = true, StartingTransferAllowance = 10, TransferAllowance = 20 },
                    new AccountWithBalanceViewModel { AccountHashId = "ABC999", AccountId = 987, AccountName = "Test 2", Balance = 200, IsLevyPayer = true, StartingTransferAllowance = 100, TransferAllowance = 200 }
                }
            });
        }
    }
}
