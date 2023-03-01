//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.EAS.Support.ApplicationServices.Models;

//namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account
//{
//    [TestFixture]
//    public class WhenTestingHeaderGet : WhenTestingAccountController
//    {
//        [Test]
//        public async Task ItShouldReturnHttpNotFoundOnNoSearchResultsFound()
//        {
//            var accountReponse = new AccountReponse
//            {
//                StatusCode = SearchResponseCodes.NoSearchResultsFound
//            };
//            var id = "123";
//            AccountHandler.Setup(x => x.Find(id)).ReturnsAsync(accountReponse);
//            var actual = await Unit.Header(id);
//            Assert.IsNotNull(actual);
//            Assert.IsInstanceOf<NotFoundResult>(actual);
//        }

//        [Test]
//        public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
//        {
//            var accountReponse = new AccountReponse
//            {
//                StatusCode = SearchResponseCodes.SearchFailed
//            };
//            var id = "123";
//            AccountHandler.Setup(x => x.Find(id)).ReturnsAsync(accountReponse);
//            var actual = await Unit.Header(id);
//            Assert.IsNotNull(actual);
//            Assert.IsInstanceOf<NotFoundResult>(actual);
//        }

//        [Test]
//        public async Task ItShouldReturnTheSubHeaderViewAndModelOnSuccess()
//        {
//            var accountReponse = new AccountReponse
//            {
//                Account = new Core.Models.Account
//                {
//                    AccountId = 123,
//                    DasAccountName = "Test Account",
//                    DateRegistered = DateTime.Today,
//                    OwnerEmail = "owner@tempuri.org"
//                },
//                StatusCode = SearchResponseCodes.Success
//            };
//            var id = "123";
//            AccountHandler.Setup(x => x.Find(id)).ReturnsAsync(accountReponse);
//            var actual = await Unit.Header(id);
//            Assert.IsNotNull(actual);
//            Assert.IsInstanceOf<ViewResult>(actual);
//            Assert.AreEqual("SubHeader", ((ViewResult) actual).ViewName);
//            Assert.IsInstanceOf<Core.Models.Account>(((ViewResult) actual).Model);
//            Assert.AreEqual(accountReponse.Account, ((ViewResult) actual).Model);
//        }
//    }
//}