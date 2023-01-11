using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.API.Client;
using SFA.DAS.Tasks.API.Types.DTOs;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EmployerAccounts.Services;

public class TaskService : ITaskService
{
    private readonly ITaskApiClient _apiClient;
    private readonly ILog _logger;

    public TaskService(ITaskApiClient apiClient, ILog logger)
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
            _logger.Error(ex, "Could not retrieve account tasks successfully");
        }

        return new TaskDto[0];
    }

    public async Task DismissMonthlyTaskReminder(long accountId, string externalUserId, TaskType taskType)
    {
        try
        {
            if (taskType == TaskType.None) return;

            var taskName = Enum.GetName(typeof(TaskType), taskType);

            await _apiClient.AddUserReminderSupression(accountId.ToString(), externalUserId, taskName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Could not dismiss account tasks successfully");
        }
    }
}