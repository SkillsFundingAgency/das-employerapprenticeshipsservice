using System.Threading;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;

public class UnsubscribeProviderEmailQueryHandler : IRequestHandler<UnsubscribeProviderEmailQuery>
{
    private readonly IProviderRegistrationApiClient _providerRegistrationApiClient;
    private readonly ILog _logger;

    public UnsubscribeProviderEmailQueryHandler(IProviderRegistrationApiClient providerRegistrationApiClient, ILog logger)
    {
        _providerRegistrationApiClient = providerRegistrationApiClient;
        _logger = logger;
    }

    public async Task<Unit> Handle(UnsubscribeProviderEmailQuery message, CancellationToken cancellationToken)
    {
        await _providerRegistrationApiClient.Unsubscribe(message.CorrelationId.ToString());
        _logger.Info($"Sent ProviderEmail to Unsubscribe {message.CorrelationId}");

        return default;
    }
}