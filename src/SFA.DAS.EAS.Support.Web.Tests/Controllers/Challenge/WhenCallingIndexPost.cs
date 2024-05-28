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
    /// <summary>
    ///     Note that this Controller method scenario sets HttpResponse.StatusCode = 403 (Forbidden), this result is not
    ///     testable from a unit test
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task ItShouldReturnAViewModelWhenTheChallengeEntryIsInvalid()
    {
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

        var actual = await Unit!.Index(challengeEntry.Id, challengeEntry);

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<ViewResult>());
        Assert.That(((ViewResult)actual).Model, Is.InstanceOf<ChallengeViewModel>());
        Assert.That(true, Is.EqualTo(((ChallengeViewModel)((ViewResult)actual).Model!).HasError));
    }

    [Test]
    public async Task ItShouldReturnChallengeValidationJsonResultWhenTheChallengeEntryIsValid()
    {
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

        var actual = await Unit!.Index(challengeEntry.Id, challengeEntry);

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<JsonResult>());

        var result = ((JsonResult)actual).Value as ChallengeValidationResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.IsValidResponse, Is.True);
    }
}