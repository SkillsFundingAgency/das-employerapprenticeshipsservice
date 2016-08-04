using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IAggregationRepository
    {
        Task Update(long accountId, int pageNumber, string json);

        Task<AggregationData> GetByAccountId(long accountId);
    }
}