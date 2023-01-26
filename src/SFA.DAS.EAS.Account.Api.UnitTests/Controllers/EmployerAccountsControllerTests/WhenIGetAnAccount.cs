using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            //Arrange
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
            _employerAccountsApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(account);


            var accountBalancesResponse = new List<AccountBalance> { new AccountBalance { AccountId = account.AccountId, Balance = 123.45m } };
            _employerFinanceApiService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>())).ReturnsAsync(accountBalancesResponse);

            var transferAllowanceResponse = new TransferAllowance() { StartingTransferAllowance = 10, RemainingTransferAllowance = 15 };            
            _employerFinanceApiService.Setup(x => x.GetTransferAllowance(It.IsAny<string>())).ReturnsAsync(transferAllowanceResponse);

            //Act
            var response = await _controller.GetAccount(hashedAccountId);

            //Assert
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
            model?.Content?.Balance.Should().Be(accountBalancesResponse.Single().Balance);
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";
           
            //Act
            _employerAccountsApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = 0 });
            var response = await _controller.GetAccount(hashedAccountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task ThenIAmAbleToGetAnAccountByTheInternalId()
        {
            //Arrange
            var accountId = 1923701937;
            var hashedAccountId = "ABC123";
            
            _hashingService.Setup(x => x.HashValue(accountId)).Returns(hashedAccountId);
            _employerAccountsApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = accountId, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString(), LegalEntities = new ResourceList(new List<ResourceViewModel>()), PayeSchemes = new ResourceList(new List<ResourceViewModel>()) });
            _employerFinanceApiService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>())).ReturnsAsync( new List<AccountBalance> { new AccountBalance() });
            _employerFinanceApiService.Setup(x => x.GetTransferAllowance(It.IsAny<string>())).ReturnsAsync( new TransferAllowance());

            //Act
            var response = await _controller.GetAccount(accountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetailViewModel>>(response);
        }
    }
}
