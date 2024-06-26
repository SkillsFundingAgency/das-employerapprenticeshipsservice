using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Encoding;
using ResourceList = SFA.DAS.EAS.Account.Api.Types.ResourceList;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldSelectionPayeSchemes : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturnNullOnException()
    {
        const string hashedAccountId = "ABH3D";
        const long accountId = 44332;
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var actual = await Sut.Get(hashedAccountId, AccountFieldsSelection.PayeSchemes);

        // Assert
        Logger!.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        actual.Should().BeNull();
    }

    [Test]
    public async Task ItShouldReturnTheAccountWithPayeSchemes()
    {
        // Arrange
        const string hashedAccountId = "ABH3D";
        const long accountId = 44332;
        const string payeRef = "123/123456";
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);

        var accountDetailViewModel = new AccountDetailViewModel
        {
            AccountId = 123,
            Balance = 0m,
            PayeSchemes = new ResourceList(
                new List<ResourceViewModel>
                {
                    new ResourceViewModel
                    {
                        Id = payeRef,
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

        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountDetailViewModel);

        const string obscuredPayePayeScheme = "1**/****6";

        PayeSchemeObfuscator!.Setup(x => x.ObscurePayeScheme(payeRef))
            .Returns(obscuredPayePayeScheme)
            .Verifiable(Times.Exactly(2));

        var payeSchemeViewModel = new PayeSchemeModel
        {
            AddedDate = DateTime.Today.AddMonths(-4),
            Ref = payeRef,
            Name = "123/123456",
            DasAccountId = "123",
            RemovedDate = null
        };

        EmployerAccountsApiService
            .Setup(x => x.GetResource<PayeSchemeModel>(It.IsAny<string>()))
            .ReturnsAsync(payeSchemeViewModel);

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.PayeSchemes);

        // Assert
        PayeSchemeObfuscator.Verify();
        actual.Should().NotBeNull();
        actual.PayeSchemes.Should().NotBeNull();
        actual.PayeSchemes.Count().Should().Be(1);
        actual.LegalEntities.Should().BeNull();
        actual.TeamMembers.Should().BeNull();
        actual.Transactions.Should().BeNull();
    }
}