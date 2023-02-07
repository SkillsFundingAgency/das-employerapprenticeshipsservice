using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests;

[TestFixture]
public class WhenManagingUserSettings
{
    private Web.Orchestrators.UserSettingsOrchestrator _orchestrator;
    private Mock<IMediator> _mediator;
    private Mock<IHashingService> _hashingService;

    [SetUp]
    public void Arrange()
    {
        _mediator = new Mock<IMediator>();
        _hashingService = new Mock<IHashingService>();

        _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(() => 123);

        _orchestrator = new Web.Orchestrators.UserSettingsOrchestrator(_mediator.Object, _hashingService.Object, Mock.Of<ILogger<Web.Orchestrators.UserSettingsOrchestrator>>());

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
        await _orchestrator.GetNotificationSettingsViewModel("USERREF");

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetUserNotificationSettingsQuery>(s => s.UserRef == "USERREF"), It.IsAny<CancellationToken>()),
            Times.Once);
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