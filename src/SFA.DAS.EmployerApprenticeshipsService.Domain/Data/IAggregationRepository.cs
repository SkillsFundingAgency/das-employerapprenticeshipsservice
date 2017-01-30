using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IAggregationRepository
    {
        Task Update(long accountId, int pageNumber, string json);

        Task<AggregationData> GetByAccountId(long accountId);
        Task<List<AggregationData>> GetByAccountIds(List<long> accountIds);
    }
}