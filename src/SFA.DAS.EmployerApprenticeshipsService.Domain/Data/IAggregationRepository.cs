using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IAggregationRepository
    {
        Task Update(int accountId, int pageNumber, string json);
    }
}