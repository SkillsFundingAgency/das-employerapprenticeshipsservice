using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests;

public class WhenIManageMyNotificationSettings : ControllerTestBase
{
    private SettingsController _controller;
    private Mock<Web.Orchestrators.UserSettingsOrchestrator> _orchestrator;
    private Mock<IAuthenticationService> _owinWrapper;
    private Mock<IMultiVariantTestingService> _userViewTestingService;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private const string ExpectedRedirectUrl = "http://redirect.local.test";

    [SetUp]
    public void Arrange()
    {
        base.Arrange(ExpectedRedirectUrl);

        _orchestrator = new Mock<Web.Orchestrators.UserSettingsOrchestrator>();

        _owinWrapper = new Mock<IAuthenticationService>();
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

        _controller = new SettingsController( _orchestrator.Object, _flashMessage.Object)
        {
            ControllerContext = ControllerContext,
            Url = new UrlHelper(new ActionContext(HttpContext.Object, Routes, new ActionDescriptor()))
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