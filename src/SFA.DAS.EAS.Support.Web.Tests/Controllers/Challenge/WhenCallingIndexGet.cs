using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge
{
    [TestFixture]
    public class WhenCallingIndexGet : WhenTestingChallengeController
    {
        [Test]
        public async Task ItShouldReturnHttpNoFoundWhenThereIsNotAMatch()
        {
            var challengeResponse = new ChallengeResponse
            {
                Account = null,
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };

            var id = "123";

            MockChallengeHandler.Setup(x => x.Get(id))
                .ReturnsAsync(challengeResponse);

            var actual = await Unit.Index(id);

            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouldReturnHttpNoFoundWhenTheSearchFails()
        {
            var challengeResponse = new ChallengeResponse
            {
                Account = null,
                StatusCode = SearchResponseCodes.SearchFailed
            };

            var id = "123";

            MockChallengeHandler.Setup(x => x.Get(id))
                .ReturnsAsync(challengeResponse);

            var actual = await Unit.Index(id);

            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouldReturnTheChallengeViewWithAModelWhenThereIsAMatch()
        {
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

            var id = "123";

            MockChallengeHandler.Setup(x => x.Get(id))
                .ReturnsAsync(challengeResponse);

            var actual = await Unit.Index(id);

            Assert.IsInstanceOf<ViewResult>(actual);
            var viewResult = (ViewResult) actual;
            Assert.IsInstanceOf<ChallengeViewModel>(viewResult.Model);
        }
    }
}