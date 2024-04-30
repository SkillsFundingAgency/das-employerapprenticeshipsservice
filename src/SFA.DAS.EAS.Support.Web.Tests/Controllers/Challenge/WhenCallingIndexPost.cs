using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge;

[TestFixture]
public class WhenCallingIndexPost : WhenTestingChallengeController
{
    [Test]
    public async Task ItShouldReturnAViewModelWhenTheChallengeEntryIsInvalid()
    {
        // Arrange
        var challengeEntry = new ChallengeEntry
        {
            Id = "123",
            Balance = "£1000",
            Challenge1 = "1",
            Challenge2 = "A",
            FirstCharacterPosition = 0,
            SecondCharacterPosition = 1,
            Url = "https://tempuri.org/challenge/me/to/a/deul/any/time"
        };

        var response = new ChallengePermissionResponse
        {
            Id = challengeEntry.Id,
            Url = challengeEntry.Url,
            IsValid = false
        };

        MockChallengeHandler!.Setup(x => x.Handle(It.IsAny<ChallengePermissionQuery>()))
            .ReturnsAsync(response);

        // Act
        var actual = await Unit!.Index(challengeEntry.Id, challengeEntry);

        // Assert
        var result = actual.Should().BeAssignableTo<ViewResult>();
        var model = result.Subject.Model.Should().BeAssignableTo<ChallengeViewModel>();
        model.Subject.HasError.Should().BeTrue();
    }

    [Test]
    public async Task ItShouldReturnChallengeValidationJsonResultWhenTheChallengeEntryIsValid()
    {
        // Arrange
        var challengeEntry = new ChallengeEntry
        {
            Id = "123",
            Balance = "£1000",
            Challenge1 = "1",
            Challenge2 = "A",
            FirstCharacterPosition = 1,
            SecondCharacterPosition = 4,
            Url = "https://tempuri.org/challenge/me/to/a/deul/any/time"
        };

        var response = new ChallengePermissionResponse
        {
            Id = challengeEntry.Id,
            Url = challengeEntry.Url,
            IsValid = true
        };

        MockChallengeHandler!.Setup(x => x.Handle(It.IsAny<ChallengePermissionQuery>()))
            .ReturnsAsync(response);

        // Act
        var actual = await Unit!.Index(challengeEntry.Id, challengeEntry);

        // Assert
        var result = actual
            .Should()
            .BeAssignableTo<JsonResult>()
            .Which.Value
            .Should()
            .BeAssignableTo<ChallengeValidationResult>();

        result.Subject.IsValidResponse.Should().BeTrue();
    }
}