﻿using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    [TestFixture]
    public class WhenIGetAnAccount : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenTheAccountWithBalanceIsReturned()
        {
            var hashedAccountId = "ABC123";
            var account = new AccountDetailViewModel
            {
                HashedAccountId = hashedAccountId,
                AccountId = 123,
                DateRegistered = DateTime.Now.AddYears(-1),
                OwnerEmail = "test@email.com",
                DasAccountName = "Test",
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString(),
                LegalEntities = new ResourceList(new List<ResourceViewModel> { new ResourceViewModel { Href = "/api/123", Id = "123" } }),
                PayeSchemes = new ResourceList(new List<ResourceViewModel> { new ResourceViewModel { Href = "/api/XXX", Id = "XXX" } })
            };

            var accountBalanceResponse = new GetAccountBalancesResponse
            {
                Accounts = new List<AccountBalance> { new AccountBalance { AccountId = account.AccountId, Balance = 123.45m } }
            };

            ApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(account);
            Mediator.Setup(x => x.SendAsync(It.Is<GetAccountBalancesRequest>(q => q.AccountIds.Single() == account.AccountId))).ReturnsAsync(accountBalanceResponse);
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>())).ReturnsAsync(new GetTransferAllowanceResponse { TransferAllowance = new TransferAllowance() });
         
            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetailViewModel>>(response);
            var model = response as OkNegotiatedContentResult<AccountDetailViewModel>;

            model?.Content.Should().NotBeNull();
            model?.Content?.DasAccountId.Should().Be(hashedAccountId);
            model?.Content?.HashedAccountId.Should().Be(hashedAccountId);
            model?.Content?.AccountId.Should().Be(account.AccountId);
            model?.Content?.DasAccountName.Should().Be(account.DasAccountName);
            model?.Content?.DateRegistered.Should().Be(account.DateRegistered);
            model?.Content?.OwnerEmail.Should().Be(account.OwnerEmail);          
            model?.Content?.Balance.Should().Be(accountBalanceResponse.Accounts.Single().Balance);
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
           
            ApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = 0 });
            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task ThenIAmAbleToGetAnAccountByTheInternalId()
        {
            //Arrange
            var accountId = 1923701937;
            var hashedAccountId = "ABC123";
            
            HashingService.Setup(x => x.HashValue(accountId)).Returns(hashedAccountId);
            ApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = accountId, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString(), LegalEntities = new ResourceList(new List<ResourceViewModel>()), PayeSchemes = new ResourceList(new List<ResourceViewModel>()) });
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(new GetAccountBalancesResponse { Accounts = new List<AccountBalance> { new AccountBalance() } });
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>())).ReturnsAsync(new GetTransferAllowanceResponse { TransferAllowance = new TransferAllowance() });

            //Act
            var response = await Controller.GetAccount(accountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetailViewModel>>(response);
        }
    }
}
