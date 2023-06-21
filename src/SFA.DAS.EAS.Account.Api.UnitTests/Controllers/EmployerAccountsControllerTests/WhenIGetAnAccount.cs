using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests;

[TestFixture]
public class WhenIGetAnAccount : EmployerAccountsControllerTests
{
    [Test]
    public async Task ThenTheAccountWithBalanceIsReturned()
    {
        //Arrange
        const string hashedAccountId = "ABC123";
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
        EmployerAccountsApiService!.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(account);


        var accountBalancesResponse = new List<AccountBalance> { new AccountBalance { AccountId = account.AccountId, Balance = 123.45m } };
        EmployerFinanceApiService!.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), CancellationToken.None)).ReturnsAsync(accountBalancesResponse);

        var transferAllowanceResponse = new TransferAllowance() { StartingTransferAllowance = 10, RemainingTransferAllowance = 15 };            
        EmployerFinanceApiService.Setup(x => x.GetTransferAllowance(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(transferAllowanceResponse);

        //Act
        var response = await Controller!.GetAccount(hashedAccountId);

        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<AccountDetailViewModel>>());
        var model = response as ActionResult<AccountDetailViewModel>;

        Assert.That(model.Result, Is.Not.Null);
        Assert.That(model.Result, Is.InstanceOf<OkObjectResult>());

        var oKResult = model.Result as OkObjectResult;
        
        Assert.Multiple(() =>
        {
            Assert.That(oKResult!.Value, Is.Not.Null);
            Assert.That(oKResult.Value, Is.InstanceOf<AccountDetailViewModel>());
        });
        var value = oKResult!.Value as AccountDetailViewModel;

        value.Should().NotBeNull();
        value!.DasAccountId.Should().Be(hashedAccountId);
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
        const string hashedAccountId = "ABC123";
           
        //Act
        EmployerAccountsApiService!.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = 0 });
        var response = await Controller!.GetAccount(hashedAccountId);

        //Assert
        Assert.That(response.Result, Is.Not.Null);
        Assert.That(response.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task ThenIAmAbleToGetAnAccountByTheInternalId()
    {
        //Arrange
        const int accountId = 1923701937;
        const string hashedAccountId = "ABC123";
            
        EncodingService!.Setup(x => x.Encode(accountId, EncodingType.AccountId)).Returns(hashedAccountId);
        EmployerAccountsApiService!.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = accountId, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString(), LegalEntities = new ResourceList(new List<ResourceViewModel>()), PayeSchemes = new ResourceList(new List<ResourceViewModel>()) });
        EmployerFinanceApiService!.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), CancellationToken.None)).ReturnsAsync( new List<AccountBalance> { new AccountBalance() });
        EmployerFinanceApiService.Setup(x => x.GetTransferAllowance(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync( new TransferAllowance());

        //Act
        var response = await Controller!.GetAccount(accountId);

        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<OkObjectResult>());

        var okResult = (OkObjectResult)response;

        Assert.That(okResult.Value, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<AccountDetailViewModel>());
    }
}