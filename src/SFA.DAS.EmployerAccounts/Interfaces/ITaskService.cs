
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.TasksApi;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAccountTasks(long accountId, string externalUserId, ApprenticeshipEmployerType applicableToApprenticeshipEmployerType);

    Task DismissMonthlyTaskReminder(long accountId, string externalUserId, TaskType taskType);
}