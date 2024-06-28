using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests;

public class WhenGettingPayeSchemesForAnAccount : ApiClientTestBase
{
    private string? _uri;
    private List<ResourceViewModel>? _payeSchemes;

    protected override void HttpClientSetup()
    {
        _uri = $"/api/accounts/{TextualAccountId}/payeschemes";
        var absoluteUri = Configuration!.ApiBaseUrl.TrimEnd('/') + _uri;

        _payeSchemes = new List<ResourceViewModel>() { new() { Id = "1", Href = "/api/payeschemes/scheme?ref=test1" } };

        HttpClient!.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_payeSchemes)));
    }

    [Test]
    public async Task ThenTheAccountDetailsAreReturned()
    {
        // Act
        var response = await ApiClient!.GetPayeSchemesConnectedToAccount(TextualAccountId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(_payeSchemes);
    }
}