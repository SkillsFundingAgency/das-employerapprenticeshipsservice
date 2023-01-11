using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.SkipRegistration
{
    class WhenISkipRegistration : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private EmployerAccountData _accountData;
        private OrchestratorResponse<EmployerAccountViewModel> _response;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string HashedAccountId = "ABC123";
        private const string ExpectedUserId = "USER123";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _owinWrapper.Setup(x => x.GetClaimValue(ControllerConstants.UserRefClaimKeyName)).Returns(ExpectedUserId);
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();  

            _response = new OrchestratorResponse<EmployerAccountViewModel>()
            {
                Data = new EmployerAccountViewModel
                {
                    HashedId = HashedAccountId
                },
                Status = HttpStatusCode.OK
            };

            _orchestrator.Setup(x => x.CreateMinimalUserAccountForSkipJourney(It.IsAny<CreateUserAccountViewModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);        

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenTheAccountIsCreated()
        {
            //Act
            var result = await _employerAccountController.SkipRegistration() as RedirectToRouteResult;

            //Assert
            _orchestrator.Verify(x => x.CreateMinimalUserAccountForSkipJourney(It.Is<CreateUserAccountViewModel>(vm => vm.UserId == ExpectedUserId && vm.OrganisationName == "MY ACCOUNT"), It.IsAny<HttpContextBase>()));
        }

        [Test]
        public async Task ThenIShouldGoToTheHomePage()
        {
            //Act
            var result = await _employerAccountController.SkipRegistration() as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(ControllerConstants.IndexActionName, result.RouteValues["Action"]);
            Assert.AreEqual(ControllerConstants.EmployerTeamControllerName, result.RouteValues["Controller"]);
        }

        [Test]
        public async Task ThenIShouldGetBackTheAccountId()
        {
            //Act
            var result = await _employerAccountController.SkipRegistration() as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HashedAccountId, result.RouteValues["HashedAccountId"]);
        }
    }
}