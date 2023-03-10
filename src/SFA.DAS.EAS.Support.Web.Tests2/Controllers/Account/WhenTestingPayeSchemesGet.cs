using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account
{
    [TestFixture]
    public class WhenTestingPayeSchemesGet : WhenTestingAccountController
    {
        [Test]
        public async Task ItShouldReturnAViewAndModelOnSuccess()
        {
            var reponse = new AccountPayeSchemesResponse
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
            var id = "123";
            AccountHandler.Setup(x => x.FindPayeSchemes(id)).ReturnsAsync(reponse);
            var actual = await Unit.PayeSchemes("123");

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual(true, String.IsNullOrEmpty(((ViewResult)actual).ViewName));
            Assert.IsInstanceOf<AccountDetailViewModel>(((ViewResult)actual).Model);
            Assert.AreEqual(reponse.Account, ((AccountDetailViewModel)((ViewResult)actual).Model).Account);
            Assert.IsNull(((AccountDetailViewModel)((ViewResult)actual).Model).SearchUrl);
        }

        [Test]
        public async Task ItShouodReturnHttpNotFoundOnNoSearchResultsFound()
        {
            var reponse = new AccountPayeSchemesResponse
            {
                Account = new Core.Models.Account
                {
                    AccountId = 123,
                    DasAccountName = "Test Account",
                    DateRegistered = DateTime.Today,
                    OwnerEmail = "owner@tempuri.org"
                },
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };
            var id = "123";
            AccountHandler.Setup(x => x.FindPayeSchemes(id)).ReturnsAsync(reponse);
            var actual = await Unit.PayeSchemes("123");
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouodReturnHttpNotFoundOnSearchFailed()
        {
            var reponse = new AccountPayeSchemesResponse
            {
                Account = new Core.Models.Account
                {
                    AccountId = 123,
                    DasAccountName = "Test Account",
                    DateRegistered = DateTime.Today,
                    OwnerEmail = "owner@tempuri.org"
                },
                StatusCode = SearchResponseCodes.SearchFailed
            };
            var id = "123";
            AccountHandler.Setup(x => x.FindPayeSchemes(id)).ReturnsAsync(reponse);
            var actual = await Unit.PayeSchemes("123");

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }
    }
}