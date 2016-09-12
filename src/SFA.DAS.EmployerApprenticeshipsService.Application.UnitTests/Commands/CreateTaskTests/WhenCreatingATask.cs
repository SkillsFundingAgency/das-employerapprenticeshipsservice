using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateTask;
using SFA.DAS.Tasks.Api.Client;
using SFA.DAS.Tasks.Domain.Entities;
using SystemThreading = System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateTaskTests
{
    [TestFixture]
    public sealed class WhenCreatingATask
    {
        private CreateTaskCommandHandler _handler;
        private Mock<ITasksApi> _mockTaskApi;
        private CreateTaskCommand _validCommand;

        [SetUp]
        public void Setup()
        {
            _validCommand = new CreateTaskCommand { ProviderId = 12L };

            _mockTaskApi = new Mock<ITasksApi>();
            _handler = new CreateTaskCommandHandler(_mockTaskApi.Object);
        }

        [Test]
        public async SystemThreading.Task ThenTheTaskApiShouldBeCalled()
        {
            await _handler.Handle(_validCommand);

            _mockTaskApi.Verify(x => x.CreateTask(It.IsAny<string>(), It.IsAny<Task>()));
        }
    }
}
