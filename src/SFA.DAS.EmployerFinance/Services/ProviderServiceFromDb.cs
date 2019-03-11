using SFA.DAS.EmployerFinance.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ProviderServiceFromDb : IProviderService
    {
        private readonly IDasLevyRepository _dasLevyRepository;
        public ProviderServiceFromDb(IDasLevyRepository dasLevyRepository)
        {
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<Models.ApprenticeshipProvider.Provider> Get(long ukPrn)
        {
            var providerName = await _dasLevyRepository.FindHistoricalProviderName(ukPrn);

            // TODO: if null name then what to return?

            return new Models.ApprenticeshipProvider.Provider {
                Name = providerName.FirstOrDefault(),
                Ukprn = ukPrn,
                HistoricProviderName = true
            };
        }
    }
}
