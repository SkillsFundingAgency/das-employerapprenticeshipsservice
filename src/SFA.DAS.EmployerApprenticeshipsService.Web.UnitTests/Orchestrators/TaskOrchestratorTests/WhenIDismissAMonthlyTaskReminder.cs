using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.DismissMonthlyTaskReminder;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.API.Types.Enums;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.TaskOrchestratorTests
{
    public class WhenIDismissAMonthlyTaskReminder
    {
        private TaskOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _orchestrator = new TaskOrchestrator(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheDismissShouldBeSaved()
        {
            //Arrange
            const string hashedAccountId = "ABC123";
            const string hashedUserId = "DEF456";
            const TaskType taskType = TaskType.LevyDeclarationDue;

            var taskTypeString = Enum.GetName(typeof(TaskType), taskType);

            //Act
            var result = await _orchestrator.DismissMonthlyReminderTask(hashedAccountId, hashedUserId, taskTypeString);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
            _mediator.Verify(x => x.SendAsync(It.Is<DismissMonthlyTaskReminderCommand>( command =>
            command.HashedAccountId.Equals(hashedAccountId) &&
            command.ExternalUserId.Equals(hashedUserId) && 
            command.TaskType == taskType)), Times.Once);
        }

        [Test]
        public async Task ThenIfTheRequestIsInvalidIShouldBeTold()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<DismissMonthlyTaskReminderCommand>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
                {
                    {nameof(DismissMonthlyTaskReminderCommand.HashedAccountId),"test error" }
                }));

            //Act
            var result = await _orchestrator.DismissMonthlyReminderTask("ABC123", "DEF123", "LevyDeclarationDue");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }

        [Test]
        public async Task ThenIfTheTaskIsInvalidIShouldBeTold()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<DismissMonthlyTaskReminderCommand>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
                {
                    {nameof(DismissMonthlyTaskReminderCommand.HashedAccountId),"test error" }
                }));

            //Act
            var result = await _orchestrator.DismissMonthlyReminderTask("ABC123", "DEF123", "this is not a task");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }

        [Test]
        public async Task ThenIfAnInternalServerErrorOccursIShouldBeTold()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<DismissMonthlyTaskReminderCommand>()))
                .ThrowsAsync(new Exception());

            //Act
            var result = await _orchestrator.DismissMonthlyReminderTask("ABC123", "DEF123", "LevyDeclarationDue");

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.Status);
        }
    }
}
