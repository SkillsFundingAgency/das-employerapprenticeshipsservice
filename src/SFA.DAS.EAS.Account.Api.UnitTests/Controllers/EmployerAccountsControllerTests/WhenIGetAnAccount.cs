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
        const long accountId = 88234;
        const string hashedAccountId = "ABC123";
        var account = new AccountDetailViewModel
        {
            AccountId = accountId,
            HashedAccountId = hashedAccountId,
            DateRegistered = DateTime.Now.AddYears(-1),
            OwnerEmail = "test@email.com",
            DasAccountName = "Test",
            ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString(),
            LegalEntities = new ResourceList(new List<ResourceViewModel> { new() { Href = "/api/123", Id = "123" } }),
            PayeSchemes = new ResourceList(new List<ResourceViewModel> { new() { Href = "/api/XXX", Id = "XXX" } })
        };
        EmployerAccountsApiService!.Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        EncodingService.Setup(x => x.Encode(accountId, EncodingType.AccountId)).Returns(hashedAccountId);
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);

        var accountBalancesResponse = new List<AccountBalance> { new() { AccountId = account.AccountId, Balance = 123.45m } };
        EmployerFinanceApiService!
            .Setup(x => x.GetAccountBalances(It.Is<List<string>>(x => x.Contains(hashedAccountId)), CancellationToken.None))
            .ReturnsAsync(accountBalancesResponse);

        var transferAllowanceResponse = new TransferAllowance { StartingTransferAllowance = 10, RemainingTransferAllowance = 15 };            
        EmployerFinanceApiService
            .Setup(x => x.GetTransferAllowance(hashedAccountId, CancellationToken.None))
            .ReturnsAsync(transferAllowanceResponse);

        //Act
        var response = await Controller!.GetAccount(hashedAccountId);

        //Assert
        var result = response.Result.Should().BeAssignableTo<OkObjectResult>();
        var model = result.Subject.Value.Should().BeAssignableTo<AccountDetailViewModel>();
        var value = model.Subject;
        
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
        const long accountId = 88235;
           
        //Act
        EmployerAccountsApiService!.Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = 0 });
        var response = await Controller!.GetAccount(accountId);

        //Assert
        response.Should().BeAssignableTo<NotFoundResult>();
    }

    [Test]
    public async Task ThenIAmAbleToGetAnAccountByTheInternalId()
    {
        //Arrange
        const int accountId = 1923701937;
        const string hashedAccountId = "YH757J";

        EncodingService.Setup(x => x.Encode(accountId, EncodingType.AccountId)).Returns(hashedAccountId);
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService!.Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel { AccountId = accountId, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString(), LegalEntities = new ResourceList(new List<ResourceViewModel>()), PayeSchemes = new ResourceList(new List<ResourceViewModel>()) });
        EmployerFinanceApiService!.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), CancellationToken.None)).ReturnsAsync( new List<AccountBalance> { new AccountBalance() });
        EmployerFinanceApiService.Setup(x => x.GetTransferAllowance(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync( new TransferAllowance());

        //Act
        var response = await Controller!.GetAccount(accountId);

        //Assert
        var okResponse = response.Should().BeAssignableTo<OkObjectResult>();
        okResponse.Subject.Value.Should().BeAssignableTo<AccountDetailViewModel>();
    }
}