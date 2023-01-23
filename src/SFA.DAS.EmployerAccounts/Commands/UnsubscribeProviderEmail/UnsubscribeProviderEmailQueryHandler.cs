using System.Threading;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;

public class UnsubscribeProviderEmailQueryHandler : IRequestHandler<UnsubscribeProviderEmailQuery>
{
    private readonly IProviderRegistrationApiClient _providerRegistrationApiClient;
    private readonly ILogger<UnsubscribeProviderEmailQueryHandler> _logger;

    public UnsubscribeProviderEmailQueryHandler(IProviderRegistrationApiClient providerRegistrationApiClient, ILogger<UnsubscribeProviderEmailQueryHandler> logger)
    {
        _providerRegistrationApiClient = providerRegistrationApiClient;
        _logger = logger;
    }

    public async Task<Unit> Handle(UnsubscribeProviderEmailQuery message, CancellationToken cancellationToken)
    {
        await _providerRegistrationApiClient.Unsubscribe(message.CorrelationId.ToString());
        _logger.LogInformation($"Sent ProviderEmail to Unsubscribe {message.CorrelationId}");

        return default;
    }
}