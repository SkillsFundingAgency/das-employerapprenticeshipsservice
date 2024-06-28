using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldsSelectionChallengePayeSchemes : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturnTheAccountWithTheChallengedPayeSchemes()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;
        
        var accountDetailViewModel = new AccountDetailViewModel
        {
            AccountId = 123,
            Balance = 0m,
            PayeSchemes = new ResourceList(
                new List<ResourceViewModel>
                {
                    new()
                    {
                        Id = "123/123456",
                        Href = "https://tempuri.org/payescheme/{1}"
                    }
                }),
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new()
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

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountDetailViewModel);
        
        var obscuredPayePayeScheme = "123/123456";

        PayeSchemeObfuscator!.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
            .Returns(obscuredPayePayeScheme);
        
        var payeSchemeViewModel = new PayeSchemeModel
        {
            AddedDate = DateTime.Today.AddMonths(-4),
            Ref = "123/123456",
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
        PayeSchemeObfuscator.Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

        actual.Should().NotBeNull();
        actual.PayeSchemes.Should().HaveCount(1);
        actual.LegalEntities.Should().BeNull();
        actual.TeamMembers.Should().BeNull();
        actual.Transactions.Should().BeNull();
    }
}