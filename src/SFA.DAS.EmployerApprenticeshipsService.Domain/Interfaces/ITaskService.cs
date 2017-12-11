using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Tasks.API.Types.DTOs;
using SFA.DAS.Tasks.API.Types.Enums;


namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAccountTasks(long accountId, long userId);

        Task DismissMonthlyTaskReminder(long accountId, long userId, TaskType taskType);
    }
}
