using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.ChallengeRepository;

[TestFixture]
public class WhenTestingChallengeRepository
{
    [SetUp]
    public void Setup()
    {
        _financeRepository = new Mock<IFinanceRepository>();
        _unit = new Services.ChallengeRepository(_financeRepository.Object, Mock.Of<ILogger<Services.ChallengeRepository>>());
    }

    private Services.ChallengeRepository? _unit;
    private Mock<IFinanceRepository>? _financeRepository;

    [Test]
    public async Task ItShouldReturnFalseWhenCheckDataHasIncorrectBalance()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() { Balance = 300m },
                new() { Balance = 700m }
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
            Id = Guid.NewGuid().ToString(),
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "4",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 3
        };

        const decimal balance = 999m;

        _financeRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        actual.Should().BeFalse();
    }

    [Test]
    public async Task ItShouldReturnFalseWhenCheckDataHasInvalidBalance()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() { Balance = 300m },
                new() { Balance = 700m }
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
            Id = Guid.NewGuid().ToString(),
            Balance = "£Z000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "A",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 4
        };

        const decimal balance = 1000m;

        _financeRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        actual.Should().BeFalse();
    }

    [Test]
    public async Task ItShouldReturnFalseWhenCheckDataHasInvalidCharacterData()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() { Balance = 300m },
                new() { Balance = 700m }
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
            Id = Guid.NewGuid().ToString(),
            Balance = "£1000",
            ChallengeElement1 = "1",
            ChallengeElement2 = "A",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 1
        };

        const decimal balance = 1000m;

        _financeRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        actual.Should().BeFalse();
    }

    [Test]
    public async Task ItShouldReturnTrueWhenCheckDataHasValidData()
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() { Balance = 300m },
                new() { Balance = 700m }
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

        _financeRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(balance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        actual.Should().BeTrue();
    }

    [TestCase(593940.60)]
    [TestCase(593940.10)]
    [TestCase(593940.00)]
    public async Task ItShouldReturnTrueWhenCheckDataHasValidData(decimal accountBalance)
    {
        var account = new Core.Models.Account
        {
            Transactions = new List<TransactionViewModel>
            {
                new() { Balance = 300m },
                new() { Balance = 700m }
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
            // Employer Finance UI current balance formats the value as below
            Balance = accountBalance.ToString("C0", new CultureInfo("en-GB")),
            ChallengeElement1 = "1",
            ChallengeElement2 = "A",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 5
        };

        _financeRepository!.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
            .ReturnsAsync(accountBalance);

        var actual = await _unit!.CheckData(account, challengePermissionQuery);

        actual.Should().BeTrue();
    }
}