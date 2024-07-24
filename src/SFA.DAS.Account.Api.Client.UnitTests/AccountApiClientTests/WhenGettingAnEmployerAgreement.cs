using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests;

public class WhenGettingAnEmployerAgreement : ApiClientTestBase
{
    private const string HashedAccountId = "ABC123";
    private const string HashedLegalEntityId = "DEF456";
    private const string HashedAgreementId = "GHI789";

    private string? _uri;

    protected override void HttpClientSetup()
    {
        _uri = $"/api/accounts/{HashedAccountId}/legalEntities/{HashedLegalEntityId}/agreements/{HashedAgreementId}/agreement";
        var absoluteUri = Configuration!.ApiBaseUrl.TrimEnd('/') + _uri;

        var agreement = new EmployerAgreementView { HashedAccountId = HashedAccountId };

        HttpClient!.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(agreement)));
    }

    [Test]
    public async Task ThenTheCorrectUrlIsCalled()
    {
        //Act
        var response = await ApiClient!.GetEmployerAgreement(HashedAccountId, HashedLegalEntityId, HashedAgreementId);

        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.HashedAccountId, Is.EqualTo(HashedAccountId));
    }
}