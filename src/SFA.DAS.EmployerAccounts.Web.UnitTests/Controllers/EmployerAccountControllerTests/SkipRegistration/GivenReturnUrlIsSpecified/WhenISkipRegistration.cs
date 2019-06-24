﻿using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.SkipRegistration.GivenReturnUrlIsSpecified
{ 
    class WhenISkipRegistration : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private OrchestratorResponse<EmployerAccountViewModel> _response;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<ICookieStorageService<ReturnUrlModel>> _returnUrlCookieStorage;
        private const string HashedAccountId = "ABC123";
        private const string ExpectedReturnUrl = "test.com";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _returnUrlCookieStorage = new Mock<ICookieStorageService<ReturnUrlModel>>();

            _response = new OrchestratorResponse<EmployerAccountViewModel>()
            {
                Data = new EmployerAccountViewModel
                {
                    HashedId = HashedAccountId
                },
                Status = HttpStatusCode.OK
            };

            _orchestrator.Setup(x =>
                    x.CreateMinimalUserAccountForSkipJourney(It.IsAny<CreateUserAccountViewModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);

            _returnUrlCookieStorage.Setup(x => x.Get("SFA.DAS.EmployerAccounts.Web.Controllers.ReturnUrlCookie"))
                .Returns(new ReturnUrlModel() {Value = ExpectedReturnUrl});

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                _returnUrlCookieStorage.Object,
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenIShouldGoToTheReturnUrl()
        {
            //Act
            var result = await _employerAccountController.SkipRegistration() as RedirectResult;

            //Assert
            Assert.AreEqual(ExpectedReturnUrl, result.Url);
        }
    }
}