using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;

using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
{
    [TestFixture]
    public class WhenManagingUserSettings
    {
        private Web.Orchestrators.UserSettingsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _hashingService = new Mock<IHashingService>();

            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(() => 123);

            _orchestrator = new Web.Orchestrators.UserSettingsOrchestrator(_mediator.Object, _hashingService.Object, _logger.Object);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserNotificationSettingsQuery>()))
                .ReturnsAsync(new GetUserNotificationSettingsQueryResponse
                {
                    NotificationSettings = new List<UserNotificationSetting>()
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<UpdateUserNotificationSettingsCommand>()))
                .Returns(() => Task.FromResult(new Unit()));
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToRetrieveSettings()
        {
            var externalUserId = Guid.NewGuid();    
            //Act
            await _orchestrator.GetNotificationSettingsViewModel(externalUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(
                It.Is<GetUserNotificationSettingsQuery>(s => s.ExternalUserId.Equals(externalUserId))),
                Times.Once);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToUpdateSettings()
        {
            var testGuid = Guid.NewGuid();
            //Arrange
            var settings = new List<UserNotificationSetting>();

            //Act
            await _orchestrator.UpdateNotificationSettings(testGuid, settings);

            //Assert
            _mediator.Verify(x => x.SendAsync(
                It.Is<UpdateUserNotificationSettingsCommand>(
                    s => s.ExternalUserId.Equals(testGuid)
                    && s.Settings == settings)
                ), Times.Once);

        }
    }
}
