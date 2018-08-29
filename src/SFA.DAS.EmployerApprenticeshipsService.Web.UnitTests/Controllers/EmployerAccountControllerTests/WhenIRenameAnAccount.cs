﻿using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIRenameAnAccount : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator.Setup(x => x.RenameEmployerAccount(It.IsAny<RenameEmployerAccountViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<RenameEmployerAccountViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new RenameEmployerAccountViewModel()
                });

            _employerAccountController = new EmployerAccountController(_owinWrapper.Object, _orchestrator.Object, 
                _featureToggle.Object, _userViewTestingService.Object, logger.Object, _flashMessage.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenTheAccountIsRenamed()
        {
            //Arrange
            var model = new RenameEmployerAccountViewModel
            {
                CurrentName = "Test Account",
                NewName = "New Account Name",
                HashedId = "ABC123"
            };

            //Act
            await _employerAccountController.RenameAccount(model);

            //Assert
            _orchestrator.Verify(x => x.RenameEmployerAccount(It.Is<RenameEmployerAccountViewModel>(r =>
                r.CurrentName == "Test Account"
                && r.NewName == "New Account Name"
            ), It.IsAny<string>()));
        }

    }
}
