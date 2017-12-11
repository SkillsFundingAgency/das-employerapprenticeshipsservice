using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.API.Client;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.TaskServiceTests
{
    class WhenIDismissAReminderTask
    {
        private Mock<ITaskApiClient> _apiClient;
        private Mock<ILog> _logger;
        private TaskService _service;
       

        [SetUp]
        public void Arrange()
        {
            _apiClient = new Mock<ITaskApiClient>();
            _logger = new Mock<ILog>();

            _service = new TaskService(_apiClient.Object, _logger.Object);

            _apiClient.Setup(x =>
                x.AddUserReminderSupression(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task ThenTheTaskShouldGetDismissed()
        {
            //Arrange
            const long accountId = 2;
            const long userId = 4;
            const TaskType type = TaskType.LevyDeclarationDue;

            //Act
            await _service.DismissMonthlyTaskReminder(accountId, userId, type);

            //Arrange
            var taskName = Enum.GetName(typeof(TaskType), type);
            _apiClient.Verify(x => x.AddUserReminderSupression(accountId.ToString(),userId.ToString(), taskName), Times.Once);
        }

        [Test]
        public async Task ThenIfTheDismissFailsItShouldBeLogged()
        {
            //Arrange
            _apiClient.Setup(x =>
                x.AddUserReminderSupression(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            //Act
            await _service.DismissMonthlyTaskReminder(2, 4, TaskType.LevyDeclarationDue);

            //Assert
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
        [Test]
        public async Task ThenIfNoTaskIsDismissedTheApiShouldNotBeCalled()
        { 
            //Act
            await _service.DismissMonthlyTaskReminder(2, 4, TaskType.None);

            //Assert
            _apiClient.Verify(x =>
                x.AddUserReminderSupression(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
