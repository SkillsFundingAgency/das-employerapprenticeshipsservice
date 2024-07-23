using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests;

public class WhenGettingAccountUsers : ApiClientTestBase
{
    private TeamMemberViewModel? _teamMember;
    private string? _uri;

    protected override void HttpClientSetup()
    {
        _uri = $"/api/accounts/{TextualAccountId}/users";
        var absoluteUri = Configuration!.ApiBaseUrl.TrimEnd('/') + _uri;
        var fixture = new Fixture();

        _teamMember = fixture.Create<TeamMemberViewModel>();

        var members = new List<TeamMemberViewModel?> { _teamMember };

        HttpClient!
            .Setup(c => c.GetAsync(absoluteUri))
            .Returns(Task.FromResult(JsonConvert.SerializeObject(members)));
    }

    [Test]
    public async Task ThenThePayeSchemeIsReturned()
    {
        // Act
        var response = await ApiClient!.GetAccountUsers(TextualAccountId);
        var viewModel = response?.FirstOrDefault();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);
        });
            
        viewModel.Should().BeEquivalentTo(_teamMember);
    }
}