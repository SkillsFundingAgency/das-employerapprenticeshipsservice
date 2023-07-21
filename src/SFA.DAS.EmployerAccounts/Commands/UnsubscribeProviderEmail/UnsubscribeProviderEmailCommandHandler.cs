using System.Threading;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;

public class UnsubscribeProviderEmailCommandHandler : IRequestHandler<UnsubscribeProviderEmailCommand>
{
    private readonly IProviderRegistrationApiClient _providerRegistrationApiClient;
    private readonly ILogger<UnsubscribeProviderEmailCommandHandler> _logger;

    public UnsubscribeProviderEmailCommandHandler(IProviderRegistrationApiClient providerRegistrationApiClient, ILogger<UnsubscribeProviderEmailCommandHandler> logger)
    {
        _providerRegistrationApiClient = providerRegistrationApiClient;
        _logger = logger;
    }

    public async Task<Unit> Handle(UnsubscribeProviderEmailCommand message, CancellationToken cancellationToken)
    {
        await _providerRegistrationApiClient.Unsubscribe(message.CorrelationId.ToString());
        _logger.LogInformation("Sent ProviderEmail to Unsubscribe {CorrelationId}", message.CorrelationId);

        return Unit.Value;
    }
}