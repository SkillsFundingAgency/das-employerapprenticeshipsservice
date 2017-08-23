using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Tasks.API.Client;
using SFA.DAS.Tasks.API.Types.DTOs;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskApiClient _apiClient;

        public TaskService(ITaskApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<TaskDto>> GetAccountTasks(string accountId)
        {
            return await _apiClient.GetTasks(accountId);
        }
    }
}
