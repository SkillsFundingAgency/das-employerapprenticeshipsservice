using Microsoft.Extensions.Logging;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.TasksApi;

namespace SFA.DAS.EmployerAccounts.Services;

public class TaskService : ITaskService
{
    private readonly ITaskApiClient _apiClient;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskApiClient apiClient, ILogger<TaskService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskDto>> GetAccountTasks(long accountId, string externalUserId, ApprenticeshipEmployerType applicableToApprenticeshipEmployerType)
    {
        try
        {
            return await _apiClient.GetTasks(accountId.ToString(), externalUserId, applicableToApprenticeshipEmployerType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not retrieve account tasks successfully");
        }

        return Array.Empty<TaskDto>();
    }

    public async Task DismissMonthlyTaskReminder(long accountId, string externalUserId, TaskType taskType)
    {
        try
        {
            if (taskType == TaskType.None) return;

            var taskName = Enum.GetName(typeof(TaskType), taskType);

            await _apiClient.AddUserReminderSuppression(accountId.ToString(), externalUserId, taskName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not dismiss account tasks successfully");
        }
    }
}