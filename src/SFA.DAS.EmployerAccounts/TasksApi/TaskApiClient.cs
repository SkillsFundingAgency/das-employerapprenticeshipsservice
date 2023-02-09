using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.TasksApi;

public interface ITaskApiClient
{
    Task<IEnumerable<TaskDto>> GetTasks(string employerAccountId, string userid, ApprenticeshipEmployerType applicableToApprenticeshipEmployerType);

    Task AddUserReminderSupression(string employerAccountId, string userId, string taskType);
}

public class TaskApiClient : ITaskApiClient
{
    private readonly ITaskApiConfiguration _configuration;
    private readonly SecureHttpClient _httpClient;
    private readonly ILogger<TaskApiClient> _logger;

    public TaskApiClient(ITaskApiConfiguration configuration, ILogger<TaskApiClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = new SecureHttpClient(configuration);
    }

    public async Task<IEnumerable<TaskDto>> GetTasks(string employerAccountId, string userId, ApprenticeshipEmployerType applicableToApprenticeshipEmployerType)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/tasks/{employerAccountId}/{userId}?{nameof(applicableToApprenticeshipEmployerType)}={(int)applicableToApprenticeshipEmployerType}";

        _logger.LogInformation($"Get: {url}");
        var json = await _httpClient.GetAsync(url);
        return JsonConvert.DeserializeObject<IEnumerable<TaskDto>>(json);
    }

    public async Task AddUserReminderSupression(string employerAccountId, string userId, string taskType)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/tasks/{employerAccountId}/supressions/{userId}/add/{taskType}";

        _logger.LogInformation($"Post: {url}");
        await _httpClient.PostAsync(url, new StringContent(string.Empty));
    }

    private string GetBaseUrl()
    {
        return _configuration.ApiBaseUrl.EndsWith("/")
            ? _configuration.ApiBaseUrl
            : _configuration.ApiBaseUrl + "/";
    }
}