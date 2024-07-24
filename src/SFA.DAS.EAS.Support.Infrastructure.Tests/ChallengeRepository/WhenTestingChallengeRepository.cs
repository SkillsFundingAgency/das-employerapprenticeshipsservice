using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.ChallengeRepository;

[TestFixture]
public class WhenTestingChallengeRepository
{
    [SetUp]
    public void Setup()
    {
        _accountRepository = new Mock<IAccountRepository>();
        _unit = new Services.ChallengeRepository(_accountRepository.Object);
    }

    private Services.ChallengeRepository? _unit;
    private Mock<IAccountRepository>? _accountRepository;

    [Test]
    public async Task ItShouldReturnFalseWhenCheckDataHasIncorrectBalance()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() {Balance = 300m},
                new() {Balance = 700m}
            },
            PayeSchemes = new List<PayeSchemeModel>
            {
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "123/456789"
                },
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "124/456789"
                }
            }
        };
        var challengePermissionQuery = new ChallengePermissionQuery
        {
            Id = "123",
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "4",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 3
        };

        const decimal balance = 999m;

        _accountRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task ItShouldReturnFalseWhenCheckDataHasInvalidBalance()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() {Balance = 300m},
                new() {Balance = 700m}
            },
            PayeSchemes = new List<PayeSchemeModel>
            {
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "123/456789"
                },
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "124/AA6789"
                }
            }
        };
        var challengePermissionQuery = new ChallengePermissionQuery
        {
            Id = "123",
            Balance = "£Z000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "A",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 4
        };

        const decimal balance = 1000m;

        _accountRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task ItShouldReturnFalseWhenCheckDataHasInvalidCharacterData()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() {Balance = 300m},
                new() {Balance = 700m}
            },
            PayeSchemes = new List<PayeSchemeModel>
            {
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "123/456789"
                },
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "124/456789"
                }
            }
        };
            
        var challengePermissionQuery = new ChallengePermissionQuery
        {
            Id = "123",
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "A",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 1
        };

        const decimal balance = 1000m;

        _accountRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task ItShouldReturnTrueWhenCheckDataHasValidData()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() {Balance = 300m},
                new() {Balance = 700m}
            },
            PayeSchemes = new List<PayeSchemeModel>
            {
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "123/456789"
                },
                new()
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Ref = "124/45A789"
                }
            }
        };
        var challengePermissionQuery = new ChallengePermissionQuery
        {
            Id = "123",
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "A",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 5
        };

        const decimal balance = 1000m;

        _accountRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        Assert.That(actual, Is.True);
    }
}