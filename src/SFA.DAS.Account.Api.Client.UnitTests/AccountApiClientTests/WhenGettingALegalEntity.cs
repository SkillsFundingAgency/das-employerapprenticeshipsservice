using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests;

public class WhenGettingALegalEntity : ApiClientTestBase
{
    private LegalEntityViewModel? _expectedLegalEntity;
    private string? _uri;

    protected override void HttpClientSetup()
    {
        _uri = $"/api/accounts/{TextualAccountId}/legalentities/123";
        var absoluteUri = Configuration!.ApiBaseUrl.TrimEnd('/') + _uri;

        _expectedLegalEntity = new LegalEntityViewModel
        {
            LegalEntityId = 123,
            Code = "Code",
            Name = "Name",
            DateOfInception = DateTime.Now.AddYears(-1),
            Source = "Source",
            Address = "An address",
            Status = "Status"
        };

        HttpClient!.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedLegalEntity)));
    }

    [Test]
    public async Task ThenTheLegalEntityIsReturned()
    {
        // Act
        var response = await ApiClient!.GetLegalEntity(TextualAccountId,123);

        // Assert
        response.Should().BeAssignableTo<LegalEntityViewModel>();
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(_expectedLegalEntity);
    }
}