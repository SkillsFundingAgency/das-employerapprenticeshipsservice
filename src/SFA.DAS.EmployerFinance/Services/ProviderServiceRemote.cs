using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ProviderServiceRemote : IProviderService
    {
        private readonly IProviderService _providerService;
        private readonly IApiClient _apiClient;
        private readonly ILog _logger;

        public ProviderServiceRemote(IProviderService providerService, IApiClient apiClient, ILog logger)
        {
            _providerService = providerService;
            _apiClient = apiClient;
            _logger = logger;
        }
     
        public async Task<Models.ApprenticeshipProvider.Provider> Get(long ukPrn)
        {
            try
            {                
                var provider = await _apiClient.Get<ProviderResponseItem>(new GetProviderRequest(ukPrn));
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

        private static Models.ApprenticeshipProvider.Provider MapFrom(ProviderResponseItem provider)
        {
            return new Models.ApprenticeshipProvider.Provider()
            {
                Ukprn = provider.Ukprn,
                Name = provider.Name,
                Email = provider.Email,
                Phone = provider.Phone
            };
        }
    }
}
