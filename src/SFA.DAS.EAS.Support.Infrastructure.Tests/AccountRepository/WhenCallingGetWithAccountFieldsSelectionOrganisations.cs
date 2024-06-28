using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldsSelectionOrganisations : WhenTestingAccountRepository
{
    [TestCase(EmployerAgreementStatus.Signed)]
    [TestCase(EmployerAgreementStatus.Pending)]
    [TestCase(EmployerAgreementStatus.Superseded)]
    public async Task ItShouldReturnTheMatchingAccountWithLegalEntitiesThatAreInScope(EmployerAgreementStatus scope)
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;
        
        var accountResponse = new AccountDetailViewModel
        {
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new() { Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC" }
                })
        };

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountResponse)
            .Verifiable();

        var legalEntityResponse = new LegalEntityViewModel
        {
            AgreementStatus = scope
        };

        var legalEntity = accountResponse.LegalEntities[0];

        EmployerAccountsApiService
            .Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
            .ReturnsAsync(legalEntityResponse)
            .Verifiable();

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.Organisations);

        // Assert
        EmployerAccountsApiService.Verify();
        EmployerAccountsApiService.VerifyNoOtherCalls();

        actual.Should().NotBeNull();
        actual.LegalEntities.Should().HaveCount(accountResponse.LegalEntities.Count);
        actual.PayeSchemes.Should().BeNull();
        actual.Transactions.Should().BeNull();
        actual.TeamMembers.Should().BeNull();
    }

    [TestCase(EmployerAgreementStatus.Expired)]
    [TestCase(EmployerAgreementStatus.Removed)]
    public async Task ItShouldReturnTheMatchingAccountWithOutLegalEntitiesThatAreOutOfScope(
        EmployerAgreementStatus scope)
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;
        
        var accountResponse = new AccountDetailViewModel
        {
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new() { Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC" }
                })
        };

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountResponse)
            .Verifiable();
        
        var legalEntityResponse = new LegalEntityViewModel
        {
            AgreementStatus = scope
        };

        var legalEntity = accountResponse.LegalEntities[0];
        
        EmployerAccountsApiService
            .Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
            .ReturnsAsync(legalEntityResponse)
            .Verifiable();

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.Organisations);
        
        // Assert
        EmployerAccountsApiService.Verify();
        EmployerAccountsApiService.VerifyNoOtherCalls();

        actual.LegalEntities.Should().HaveCount(0);
        actual.Should().NotBeNull();
        actual.PayeSchemes.Should().BeNull();
        actual.Transactions.Should().BeNull();
        actual.TeamMembers.Should().BeNull();
    }

    [Test]
    public async Task ItShoudFailGracefullyAndLogErrorWhenClientThrowsExceptionOnGetResourceAccount()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;

        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception())
            .Verifiable();

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.Organisations);

        // Assert
        Logger!.Verify(
            x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        EmployerAccountsApiService.Verify();
        EmployerAccountsApiService.VerifyNoOtherCalls();
        actual.Should().BeNull();
    }

    [Test]
    public async Task ItShoudFailGracefullyAndLogErrorWhenClientThrowsExceptionOnGetResourceLegalEntity()
    {
        // Arrange
        const string hashedAccountId = "123";
        const long accountId = 222;

        var accountResponse = new AccountDetailViewModel
        {
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new() { Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC" }
                })
        };
        
        EncodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountResponse)
            .Verifiable();
        
        var legalEntity = accountResponse.LegalEntities[0];

        EmployerAccountsApiService.Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
            .ThrowsAsync(new Exception());

        // Act
        var actual = await Sut!.Get(hashedAccountId, AccountFieldsSelection.Organisations);
        
        // Assert
        Logger.Verify(
            x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        
        EmployerAccountsApiService.Verify();
        EmployerAccountsApiService.Verify(x => x.GetResource<LegalEntityViewModel>(It.IsAny<string>()),
            Times.Exactly(accountResponse.LegalEntities.Count));
        EmployerAccountsApiService.VerifyNoOtherCalls();
        
        actual.Should().NotBeNull();
        actual.LegalEntities.Should().BeEmpty();
    }
}