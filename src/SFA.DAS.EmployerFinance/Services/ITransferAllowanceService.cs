using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface ITransferAllowanceService
    {
        Task<decimal> GetTransferAllowance(long accountId);
    }
}