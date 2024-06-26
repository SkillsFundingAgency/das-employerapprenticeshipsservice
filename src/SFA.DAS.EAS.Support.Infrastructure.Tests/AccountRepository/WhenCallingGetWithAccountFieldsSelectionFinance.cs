using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldsSelectionFinance : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturntheMatchingAccountWithTransaction()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;

        var accountDetailViewModel = new AccountDetailViewModel
        {
            AccountId = 123,
            Balance = 0m,
            PayeSchemes = new ResourceList(
                new List<ResourceViewModel>
                {
                    new()
                    {
                        Id = "123/123456",
                        Href = "https://tempuri.org/payescheme/{1}"
                    }
                }),
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new()
                    {
                        Id = "TempUri Limited",
                        Href = "https://tempuri.org/organisation/{1}"
                    }
                }),

            HashedAccountId = "DFGH",
            DateRegistered = DateTime.Today.AddYears(-2),
            OwnerEmail = "Owner@tempuri.org",
            DasAccountName = "Test Account 1"
        };
        
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountDetailViewModel);

        var obscuredPayePayeScheme = "123/123456";

        PayeSchemeObfuscator!.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
            .Returns(obscuredPayePayeScheme);


        var payeSchemeViewModel = new PayeSchemeModel
        {
            AddedDate = DateTime.Today.AddMonths(-4),
            Ref = "123/123456",
            Name = "123/123456",
            DasAccountId = "123",
            RemovedDate = null
        };

        EmployerAccountsApiService.Setup(x => x.GetResource<PayeSchemeModel>(It.IsAny<string>()))
            .ReturnsAsync(payeSchemeViewModel);
        
        /* 
         * This is a testing HACK to avoid using a concrete datetime service
         * because our collegues used DateTime.Now in the code! 
         * See ASCS-83 for a fix
         */
        var now = DateTime.Now;
        var yearOffset = now.Month <= 4 ? -1 : 0;
        var startOfFinancialYear = new DateTime(now.Year + yearOffset, 4, 1);

        DatetimeService!.Setup(x => x.GetBeginningFinancialYear(It.IsAny<DateTime>()))
            .Returns(startOfFinancialYear);

        var monthsToQuery = (now.Year - startOfFinancialYear.Year) * 12 +
                            (now.Month - startOfFinancialYear.Month) + 1;
        
        const decimal isNotZero = 100m;
        var isTxDateCreated = DateTime.Today;
        var transactionsViewModel = new TransactionsViewModel
        {
            new()
            {
                Description = "Is Not Null",
                Amount = isNotZero,
                DateCreated = isTxDateCreated
            },
            new()
            {
                Description = "Is Not Null 2",
                Amount = isNotZero,
                DateCreated = isTxDateCreated
            }
        };

        AccountApiClient.Setup(x => x.GetTransactions(accountDetailViewModel.HashedAccountId,
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(transactionsViewModel
            );

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.Finance);

        // Assert
        PayeSchemeObfuscator
            .Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

        AccountApiClient
            .Verify(x => x.GetTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Exactly(monthsToQuery));

        actual.Should().NotBeNull();
        actual.PayeSchemes.Should().HaveCount(1);
        actual.Transactions.Should().HaveCount(2 * monthsToQuery);
        actual.LegalEntities.Should().BeNull();
        actual.TeamMembers.Should().BeNull();
    }

    [Test]
    public async Task ItShouldReturnZeroAccountTransactionsIfTheApiThrowsAnException()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;

        var accountDetailViewModel = new AccountDetailViewModel
        {
            AccountId = 123,
            Balance = 0m,
            PayeSchemes = new ResourceList(
                new List<ResourceViewModel>
                {
                    new()
                    {
                        Id = "123/123456",
                        Href = "https://tempuri.org/payescheme/{1}"
                    }
                }),
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new()
                    {
                        Id = "TempUri Limited",
                        Href = "https://tempuri.org/organisation/{1}"
                    }
                }),

            HashedAccountId = "DFGH",
            DateRegistered = DateTime.Today.AddYears(-2),
            OwnerEmail = "Owner@tempuri.org",
            DasAccountName = "Test Account 1"
        };
        
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountDetailViewModel);

        var obscuredPayePayeScheme = "123/123456";

        PayeSchemeObfuscator!.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
            .Returns(obscuredPayePayeScheme);

        var payeSchemeViewModel = new PayeSchemeModel
        {
            AddedDate = DateTime.Today.AddMonths(-4),
            Ref = "123/123456",
            Name = "123/123456",
            DasAccountId = "123",
            RemovedDate = null
        };

        EmployerAccountsApiService
            .Setup(x => x.GetResource<PayeSchemeModel>(It.IsAny<string>()))
            .ReturnsAsync(payeSchemeViewModel);

        /* 
         * This is a testing HACK to avoid using a concrete datetime service
         * because our collegues used DateTime.Now in the code! 
         * See ASCS-83 for a fix
         */
        var now = DateTime.Now.Date;
        var startOfFirstFinancialYear = new DateTime(2017, 4, 1);
        var monthsToQuery = (now.Year - startOfFirstFinancialYear.Year) * 12 +
                            (now.Month - startOfFirstFinancialYear.Month) + 1;

        DatetimeService!.Setup(x => x.GetBeginningFinancialYear(startOfFirstFinancialYear))
            .Returns(startOfFirstFinancialYear);

        AccountApiClient.Setup(x => x.GetTransactions(accountDetailViewModel.HashedAccountId,
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ThrowsAsync(new Exception("Waaaaaaaah"));

        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.Finance);

        PayeSchemeObfuscator
            .Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

        AccountApiClient
            .Verify(x => x.GetTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Exactly(monthsToQuery));

        actual.Should().NotBeNull();
        actual.PayeSchemes.Should().HaveCount(1);
        actual.Transactions.Should().HaveCount(0);
        actual.LegalEntities.Should().BeNull();
        actual.TeamMembers.Should().BeNull();
    }
}