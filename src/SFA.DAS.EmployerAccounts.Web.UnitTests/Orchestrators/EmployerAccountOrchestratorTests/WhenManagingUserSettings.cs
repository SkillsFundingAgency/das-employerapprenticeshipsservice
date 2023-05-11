using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests;

[TestFixture]
public class WhenManagingUserSettings
{
    private Web.Orchestrators.UserSettingsOrchestrator _orchestrator;
    private Mock<IMediator> _mediator;
    private Mock<IEncodingService> _encodingService;

    [SetUp]
    public void Arrange()
    {
        var configuration = new EmployerAccountsConfiguration
        {
            UseGovSignIn = true
        };
        _mediator = new Mock<IMediator>();
        _encodingService = new Mock<IEncodingService>();

        _encodingService.Setup(x => x.Decode(It.IsAny<string>(), It.IsAny<EncodingType>())).Returns(() => 123);

        _orchestrator = new Web.Orchestrators.UserSettingsOrchestrator(_mediator.Object, Mock.Of<ILogger<Web.Orchestrators.UserSettingsOrchestrator>>(), _encodingService.Object, configuration);

        _mediator.Setup(x => x.Send(It.IsAny<GetUserNotificationSettingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserNotificationSettingsQueryResponse
            {
                NotificationSettings = new List<UserNotificationSetting>()
            });

        _mediator.Setup(x => x.Send(It.IsAny<UpdateUserNotificationSettingsCommand>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(new Unit()));
    }

    [Test]
    public async Task ThenTheMediatorIsCalledToRetrieveSettings()
    {
        //Act
        var actual = await _orchestrator.GetNotificationSettingsViewModel("USERREF");

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetUserNotificationSettingsQuery>(s => s.UserRef == "USERREF"), It.IsAny<CancellationToken>()),
            Times.Once);
        actual.Data.UseGovSignIn.Should().BeTrue();
    }

    [Test]
    public async Task ThenTheMediatorIsCalledToUpdateSettings()
    {
        //Arrange
        var settings = new List<UserNotificationSetting>();

        //Act
        await _orchestrator.UpdateNotificationSettings("USERREF", settings);

        //Assert
        _mediator.Verify(x => x.Send(
            It.Is<UpdateUserNotificationSettingsCommand>(s => s.UserRef == "USERREF" && s.Settings == settings), 
            It.IsAny<CancellationToken>()), Times.Once);

    }
}