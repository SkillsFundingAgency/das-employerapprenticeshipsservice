using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ProviderServiceFromDb : IProviderService
    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly ILog _logger;

        public ProviderServiceFromDb(IDasLevyRepository dasLevyRepository, ILog logger)
        {
            _dasLevyRepository = dasLevyRepository;
            _logger = logger;
        }

        public async Task<Models.ApprenticeshipProvider.Provider> Get(long ukPrn)
        {
            _logger.Info($"Getting provider name from previous payments for Ukprn: {ukPrn}");
            var providerName = await _dasLevyRepository.FindHistoricalProviderName(ukPrn);

            if(providerName == null)
            {
                _logger.Warn($"No provider name found for Ukprn:{providerName} in previous payments");
            }

            return new Models.ApprenticeshipProvider.Provider {
                Name = providerName.FirstOrDefault(),
                Ukprn = ukPrn,
                IsHistoricProviderName = true
            };
        }
    }
}
