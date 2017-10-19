using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Tasks.API.Types.DTOs;


namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAccountTasks(string accountId);
    }
}
