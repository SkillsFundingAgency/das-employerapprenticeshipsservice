using System;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ProviderServiceRemote : IProviderService
    {
        private readonly IProviderService _providerService;
        private readonly IProviderApiClient _providerApiClient;
        private readonly ILog _logger;

        public ProviderServiceRemote(IProviderService providerService, IProviderApiClient providerApiClient, ILog logger)
        {
            _providerService = providerService;
            _providerApiClient = providerApiClient;
            _logger = logger;
        }
     
        public async Task<Models.ApprenticeshipProvider.Provider> Get(long ukPrn)
        {
            try
            {                
                var provider = await _providerApiClient.GetAsync(ukPrn);
                if(provider != null)
                {
                    return MapFrom(provider);
                }                
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Unable to get provider details with UKPRN {ukPrn} from apprenticeship API.");
            }
            
            return await _providerService.Get(ukPrn);
        }

        private static Models.ApprenticeshipProvider.Provider MapFrom(Apprenticeships.Api.Types.Providers.Provider provider)
        {
            return new Models.ApprenticeshipProvider.Provider()
            {
                Ukprn = provider.Ukprn,
                ProviderName = provider.ProviderName,
                Email = provider.Email,
                Phone = provider.Phone,
                NationalProvider = provider.NationalProvider
            };
        }
    }
}
