using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldSelectionNone : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturnJustTheAccount()
    {
        // Arrange
        const string hashedAccountId = "ABH3D";
        const long accountId = 44332;
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountDetailViewModel());

        // Act
        var actual = await Sut.Get(hashedAccountId, AccountFieldsSelection.None);
            
        // Assert
        actual.Should().NotBeNull();
        actual.PayeSchemes.Should().BeNull();
        actual.LegalEntities.Should().BeNull();
        actual.TeamMembers.Should().BeNull();
        actual.Transactions.Should().BeNull();
    }

    [Test]
    public async Task ItShouldReturnNullOnException()
    {
        // Arrange
        const string hashedAccountId = "ABH3D";
        const long accountId = 44332;
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);

        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Arrange
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.None);

        // Assert
        Logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        actual.Should().BeNull();
    }
}