using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge;

[TestFixture]
public class WhenCallingIndexGet : WhenTestingChallengeController
{
    [Test]
    public async Task ItShouldReturnHttpNoFoundWhenThereIsNotAMatch()
    {
        // Arrange
        var challengeResponse = new ChallengeResponse
        {
            Account = null,
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };

        const string id = "123";

        MockChallengeHandler!.Setup(x => x.Get(id))
            .ReturnsAsync(challengeResponse);

        // Act
        var actual = await Unit!.Index(id);

        // Assert
        actual.Should().BeAssignableTo<NotFoundObjectResult>();
    }

    [Test]
    public async Task ItShouldReturnHttpNoFoundWhenTheSearchFails()
    {
        // Arrange
        var challengeResponse = new ChallengeResponse
        {
            Account = null,
            StatusCode = SearchResponseCodes.SearchFailed
        };

        const string id = "123";

        MockChallengeHandler!.Setup(x => x.Get(id))
            .ReturnsAsync(challengeResponse);

        // Act
        var actual = await Unit!.Index(id);

        // Assert
        actual.Should().BeAssignableTo<NotFoundObjectResult>();
    }

    [Test]
    public async Task ItShouldReturnTheChallengeViewWithAModelWhenThereIsAMatch()
    {
        // Arrange
        var challengeResponse = new ChallengeResponse
        {
            Account = new Core.Models.Account
            {
                AccountId = 123,
                HashedAccountId = "ERERER",
                DasAccountName = "Test Account"
            },
            StatusCode = SearchResponseCodes.Success
        };

        const string id = "123";

        MockChallengeHandler!.Setup(x => x.Get(id))
            .ReturnsAsync(challengeResponse);

        // Act
        var actual = await Unit!.Index(id);

        // Assert
        var viewResult = actual.Should().BeAssignableTo<ViewResult>();
        viewResult.Subject.Model.Should().BeAssignableTo<ChallengeViewModel>();
    }
}