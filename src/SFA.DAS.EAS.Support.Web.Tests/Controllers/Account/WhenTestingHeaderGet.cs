using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account
{
    [TestFixture]
    public class WhenTestingHeaderGet : WhenTestingAccountController
    {
        [Test]
        public async Task ItShouldReturnHttpNotFoundOnNoSearchResultsFound()
        {
            var accountResponse = new AccountReponse
            {
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };
            
            const string id = "123";
            AccountHandler!.Setup(x => x.Find(id)).ReturnsAsync(accountResponse);
            var actual = await Unit!.Header(id);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
        {
            var accountResponse = new AccountReponse
            {
                StatusCode = SearchResponseCodes.SearchFailed
            };
            const string id = "123";
            AccountHandler!.Setup(x => x.Find(id)).ReturnsAsync(accountResponse);
            var actual = await Unit!.Header(id);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouldReturnTheSubHeaderViewAndModelOnSuccess()
        {
            var accountResponse = new AccountReponse
            {
                Account = new Core.Models.Account
                {
                    AccountId = 123,
                    DasAccountName = "Test Account",
                    DateRegistered = DateTime.Today,
                    OwnerEmail = "owner@tempuri.org"
                },
                StatusCode = SearchResponseCodes.Success
            };
            
            const string id = "123";
            AccountHandler!.Setup(x => x.Find(id)).ReturnsAsync(accountResponse);
            var actual = await Unit!.Header(id);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual("SubHeader", ((ViewResult)actual).ViewName);
            Assert.IsInstanceOf<Core.Models.Account>(((ViewResult)actual).Model);
            Assert.AreEqual(accountResponse.Account, ((ViewResult)actual).Model);
        }
    }
}