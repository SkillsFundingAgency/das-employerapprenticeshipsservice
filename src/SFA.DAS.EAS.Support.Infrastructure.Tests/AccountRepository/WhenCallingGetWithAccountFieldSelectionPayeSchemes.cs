using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using ResourceList = SFA.DAS.EAS.Account.Api.Types.ResourceList;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldSelectionPayeSchemes : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturnNullOnException()
    {
        const string id = "123";

        AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
            .ThrowsAsync(new Exception());

        var actual = await Sut!.Get(id, AccountFieldsSelection.PayeSchemes);

        Logger!.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        Assert.That(actual, Is.Null);
    }

    [Test]
    public async Task ItShouldReturnTheAccountWithPayeSchemes()
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

        var actual = await Sut!.Get(id, AccountFieldsSelection.PayeSchemes);

        PayeSchemeObsfuscator.Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.PayeSchemes, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.PayeSchemes.Count(), Is.EqualTo(1));
            Assert.That(actual.LegalEntities, Is.Null);
            Assert.That(actual.TeamMembers, Is.Null);
            Assert.That(actual.Transactions, Is.Null);
        });
    }
}