using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IStandardsRepository
    {
        Task<Standard[]> GetAllAsync();
        Task<Standard> GetByCodeAsync(string code);
    }
}