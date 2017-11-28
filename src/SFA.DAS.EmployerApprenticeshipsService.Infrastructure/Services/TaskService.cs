using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.API.Client;
using SFA.DAS.Tasks.API.Types.DTOs;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskApiClient _apiClient;
        private readonly ILog _logger;

        public TaskService(ITaskApiClient apiClient, ILog logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskDto>> GetAccountTasks(string accountId)
        {
            try
            {
                return await _apiClient.GetTasks(accountId);
            }
            catch (Exception ex)
            {
               _logger.Error(ex, "Could not retrieve account tasks successfully");
            }

            return new TaskDto[0];
        }

        public Task DismissMonthlyTaskReminder(string hashedAccountId, string hashedUserId, TaskType taskType)
        {
            throw new NotImplementedException();
        }
    }
}
