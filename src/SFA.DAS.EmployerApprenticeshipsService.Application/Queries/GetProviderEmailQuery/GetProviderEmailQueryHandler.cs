using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using NLog;

using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Queries.GetProviderEmailQuery
{
    public class GetProviderEmailQueryHandler : IAsyncRequestHandler<GetProviderEmailQueryRequest, GetProviderEmailQueryResponse>
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        private readonly ILogger _logger;

        public GetProviderEmailQueryHandler(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<GetProviderEmailQueryResponse> Handle(GetProviderEmailQueryRequest message)
        {
            if (!_configuration.SendEmailToProviders)
            {
                if (!_configuration.ProviderTestEmails.Any())
                {
                    _logger.Warn("No provider test email in configuration");
                }

                _logger.Info($"Getting provider test email (${string.Join(", ", _configuration.ProviderTestEmails)})");
                return Task.FromResult(
                    new GetProviderEmailQueryResponse { Emails = _configuration.ProviderTestEmails }
                    );
            }
            var emails = new List<string>();

            // ToDo: Call IDAMS to get emails
            // ToDo: Maybe call a backup if no email in IDAMS --> ?
            _logger.Info($"Getting {emails.Count} emails for provider {message.ProviderId}");
            var resp = new GetProviderEmailQueryResponse { Emails = emails };
            return Task.FromResult(resp);
        }
    }
}