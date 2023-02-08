using MediatR;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests;

public class WhenIManageMyNotificationSettings : ControllerTestBase
{
    private SettingsController _controller;
    private readonly Mock<Web.Orchestrators.UserSettingsOrchestrator> _orchestrator = new();
    private readonly Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage = new();
    private const string ExpectedRedirectUrl = "http://redirect.local.test";

    [SetUp]
    public void Arrange()
    {
        base.Arrange(ExpectedRedirectUrl);

        AddUserToContext("TEST");

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

        _controller = new SettingsController( _orchestrator.Object, _flashMessage.Object, Mock.Of<IMultiVariantTestingService>())
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