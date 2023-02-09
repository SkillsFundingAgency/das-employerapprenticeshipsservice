using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.TasksApi;
using SFA.DAS.Encoding;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.DismissMonthlyTaskReminderTests
{
    public class WhenIDismissATaskReminder
    {
        private Mock<ITaskService> _taskService;
        private DismissMonthlyTaskReminderCommandHandler _handler;
        private DismissMonthlyTaskReminderCommand _command;
        private Mock<ILog> _logger;
        private Mock<IValidator<DismissMonthlyTaskReminderCommand>> _validator;
        private Mock<IEncodingService> _encodingService;
        private const long AccountId = 3;
        
        [SetUp]
        public void Arrange()
        {
            _taskService = new Mock<ITaskService>();
            _logger = new Mock<ILog>();
            _validator = new Mock<IValidator<DismissMonthlyTaskReminderCommand>>();

            _command = new DismissMonthlyTaskReminderCommand
            {
                HashedAccountId = "ABC123",
                ExternalUserId = " DEF456",
                TaskType = TaskType.LevyDeclarationDue
            };

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(_command.HashedAccountId, EncodingType.AccountId)).Returns(AccountId);

            _handler = new DismissMonthlyTaskReminderCommandHandler(_taskService.Object, _validator.Object, _encodingService.Object);

            _validator.Setup(x => x.Validate(It.IsAny<DismissMonthlyTaskReminderCommand>()))
                .Returns(new ValidationResult());
        }

        [Test]
        public async Task ThenTheDismissShouldBeSaved()
        {
            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            _taskService.Verify(
                x => x.DismissMonthlyTaskReminder(AccountId, _command.ExternalUserId, _command.TaskType), Times.Once);
        }

        [Test]
        public void ThenIfTheDismissIsInvalidItShouldNotBeSaved()
        {
            // Arrange 
            _validator.Setup(x => x.Validate(It.IsAny<DismissMonthlyTaskReminderCommand>()))
            .Returns(new ValidationResult()
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        {nameof(DismissMonthlyTaskReminderCommand.HashedAccountId),"Test Error"}
                    }
                });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async() => await _handler.Handle(_command, CancellationToken.None));

            //Assert
            _taskService.Verify(
                x => x.DismissMonthlyTaskReminder(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<TaskType>()), Times.Never);
        }
    }
}
