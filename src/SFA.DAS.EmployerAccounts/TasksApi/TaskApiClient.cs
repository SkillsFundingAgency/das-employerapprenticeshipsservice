using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.TasksApi;

public interface ITaskApiClient
{
    Task<IEnumerable<TaskDto>> GetTasks(string employerAccountId, string userId, ApprenticeshipEmployerType applicableToApprenticeshipEmployerType);

    Task AddUserReminderSuppression(string employerAccountId, string userId, string taskType);
}

public class TaskApiClient : ITaskApiClient
{
    private readonly ITaskApiConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TaskApiClient> _logger;
    private readonly IAzureClientCredentialHelper _azureClientCredentialHelper;

    public TaskApiClient(HttpClient httpClient, ITaskApiConfiguration configuration, ILogger<TaskApiClient> logger, IAzureClientCredentialHelper azureClientCredentialHelper)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
        _azureClientCredentialHelper = azureClientCredentialHelper;
    }

    public async Task<IEnumerable<TaskDto>> GetTasks(string employerAccountId, string userId, ApprenticeshipEmployerType applicableToApprenticeshipEmployerType)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/tasks/{employerAccountId}/{userId}?{nameof(applicableToApprenticeshipEmployerType)}={(int)applicableToApprenticeshipEmployerType}";

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        await AddAuthenticationHeader(requestMessage);

        _logger.LogInformation("Get: {Url}", url);
        var response = await _httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonConvert.DeserializeObject<IEnumerable<TaskDto>>(content);
    }

    public async Task AddUserReminderSuppression(string employerAccountId, string userId, string taskType)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/tasks/{employerAccountId}/supressions/{userId}/add/{taskType}";

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        requestMessage.Content = new StringContent(string.Empty);
        await AddAuthenticationHeader(requestMessage);

        _logger.LogInformation("Post: {Url}", url);
        await _httpClient.SendAsync(requestMessage);
    }

    private string GetBaseUrl()
    {
        return _configuration.ApiBaseUrl.EndsWith("/")
            ? _configuration.ApiBaseUrl
            : _configuration.ApiBaseUrl + "/";
    }

    private async Task AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
    {
        if (!string.IsNullOrEmpty(_configuration.IdentifierUri))
        {
            var accessToken = await _azureClientCredentialHelper.GetAccessTokenAsync(_configuration.IdentifierUri);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}