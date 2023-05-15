using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.ChallengeHandler;

[TestFixture]
public class WhenCallingGet : WhenTestingChallengeHandler
{
    [Test]
    public async Task ItShouldReturnAnAccountAndSuccessWhenQueryHasAMatch()
    {
        const string id = "123";
        var account = new Core.Models.Account
        {
            HashedAccountId = "ASDAS",
            AccountId = 123
        };
        AccountRepository.Setup(x =>
                x.Get(id,
                    AccountFieldsSelection.PayeSchemes))
            .ReturnsAsync(account);

        var actual = await Unit.Get(id);


        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Account, Is.Not.Null);
            Assert.That(actual.StatusCode, Is.EqualTo(SearchResponseCodes.Success));
        });
    }

    [Test]
    public async Task ItShouldReturnNoSearchResultsFoundWhenQueryHasNoMatch()
    {
        const string id = "123";
        AccountRepository.Setup(x =>
                x.Get(id,
                    AccountFieldsSelection.PayeSchemes))
            .ReturnsAsync(null as Core.Models.Account);

        var actual = await Unit.Get(id);

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.StatusCode, Is.EqualTo(SearchResponseCodes.NoSearchResultsFound));
    }
}