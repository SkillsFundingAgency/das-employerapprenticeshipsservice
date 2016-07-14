using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public interface ILevyAggregationWriter
    {
        Task UpdateAsync(AggregationData data);
        Task<AggregationData> GetAsync(int accountId, int pageNumber);
    }
}