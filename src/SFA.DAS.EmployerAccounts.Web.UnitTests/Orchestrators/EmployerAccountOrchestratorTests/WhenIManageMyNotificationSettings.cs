using MediatR;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests;

public class WhenIManageMyNotificationSettings : ControllerTestBase
{
    private SettingsController _controller;
    private Mock<Web.Orchestrators.UserSettingsOrchestrator> _orchestrator;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private const string ExpectedRedirectUrl = "http://redirect.local.test";

    [SetUp]
    public void Arrange()
    {
        base.Arrange(ExpectedRedirectUrl);

        AddUserToContext("TEST");

        _orchestrator = new Mock<Web.Orchestrators.UserSettingsOrchestrator>();
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

        _controller = new SettingsController(_orchestrator.Object, _flashMessage.Object)
        {
            ControllerContext = ControllerContext,
            Url = new UrlHelper(new ActionContext(MockHttpContext.Object, Routes, new ActionDescriptor()))
        };
    }

    [Test]
    public async Task ThenMySettingsAreRetrieved()
    {
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