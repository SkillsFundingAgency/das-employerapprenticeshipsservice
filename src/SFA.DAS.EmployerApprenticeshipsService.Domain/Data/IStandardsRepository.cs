using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IStandardsRepository
    {
        Task<Standard[]> GetAllAsync();
        Task<Standard> GetByCodeAsync(int code);
    }
}