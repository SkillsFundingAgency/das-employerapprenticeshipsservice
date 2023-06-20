using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Infrastructure.Data;

public class HttpResponseLogger : IHttpResponseLogger
{
    private readonly ILogger<HttpResponseLogger> _logger;

    public HttpResponseLogger(ILogger<HttpResponseLogger> logger)
    {
        _logger = logger;
    }

    public async Task LogResponseAsync(HttpResponseMessage response)
    {
        if (IsContentStringType(response))
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Logged response", new Dictionary<string, object>
            {
                { "StatusCode", response.StatusCode },
                { "Reason", response.ReasonPhrase },
                { "Content", content }
            });
        }
    }

    private static bool IsContentStringType(HttpResponseMessage response)
    {
        return response?.Content?.Headers?.ContentType != null && (
            response.Content.Headers.ContentType.MediaType.StartsWith("text") ||
            response.Content.Headers.ContentType.MediaType == "application/json");
    }
}