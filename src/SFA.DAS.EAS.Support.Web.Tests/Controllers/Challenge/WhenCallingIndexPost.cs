using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.Support.Shared.Authentication;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge
{
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

            MockChallengeHandler.Setup(x => x.Handle(It.IsAny<ChallengePermissionQuery>()))
                .ReturnsAsync(response);

            var actual = await Unit.Index(challengeEntry.Id, challengeEntry);

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.IsInstanceOf<ChallengeViewModel>(((ViewResult)actual).Model);
            Assert.AreEqual(true, ((ChallengeViewModel)((ViewResult)actual).Model!).HasError);
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

            MockChallengeHandler.Setup(x => x.Handle(It.IsAny<ChallengePermissionQuery>()))
                .ReturnsAsync(response);

            var actual = await Unit.Index(challengeEntry.Id, challengeEntry);

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<JsonResult>(actual);

            var result = ((JsonResult)actual).Value as ChallengeValidationResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result!.IsValidResponse);
        }
    }
}