using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.Web.Services;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account
{
    public abstract class WhenTestingAccountController
    {
        protected Mock<IAccountHandler> AccountHandler;
        protected Mock<IPayeLevySubmissionsHandler> _payeLevySubmissionsHandler;
        protected Mock<IPayeLevyMapper> _payeLevyDeclarationMapper;
        protected AccountController Unit;


        [SetUp]
        public void Setup()
        {
            AccountHandler = new Mock<IAccountHandler>();
            _payeLevySubmissionsHandler = new Mock<IPayeLevySubmissionsHandler>();
            _payeLevyDeclarationMapper = new Mock<IPayeLevyMapper>();

            Unit = new AccountController(AccountHandler.Object,
                _payeLevySubmissionsHandler.Object,
                _payeLevyDeclarationMapper.Object);
        }
    }

    [TestFixture]
    public class WhenTestingIndexGet : WhenTestingAccountController
    {
        [Test]
        public async Task ItShouldReturnAViewAndModelOnSuccess()
        {
            var reponse = new AccountDetailOrganisationsResponse
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
            AccountHandler.Setup(x => x.FindOrganisations(id)).ReturnsAsync(reponse);
            var actual = await Unit.Index("123");

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
            var reponse = new AccountDetailOrganisationsResponse
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
            AccountHandler.Setup(x => x.FindOrganisations(id)).ReturnsAsync(reponse);
            var actual = await Unit.Index("123");
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }

        [Test]
        public async Task ItShouodReturnHttpNotFoundOnSearchFailed()
        {
            var reponse = new AccountDetailOrganisationsResponse
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
            AccountHandler.Setup(x => x.FindOrganisations(id)).ReturnsAsync(reponse);
            var actual = await Unit.Index("123");

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }
    }
}