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
using Microsoft.AspNetCore.Mvc;

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
            Assert.IsInstanceOf<ActionResult<AccountDetailViewModel>>(response);
            var model = response as ActionResult<AccountDetailViewModel>;

            Assert.IsNotNull(model.Result);
            Assert.IsInstanceOf<OkObjectResult>(model.Result);

            var oKResult = model.Result as OkObjectResult;

            Assert.IsNotNull(oKResult.Value);
            Assert.IsInstanceOf<AccountDetailViewModel>(oKResult.Value);

            AccountDetailViewModel value = oKResult.Value as AccountDetailViewModel;

            value.Should().NotBeNull();
            value.DasAccountId.Should().Be(hashedAccountId);
            value.HashedAccountId.Should().Be(hashedAccountId);
            value.AccountId.Should().Be(account.AccountId);
            value.DasAccountName.Should().Be(account.DasAccountName);
            value.DateRegistered.Should().Be(account.DateRegistered);
            value.OwnerEmail.Should().Be(account.OwnerEmail);          
            value.Balance.Should().Be(accountBalancesResponse.Single().Balance);
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
            Assert.IsNotNull(response.Result);
            Assert.IsInstanceOf<NotFoundResult>(response.Result);
        }

        [Test]
        public async Task ThenIAmAbleToGetAnAccountByTheInternalId()
        {
            //Arrange
            var accountId = 1923701937;
            var hashedAccountId = "ABC123";
            
            _encodingService.Setup(x => x.Encode(accountId, Encoding.EncodingType.AccountId)).Returns(hashedAccountId);
            _employerAccountsApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = accountId, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString(), LegalEntities = new ResourceList(new List<ResourceViewModel>()), PayeSchemes = new ResourceList(new List<ResourceViewModel>()) });
            _employerFinanceApiService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>())).ReturnsAsync( new List<AccountBalance> { new AccountBalance() });
            _employerFinanceApiService.Setup(x => x.GetTransferAllowance(It.IsAny<string>())).ReturnsAsync( new TransferAllowance());

            //Act
            var response = await _controller.GetAccount(accountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var okResult = (OkObjectResult)response;

            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<AccountDetailViewModel>(okResult.Value);
        }
    }
}
