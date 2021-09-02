using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface ICohortsService
    {
        Task<int> GetCohortsCount(long accountId);
    }
}