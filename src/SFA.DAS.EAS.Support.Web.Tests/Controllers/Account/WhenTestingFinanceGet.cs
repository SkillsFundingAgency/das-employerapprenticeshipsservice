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
    public class WhenTestingFinanceGet : WhenTestingAccountController
    {
        [Test]
        public async Task ItShouldReturnAViewAndModelOnSuccess()
        {
            var accountFinanceResponse = new AccountFinanceResponse
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
            AccountHandler!.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceResponse);
            var actual = await Unit!.Finance("123");

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual(true, string.IsNullOrEmpty(((ViewResult)actual).ViewName));
            Assert.IsInstanceOf<FinanceViewModel>(((ViewResult)actual).Model);
            Assert.AreEqual(accountFinanceResponse.Account, ((FinanceViewModel)((ViewResult)actual).Model!).Account);
            Assert.AreEqual(accountFinanceResponse.Balance, ((FinanceViewModel)((ViewResult)actual).Model!)!.Balance);
        }

        [Test]
        public async Task ItShouldReturnHttpNotFoundOnNoSearchResultsFound()
        {
            var accountFinanceResponse = new AccountFinanceResponse
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
            const string id = "123";
            AccountHandler!.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceResponse);
            var actual = await Unit!.Finance("123");
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
        {
            var accountFinanceResponse = new AccountFinanceResponse
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
            
            const string id = "123";
            AccountHandler!.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceResponse);
            var actual = await Unit!.Finance("123");

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }
    }
}