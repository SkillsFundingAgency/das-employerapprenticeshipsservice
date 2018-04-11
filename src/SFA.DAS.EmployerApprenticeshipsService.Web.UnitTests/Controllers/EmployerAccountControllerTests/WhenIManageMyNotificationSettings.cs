using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIManageMyNotificationSettings : ControllerTestBase
    {
        private SettingsController _controller;
        private Mock<UserSettingsOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<UserSettingsOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator.Setup(x => x.GetNotificationSettingsViewModel(It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<NotificationSettingsViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new NotificationSettingsViewModel()
                });

            _orchestrator.Setup(x => x.UpdateNotificationSettings(
                It.IsAny<string>(),
                It.IsAny<List<UserNotificationSetting>>()))
                .Returns(() => Task.FromResult(new Unit()));

            _controller = new SettingsController(_owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, 
                _userViewTestingService.Object, _flashMessage.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenMySettingsAreRetrieved()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("TEST");

            //Act
            await _controller.NotificationSettings();

            //Assert
            _orchestrator.Verify(x => x.GetNotificationSettingsViewModel(
                It.Is<string>(userRef => userRef == "TEST")
            ), Times.Once);
        }

        [Test]
        public async Task TheMySettingsAreUpdated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("TEST");

            var payload = new NotificationSettingsViewModel();

            //Act
            await _controller.NotificationSettings(payload);

            //Assert
            _orchestrator.Verify(x => x.UpdateNotificationSettings(
                It.Is<string>(userRef => userRef == "TEST"),
                It.IsAny<List<UserNotificationSetting>>()),
                Times.Once);
        }
    }
}
