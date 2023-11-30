using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.ChallengeHandler;

[TestFixture]
public class WhenCallingHandle : WhenTestingChallengeHandler
{
    private const string ValidBalance = "£123";
    private const string InvalidBalance = "£ABC";
    private const string EmptyBalance = "";
    private const string NullBalance = null!;

    private const string ValidElement1 = "1";
    private const string InvalidElement1 = "12";
    private const string EmptyElement1 = "";
    private const string NullElement1 = null!;
    private const string ValidElement2 = "2";
    private const string InvalidElement2 = "12";
    private const string EmptyElement2 = "";
    private const string NullElement2 = null!;

    [TestCase(NullBalance, ValidElement1, ValidElement2)]
    [TestCase(ValidBalance, NullElement1, ValidElement2)]
    [TestCase(ValidBalance, ValidElement1, NullElement2)]
    [TestCase(ValidBalance, ValidElement1, InvalidElement2)]
    [TestCase(ValidBalance, InvalidElement1, ValidElement2)]
    [TestCase(InvalidBalance, ValidElement1, ValidElement2)]
    [TestCase(ValidBalance, ValidElement1, EmptyElement2)]
    [TestCase(ValidBalance, EmptyElement1, ValidElement2)]
    [TestCase(EmptyBalance, ValidElement1, ValidElement2)]
    public async Task ItShouldReturnAnInvalidResponseIfTheQueryIsNotValid(string? balance, string? element1, string? element2)
    {
        var message = new ChallengePermissionQuery
        {
            Balance = balance,
            ChallengeElement1 = element1,
            ChallengeElement2 = element2
        };
        var actual = await Unit!.Handle(message);
        
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.IsValid, Is.EqualTo(false));
    }

    [Test]
    public async Task ItShouldReturnAnInvalidResponseIfAValidQueryHasNoMatch()
    {
        var message = new ChallengePermissionQuery
        {
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "2"
        };
        AccountRepository!
            .Setup(x =>x.Get(message.Id, AccountFieldsSelection.PayeSchemes))
            .ReturnsAsync(null as Core.Models.Account);

        var actual = await Unit!.Handle(message);
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.IsValid, Is.EqualTo(false));
    }

    [Test]
    public async Task ItShouldReturnAnInvalidResponseWhenThereIsAMatchThatHasInvalidData()
    {
        var message = new ChallengePermissionQuery
        {
            Id = "123",
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "2"
        };
        var account = new Core.Models.Account
        {
            HashedAccountId = "ASDAS",
            AccountId = 123
        };
      
        AccountRepository!
            .Setup(x =>x.Get(message.Id, AccountFieldsSelection.PayeSchemes))
            .ReturnsAsync(account);

        ChallengeRepository!.Setup(x => x.CheckData(account, message)).ReturnsAsync(false);

        var actual = await Unit!.Handle(message);

        ChallengeRepository.Verify(x => x.CheckData(account, message));

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.IsValid, Is.EqualTo(false));
    }

    [Test]
    public async Task ItShouldReturnAnValidResponseWhenThereIsAMatchThatHasValidData()
    {
        var message = new ChallengePermissionQuery
        {
            Id = "123",
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "2"
        };
        var account = new Core.Models.Account
        {
            HashedAccountId = "ASDAS",
            AccountId = 123
        };
        
        AccountRepository!
            .Setup(x =>x.Get(message.Id, AccountFieldsSelection.PayeSchemes))
            .ReturnsAsync(account);

        ChallengeRepository!.Setup(x => x.CheckData(account, message)).ReturnsAsync(true);

        var actual = await Unit!.Handle(message);

        ChallengeRepository.Verify(x => x.CheckData(account, message));

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.IsValid, Is.EqualTo(true));
    }
}