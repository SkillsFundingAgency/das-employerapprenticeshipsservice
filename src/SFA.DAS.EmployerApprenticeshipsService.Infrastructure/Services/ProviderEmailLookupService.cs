using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ProviderEmailLookupService : IProviderEmailLookupService
    {
        private readonly ILog _logger;

        private readonly IdamsEmailServiceWrapper _idamsEmailServiceWrapper;

        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;

        private readonly CommitmentNotificationConfiguration _configuration;

        public ProviderEmailLookupService(
            ILog logger,
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
            if (!_configuration.UseProviderEmail)
            {
                _logger.Info($"Getting provider test email (${string.Join(", ", _configuration.ProviderTestEmails)})");
                return _configuration.ProviderTestEmails;
            }

            if (!string.IsNullOrEmpty(lastUpdateEmail))
            {
                _logger.Debug($"Using provider last updated email ({lastUpdateEmail})");
                return new List<string> {lastUpdateEmail};
            }

            var addresses = await _idamsEmailServiceWrapper.GetEmailsAsync(providerId);
            if (addresses.Any())
            {
                _logger.Debug($"Using provider 'DAS' emails ({string.Join(",", addresses)})");
                return addresses;
            }

            addresses = await _idamsEmailServiceWrapper.GetSuperUserEmailsAsync(providerId);
            if (addresses.Any())
            {
                _logger.Debug($"Using provider super user emails ({string.Join(",", addresses)})");
                return addresses;
            }

            if (GetProviderAddresses(providerId, out addresses))
            {
                _logger.Debug($"Using apprenticeship provider service emails ({string.Join(",", addresses)})");
                return addresses;
            }

            if (!addresses.Any())
                _logger.Warn($"Could not find any email adresses for provider: {providerId}");

            return addresses;
        }

        private bool GetProviderAddresses(long providerId, out List<string> addresses)
        {
            addresses = new List<string>();
            var providers = _apprenticeshipInfoService.GetProvider(providerId);
            if (!string.IsNullOrEmpty(providers?.Provider?.Email))
            {
                _logger.Info($"Getting email from apprenticeship provider service");
                addresses = new List<string> { providers?.Provider?.Email };
            }
            return addresses.Any();
        }
    }

}
