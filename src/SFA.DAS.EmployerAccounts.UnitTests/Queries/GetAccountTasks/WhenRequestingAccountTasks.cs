using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.TasksApi;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountTasks;

public class WhenRequestingAccountTasks
{
    private Mock<ITaskService> _mockTaskService;
    private Mock<IValidator<GetAccountTasksQuery>> _mockValidator;
    private readonly GetAccountTasksQuery _validQuery = new() { AccountId = 1, ExternalUserId = Guid.NewGuid().ToString() };

    [SetUp]
    public void Arrange()
    {
        _mockTaskService = new Mock<ITaskService>();
        _mockValidator = new Mock<IValidator<GetAccountTasksQuery>>();
    }

    private GetAccountTasksQueryHandler GetSut() => new(_mockTaskService.Object, _mockValidator.Object);

      
    [Test]
    public void ThenInvalidRequestExceptionIsRaisedWhenTheRequestFailsValidation()
    {
        //Arrange
        _mockValidator.Setup(x => x.Validate(It.IsAny<GetAccountTasksQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { string.Empty, string.Empty } } });
        var sut = GetSut();

        //Act
        Assert.ThrowsAsync<InvalidRequestException>(async () => await sut.Handle(new GetAccountTasksQuery(), CancellationToken.None));

        //Assert
        _mockValidator.Verify(x => x.Validate(It.IsAny<GetAccountTasksQuery>()), Times.Once);
        _mockTaskService.Verify(x => x.GetAccountTasks(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<ApprenticeshipEmployerType>()), Times.Never);
    }

    [TestCase(ApprenticeshipEmployerType.Levy, ApprenticeshipEmployerType.Levy)]
    [TestCase(ApprenticeshipEmployerType.NonLevy, ApprenticeshipEmployerType.NonLevy)]
    public async Task ThenTasksServiceIsInvoked(ApprenticeshipEmployerType actualApprenticeshipEmployerType, ApprenticeshipEmployerType expectedApprenticeshipEmployerType)
    {
        //Arrange
        _mockValidator.Setup(x => x.Validate(It.IsAny<GetAccountTasksQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
        var sut = GetSut();
        _validQuery.ApprenticeshipEmployerType = actualApprenticeshipEmployerType;

        //Act
        await sut.Handle(_validQuery, CancellationToken.None);

        //Assert
        _mockTaskService.Verify(s => s.GetAccountTasks(_validQuery.AccountId, _validQuery.ExternalUserId, expectedApprenticeshipEmployerType), Times.Once);
    }

    [Test]
    public async Task ThenTheHandlerReturnsAccountTasks()
    {
        //Arrange
        var taskDto = new TaskDto { Type = Guid.NewGuid().ToString(), ItemsDueCount = 1 };
        var taskList = new List<TaskDto>() { taskDto };
        _mockValidator.Setup(x => x.Validate(It.IsAny<GetAccountTasksQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
        _mockTaskService.Setup(x => x.GetAccountTasks(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<ApprenticeshipEmployerType>())).ReturnsAsync(taskList);
        var sut = GetSut();

        //Act
        var result = await sut.Handle(new GetAccountTasksQuery { AccountId = 1, ExternalUserId = Guid.NewGuid().ToString()}, CancellationToken.None);

        //Assert
        Assert.AreEqual(1, result.Tasks.Count);
    }

}