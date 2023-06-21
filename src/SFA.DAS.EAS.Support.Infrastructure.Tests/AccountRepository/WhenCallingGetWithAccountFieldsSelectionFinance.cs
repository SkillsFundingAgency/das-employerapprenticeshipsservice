using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldsSelectionFinance : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturntheMatchingAccountWithTransaction()
    {
        const string id = "123";

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

        AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
            .ReturnsAsync(accountDetailViewModel);

        var obscuredPayePayeScheme = "123/123456";

        PayeSchemeObsfuscator!.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
            .Returns(obscuredPayePayeScheme);


        var payeSchemeViewModel = new PayeSchemeModel
        {
            AddedDate = DateTime.Today.AddMonths(-4),
            Ref = "123/123456",
            Name = "123/123456",
            DasAccountId = "123",
            RemovedDate = null
        };

        AccountApiClient.Setup(x => x.GetResource<PayeSchemeModel>(It.IsAny<string>()))
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

        // Q: 2016,4,1 -> 2016,11,1    
        // A: (2016 - 2016 = 1 ) * 12  = 0
        //    + 11 - 4 + 1             = 8 (4,5,6,7,8,9,10,11)

        // Q: 2016,4,1 -> 2017,3,31    
        // A: 2017 - 2016 = 1 * 12     = 12
        //    3 - 4 = -1 + 1           = 0
        //                             = 12 (4,5,6,7,8,9,10,11,12,1,2,3)

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

        var actual = await Sut!.Get(id, AccountFieldsSelection.Finance);

        PayeSchemeObsfuscator
            .Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

        AccountApiClient
            .Verify(x => x.GetTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Exactly(monthsToQuery));

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.PayeSchemes, Is.Not.Null);
            Assert.That(actual.PayeSchemes.Count(), Is.EqualTo(1));
            Assert.That(actual.Transactions, Is.Not.Null);
            Assert.That(actual.Transactions.Count(), Is.EqualTo(2 * monthsToQuery));
            Assert.That(actual.LegalEntities, Is.Null);
            Assert.That(actual.TeamMembers, Is.Null);
        });
    }

    [Test]
    public async Task ItShouldReturnZeroAccountTransactionsIfTheApiThrowsAnException()
    {
        const string id = "123";

        var accountDetailViewModel = new AccountDetailViewModel
        {
            AccountId = 123,
            Balance = 0m,
            PayeSchemes = new ResourceList(
                new List<ResourceViewModel>
                {
                    new ResourceViewModel
                    {
                        Id = "123/123456",
                        Href = "https://tempuri.org/payescheme/{1}"
                    }
                }),
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new ResourceViewModel
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

        AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
            .ReturnsAsync(accountDetailViewModel);

        const string obscuredPayePayeScheme = "123/123456";

        PayeSchemeObsfuscator!.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
            .Returns(obscuredPayePayeScheme);

        var payeSchemeViewModel = new PayeSchemeModel
        {
            AddedDate = DateTime.Today.AddMonths(-4),
            Ref = "123/123456",
            Name = "123/123456",
            DasAccountId = "123",
            RemovedDate = null
        };

        AccountApiClient.Setup(x => x.GetResource<PayeSchemeModel>(It.IsAny<string>()))
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

        var actual = await Sut!.Get(id, AccountFieldsSelection.Finance);

        PayeSchemeObsfuscator
            .Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

        AccountApiClient
            .Verify(x => x.GetTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Exactly(monthsToQuery));

        Assert.Multiple(() =>
        {
            Assert.That(actual.PayeSchemes.Count(), Is.EqualTo(1));
            Assert.That(actual.Transactions, Is.Not.Null);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.PayeSchemes, Is.Not.Null);
            Assert.That(actual.Transactions.Count(), Is.EqualTo(0));
            Assert.That(actual.LegalEntities, Is.Null);
            Assert.That(actual.TeamMembers, Is.Null);
        });
    }
}