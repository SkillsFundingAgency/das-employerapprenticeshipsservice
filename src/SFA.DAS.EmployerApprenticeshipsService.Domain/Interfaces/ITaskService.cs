using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Tasks.API.Types.DTOs;
using SFA.DAS.Tasks.API.Types.Enums;


namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAccountTasks(string accountId);

        Task DismissMonthlyTaskReminder(string hashedAccountId, string hashedUserId, TaskType taskType);
    }
}
