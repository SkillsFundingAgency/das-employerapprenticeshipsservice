using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Tasks.API.Types.DTOs;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAccountTasks(long accountId, string externalUserId);

        Task DismissMonthlyTaskReminder(long accountId, string externalUserId, TaskType taskType);
    }
}
