using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountTasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Tasks.API.Types.DTOs;


namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTasksTests
{
    public class WhenIGetAnAccountsTasks : QueryBaseTest<GetAccountTasksQueryHandler, GetAccountTasksQuery, GetAccountTasksResponse>
    {
        private Mock<ITaskService> _taskService;
        public override GetAccountTasksQuery Query { get; set; }
        public override GetAccountTasksQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTasksQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _taskService = new Mock<ITaskService>();

            Query = new GetAccountTasksQuery
            {
                AccountId = 2
            };

            RequestHandler = new GetAccountTasksQueryHandler(_taskService.Object, RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _taskService.Verify(x => x.GetAccountTasks(Query.AccountId.ToString()), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var tasks = new List<TaskDto>
            {
                new TaskDto {OwnerId = Query.AccountId.ToString(), Type = "test", ItemsDueCount = 2}
            };
            _taskService.Setup(x => x.GetAccountTasks(It.IsAny<string>())).ReturnsAsync(tasks);

            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(response?.Tasks);
            Assert.AreEqual(tasks.First().Type, response.Tasks.First().Type);
            Assert.AreEqual(tasks.First().ItemsDueCount, response.Tasks.First().ItemsDueCount);
        }
    }
}
