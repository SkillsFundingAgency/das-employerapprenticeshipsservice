using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.API.Client;
using SFA.DAS.Tasks.API.Types.DTOs;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.TaskServiceTests
{
    public class WhenIGetTasks
    {
        private Mock<ITaskApiClient> _apiClient;
        private Mock<ILog> _logger;
        private TaskService _service;
        private List<TaskDto> _taskDtos;
            
        [SetUp]
        public void Arrange()
        {
            _taskDtos = new List<TaskDto>
            {
                new TaskDto()
            };

            _apiClient = new Mock<ITaskApiClient>();
            _logger = new Mock<ILog>();

            _service = new TaskService(_apiClient.Object, _logger.Object);

            _apiClient.Setup(x => x.GetTasks(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_taskDtos);
        }

        [Test]
        public async Task ThenIShouldReturnTasksFromClient()
        {
            //Act
            const long accountId = 2;
            const string externalUserId = "ABC123";
            var tasks = await _service.GetAccountTasks(accountId, externalUserId);

            //Assert
            Assert.AreEqual(1, tasks.Count());
            _apiClient.Verify(x => x.GetTasks(accountId.ToString(), externalUserId.ToString()), Times.Once);
        }

        [Test]
        public async Task ThenIShouldReturnNoTasksFromClientAndLogErrorWhenClientCausesException()
        {
            //Arrange
            _apiClient.Setup(x => x.GetTasks(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            //Act
            var tasks = await _service.GetAccountTasks(2, "ABC123");

            //Assert
            Assert.IsEmpty(tasks);
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
    }
}
