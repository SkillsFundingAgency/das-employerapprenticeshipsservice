using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface ILevyTransferMatchingService
    {
        Task<int> GetPledgesCount(long accountId);
    }
}