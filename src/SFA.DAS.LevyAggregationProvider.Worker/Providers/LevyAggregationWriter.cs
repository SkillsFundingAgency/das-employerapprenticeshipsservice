using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregationWriter : ILevyAggregationWriter
    {
        private readonly IAggregationRepository _aggregationRepository;

        public LevyAggregationWriter(IAggregationRepository aggregationRepository)
        {
            if (aggregationRepository == null) throw new ArgumentNullException(nameof(aggregationRepository));
            _aggregationRepository = aggregationRepository;
        }

        public async Task UpdateAsync(AggregationData data)
        {
            //TODO: Paging [Not Sprint 1]

            var json = JsonConvert.SerializeObject(data);

            await _aggregationRepository.Update(data.AccountId, 1, json);
        }

        public Task<AggregationData> GetAsync(int accountId, int pageNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}