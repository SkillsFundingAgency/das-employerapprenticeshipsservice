using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NLog;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ProviderEmailLookupService : IProviderEmailLookupService
    {
        private readonly ILogger _logger;

        private readonly IdamsEmailServiceWrapper _idamsEmailServiceWrapper;

        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;

        private readonly CommitmentNotificationConfiguration _configuration;

        public ProviderEmailLookupService(
            ILogger logger,
            IdamsEmailServiceWrapper idamsEmailServiceWrapper,
            EmployerApprenticeshipsServiceConfiguration employerConfiguration,
            IApprenticeshipInfoServiceWrapper apprenticeshipInfoService)
        {
            _logger = logger;
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
            _apprenticeshipInfoService = apprenticeshipInfoService;
            _configuration = employerConfiguration.CommitmentNotification;
        }

        public async Task<List<string>> GetEmailsAsync(long providerId, string lastUpdateEmail)
        {
            List<string> addresses;
            if (!_configuration.UseProviderEmail)
            {
                _logger.Info($"Getting provider test email (${string.Join(", ", _configuration.ProviderTestEmails)})");
                return _configuration.ProviderTestEmails;
            }

            if (!string.IsNullOrEmpty(lastUpdateEmail))
                return new List<string> { lastUpdateEmail };

            addresses = await _idamsEmailServiceWrapper.GetEmailsAsync(providerId);
            if (addresses.Any())
                return addresses;


            // *0o* Take address from super user *0o*
            // ToDo: ?

            if (GetProviderAddresses(providerId, out addresses))
                return addresses;

            if (!addresses.Any())
                _logger.Warn($"Could not find any email adresses for provider: {providerId}");

            return addresses;
        }

        private bool GetProviderAddresses(long providerId, out List<string> addresses)
        {
            addresses = new List<string>();
            var providers = _apprenticeshipInfoService.GetProvider((int)providerId);
            if (!string.IsNullOrEmpty(providers?.Providers?.FirstOrDefault()?.Email))
            {
                _logger.Info($"Getting email from apprenticeship provider service");
                addresses = new List<string> { providers?.Providers?.FirstOrDefault()?.Email };
            }
            return addresses.Any();
        }
    }

}
