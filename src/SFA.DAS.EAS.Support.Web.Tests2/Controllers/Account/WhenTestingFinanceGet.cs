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
            var accountFinanceReponse = new AccountFinanceResponse
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
            AccountHandler.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceReponse);
            var actual = await Unit.Finance("123");

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual(true, String.IsNullOrEmpty(((ViewResult)actual).ViewName));
            Assert.IsInstanceOf<FinanceViewModel>(((ViewResult)actual).Model);
            Assert.AreEqual(accountFinanceReponse.Account, ((FinanceViewModel)((ViewResult)actual).Model).Account);
            Assert.AreEqual(accountFinanceReponse.Balance, ((FinanceViewModel)((ViewResult)actual).Model).Balance);
        }

        [Test]
        public async Task ItShouodReturnHttpNotFoundOnNoSearchResultsFound()
        {
            var accountFinanceReponse = new AccountFinanceResponse
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
            AccountHandler.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceReponse);
            var actual = await Unit.Finance("123");
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouodReturnHttpNotFoundOnSearchFailed()
        {
            var accountFinanceReponse = new AccountFinanceResponse
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
            AccountHandler.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceReponse);
            var actual = await Unit.Finance("123");

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }
    }
}