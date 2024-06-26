using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldsSelectionTeamMembers : WhenTestingAccountRepository
{
    [Test]
    public async Task
        ItShouldReturnTheMatchingAccountWithAnEmptyListOfTeamMembersWhenAnExceptionIsThrownObtainingTheTeeamMembers()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;
        
        var accountDetailViewModel = new AccountDetailViewModel
        {
            HashedAccountId = "ASDAS",
            AccountId = accountId
        };

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountDetailViewModel)
            .Verifiable();

        EmployerAccountsApiService.Setup(x => x.GetAccountUsers(accountId))
            .ThrowsAsync(new Exception("Some Exception"));

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.TeamMembers);
        
        // Assert
        actual.Should().NotBeNull();
        actual.TeamMembers.Should().HaveCount(0);
        actual.Transactions.Should().BeNull();
        actual.PayeSchemes.Should().BeNull();
        actual.LegalEntities.Should().BeNull();
    }

    [Test]
    public async Task ItShouldReturnTheMatchingAccountWithTeamMembers()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;
        
        var accountDetailViewModel = new AccountDetailViewModel
        {
            HashedAccountId = "ASDAS",
            AccountId = accountId
        };

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountDetailViewModel)
            .Verifiable();

        var teamMemberResponse = new List<TeamMemberViewModel>
        {
            new() { Email = "member.1.@tempuri.org" },
            new() { Email = "member.1.@tempuri.org" }
        };

        EmployerAccountsApiService
            .Setup(x => x.GetAccountUsers(accountId))
            .ReturnsAsync(teamMemberResponse);

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.TeamMembers);
        
        // Assert
        actual.Should().NotBeNull();
        actual.TeamMembers.Should().BeEquivalentTo(teamMemberResponse);
        actual.Transactions.Should().BeNull();
        actual.PayeSchemes.Should().BeNull();
        actual.LegalEntities.Should().BeNull();
    }
}