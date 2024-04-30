using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests;

public class WhenGettingLevyDeclarations : ApiClientTestBase
{
    protected override void HttpClientSetup()
    {
        HttpClient!.Setup(c => c.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(
                JsonConvert.SerializeObject(new List<LevyDeclarationViewModel>
                    {
                        new()
                    }
                )));
    }

    [Test]
    public async Task ThenItShouldCallTheApiWithTheCorrectUrl()
    {
        // Act
        await ApiClient!.GetLevyDeclarations(TextualAccountId);

        // Assert
        const string expectedUrl = $"http://some-url/api/accounts/{TextualAccountId}/levy";
        HttpClient!.Verify(c => c.GetAsync(expectedUrl), Times.Once);
    }

    [Test]
    public async Task ThenItShouldReturnLevyDeclarations()
    {
        // Act
        var actual = await ApiClient!.GetLevyDeclarations(TextualAccountId);

        // Assert
        Assert.That(actual, Is.Not.Null);
    }

    [Test]
    public async Task ThenItShouldDeserializeTheResponseCorrectly()
    {
        // Act
        var response = await ApiClient!.GetLevyDeclarations(TextualAccountId);

        // Assert
        response.Should().BeAssignableTo<List<LevyDeclarationViewModel>>();
    }
}