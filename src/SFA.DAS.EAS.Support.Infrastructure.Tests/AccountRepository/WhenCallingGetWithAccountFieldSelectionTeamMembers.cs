using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldSelectionTeamMembers : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturnTheAccountAndTheTeamMembers()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountDetailViewModel());

        var fixture = new Fixture();
        EmployerAccountsApiService.Setup(x => x.GetAccountUsers(It.IsAny<long>())).ReturnsAsync(new List<TeamMemberViewModel>()
        {
            fixture.Create<TeamMemberViewModel>(),
            fixture.Create<TeamMemberViewModel>()
        });

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.TeamMembers);

        // Assert
        actual.Should().NotBeNull();
        actual.PayeSchemes.Should().BeNull();
        actual.LegalEntities.Should().BeNull();
        actual.TeamMembers.Should().NotBeEmpty();
        actual.Transactions.Should().BeNull();
    }

    [Test]
    public async Task ItShouldReturnTheAccountWithEmptyTeamMembersOnException()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.TeamMembers);

        // Assert
        Logger!.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        Assert.That(actual, Is.Null);
    }
}