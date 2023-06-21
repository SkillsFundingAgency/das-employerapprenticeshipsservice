using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetWithAccountFieldsSelectionOrganisations : WhenTestingAccountRepository
{
    [TestCase(EmployerAgreementStatus.Signed)]
    [TestCase(EmployerAgreementStatus.Pending)]
    [TestCase(EmployerAgreementStatus.Superseded)]
    public async Task ItShouldReturnTheMatchingAccountWithLegalEntitiesThatAreInScope(EmployerAgreementStatus scope)
    {
        const string id = "123";

        var accountResponse = new AccountDetailViewModel
        {
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new ResourceViewModel { Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC" }
                })
        };

        AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
            .ReturnsAsync(accountResponse);

        var legalEntityResponse = new LegalEntityViewModel
        {
            AgreementStatus = scope
        };

        var legalEntity = accountResponse.LegalEntities[0];

        AccountApiClient.Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
            .ReturnsAsync(legalEntityResponse);

        var actual = await Sut!.Get(id, AccountFieldsSelection.Organisations);

        AccountApiClient.Verify(x => x.GetResource<LegalEntityViewModel>(It.IsAny<string>()),
            Times.Exactly(accountResponse.LegalEntities.Count));

        Assert.Multiple(() =>
        {
            Assert.That(actual.LegalEntities.Count(), Is.EqualTo(accountResponse.LegalEntities.Count));
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.LegalEntities, Is.Not.Null);
            Assert.That(actual.PayeSchemes, Is.Null);
            Assert.That(actual.Transactions, Is.Null);
            Assert.That(actual.TeamMembers, Is.Null);
        });
    }

    [TestCase(EmployerAgreementStatus.Expired)]
    [TestCase(EmployerAgreementStatus.Removed)]
    public async Task ItShouldReturnTheMatchingAccountWithOutLegalEntitiesThatAreOutOfScope(
        EmployerAgreementStatus scope)
    {
        const string id = "123";

        var accountResponse = new AccountDetailViewModel
        {
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new() { Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC" }
                })
        };

        AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
            .ReturnsAsync(accountResponse);

        var legalEntityResponse = new LegalEntityViewModel
        {
            AgreementStatus = scope
        };

        var legalEntity = accountResponse.LegalEntities[0];

        AccountApiClient.Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
            .ReturnsAsync(legalEntityResponse);

        var actual = await Sut!.Get(id, AccountFieldsSelection.Organisations);
        
        AccountApiClient.Verify(x => x.GetResource<LegalEntityViewModel>(It.IsAny<string>()),
            Times.Exactly(accountResponse.LegalEntities.Count));

        Assert.Multiple(() =>
        {
            Assert.That(actual.LegalEntities.Count(), Is.EqualTo(0));
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.LegalEntities, Is.Not.Null);
            Assert.That(actual.PayeSchemes, Is.Null);
            Assert.That(actual.Transactions, Is.Null);
            Assert.That(actual.TeamMembers, Is.Null);
        });
    }

    [Test]
    public async Task ItShoudFailGracefullyAndLogErrorWhenClientThrowsExceptionOnGetResourceAccount()
    {
        const string id = "123";

        AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
            .ThrowsAsync(new Exception());

        var actual = await Sut!.Get(id, AccountFieldsSelection.Organisations);

        Logger!.Verify(
            x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        AccountApiClient.Verify(x => x.GetResource<AccountDetailViewModel>(It.IsAny<string>()), Times.Once);
        AccountApiClient.Verify(x => x.GetResource<LegalEntityViewModel>(It.IsAny<string>()), Times.Never);

        Assert.That(actual, Is.Null);
    }

    [Test]
    public async Task ItShoudFailGracefullyAndLogErrorWhenClientThrowsExceptionOnGetResourceLegalEntity()
    {
        const string id = "123";

        var accountResponse = new AccountDetailViewModel
        {
            LegalEntities = new ResourceList(
                new List<ResourceViewModel>
                {
                    new() { Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC" }
                })
        };

        AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
            .ReturnsAsync(accountResponse);

        var legalEntity = accountResponse.LegalEntities[0];

        AccountApiClient.Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
            .ThrowsAsync(new Exception());

        var actual = await Sut!.Get(id, AccountFieldsSelection.Organisations);
        
        Logger!.Verify(
            x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        AccountApiClient.Verify(x => x.GetResource<LegalEntityViewModel>(It.IsAny<string>()),
            Times.Exactly(accountResponse.LegalEntities.Count));

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.LegalEntities, Is.Empty);
    }
}