using Moq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler;

[TestFixture]
public class WhenCallingFindSearchItems : WhenTestingAccountHandler
{
    [Test]
    public async Task ItShouldReturnAnAccountForEachItemInResponseIfFound()
    {
        var accountDetailModels = new List<Core.Models.Account>
        {
            new()
            {
                AccountId = 123,
                OwnerEmail = "owner1@tempuri.org",
                HashedAccountId = "ABC78"
            },
            new()
            {
                AccountId = 124,
                OwnerEmail = "owner2@tempuri.org",
                HashedAccountId = "DEF12"
            }
        };

        MockAccountRepository!.Setup(r => r.FindAllDetails(10,1)).ReturnsAsync(accountDetailModels);

        var actual = (await Unit!.FindAllAccounts(10,1)).ToArray();
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task ItShouldReturnAnEmptyCollectionIfNotFound()
    {
        MockAccountRepository!.Setup(r => r.FindAllDetails(10,1)).ReturnsAsync(new List<Core.Models.Account>());

        var actual = (await Unit!.FindAllAccounts(10,1)).ToArray();
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.Empty);
    }
}