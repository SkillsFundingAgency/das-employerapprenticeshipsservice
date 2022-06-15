using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail
{
    public class UnsubscribeProviderEmailQueryHandler : AsyncRequestHandler<UnsubscribeProviderEmailQuery>
    {
        private readonly IProviderRegistrationApiClient _providerRegistrationApiClient;
        private readonly ILog _logger;

        public UnsubscribeProviderEmailQueryHandler(IProviderRegistrationApiClient providerRegistrationApiClient, ILog logger)
        {
            _providerRegistrationApiClient = providerRegistrationApiClient;
            _logger = logger;
        }

        protected override async Task HandleCore(UnsubscribeProviderEmailQuery message)
        {
            await _providerRegistrationApiClient.Unsubscribe(message.CorrelationId.ToString());
            _logger.Info($"Sent ProviderEmail to Unsubscribe {message.CorrelationId}");
        }        
    }
}