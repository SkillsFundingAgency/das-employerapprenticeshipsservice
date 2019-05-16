using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
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

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();       
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

            _orchestrator.Setup(x => x.CreateUserAccount(It.IsAny<CreateUserAccountViewModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);        

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenIShouldGoToTheHomePage()
        {
            //Act
            var result = await _employerAccountController.GetGovernmentFunding(1) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(ControllerConstants.IndexActionName, result.RouteValues["Action"]);
            Assert.AreEqual(ControllerConstants.EmployerTeamControllerName, result.RouteValues["Controller"]);
        }

        [Test]
        public async Task ThenIShouldGetBackTheAccountId()
        {
            //Act
            var result = await _employerAccountController.GetGovernmentFunding(1) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HashedAccountId, result.RouteValues["HashedAccountId"]);
        }
    }
}