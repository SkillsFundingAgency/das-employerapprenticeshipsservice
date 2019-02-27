using System;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ProviderServiceRemote : IProviderService
    {
        private readonly IProviderService _providerService;
        private readonly ILog _logger;

        public ProviderServiceRemote(IProviderService providerService, ILog logger)
        {
            _providerService = providerService;
            _logger = logger;
        }
     
        public async Task<Models.ApprenticeshipProvider.Provider> Get(long ukPrn)
        {
            try
            {
                var api = new Providers.Api.Client.ProviderApiClient();
                var provider = await api.GetAsync(ukPrn);
                return MapFrom(provider);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Unable to get provider details with UKPRN {ukPrn} from apprenticeship API.");

                return null;
            }
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
