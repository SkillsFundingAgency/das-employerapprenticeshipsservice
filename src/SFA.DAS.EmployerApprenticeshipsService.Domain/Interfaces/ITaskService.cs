using System.Collections.Generic;


namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ITaskService
    {
        IEnumerable<TaskDto> GetAccountTasks(string accountId);
    }
}
